using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Raven.Client;

namespace xggameplan.PersistenceAttributes
{
    public class RavenMasterSessionManagementAttribute : IAutofacActionFilter
    {
        private readonly IDocumentSession _session;

        public RavenMasterSessionManagementAttribute(
                    IDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public void OnActionExecuting(
            HttpActionContext context)
        {
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        /// <param name="token"></param>
        public Task OnActionExecutingAsync(
            HttpActionContext context, CancellationToken token)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public void OnActionExecuted(
            HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                return;
            }

            lock (_session)
            {
                _session?.SaveChanges();
                _session?.Advanced.Clear();
                _session?.Dispose();
            }
        }

        public Task OnActionExecutedAsync(
           HttpActionExecutedContext context, CancellationToken token)
        {
            if (context.Exception != null)
            {
                return Task.FromResult(0);
            }
            lock (_session)
            {
                _session?.SaveChanges();  // Is this correct in asyc method?
                _session?.Advanced.Clear();
                _session?.Dispose();
            }
            return Task.FromResult(0);
        }
    }
}
