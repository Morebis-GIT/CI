using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace xggameplan.PersistenceAttributes
{
    public class SqlServerDbContextManagementAttribute : IAutofacActionFilter
    {
        private readonly IEnumerable<ISqlServerDbContext> _dbContexts;

        public SqlServerDbContextManagementAttribute(IEnumerable<ISqlServerDbContext> dbContexts)
        {
            _dbContexts = dbContexts;
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="context">The context for the action.
        /// </param>
        /// <param name="token"></param>
        public Task OnActionExecutingAsync(HttpActionContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public async Task OnActionExecutedAsync(
           HttpActionExecutedContext context, CancellationToken token)
        {
            if (context.Exception != null)
            {
                return;
            }

            await Task.WhenAll(_dbContexts.Select(async db => await db.SaveChangesAsync(token).ConfigureAwait(false)))
                .ConfigureAwait(false);
        }
    }
}
