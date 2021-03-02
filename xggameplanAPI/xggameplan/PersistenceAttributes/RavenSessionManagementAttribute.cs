using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Raven.Client;

namespace xggameplan.PersistenceAttributes
{
    public class RavenSessionManagementAttribute : IAutofacActionFilter
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _asyncSession;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="asyncSession"></param>
        public RavenSessionManagementAttribute(
            IDocumentSession session,
            IAsyncDocumentSession asyncSession)
        {
            _session = session;
            _asyncSession = asyncSession;
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
        /// <param name="context">The context for the action.
        /// </param>
        /// <param name="token"></param>
        public Task OnActionExecutingAsync(
            HttpActionContext context,
            CancellationToken token)
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

            if (_session != null)
            {
                if (_session.Advanced.NumberOfRequests > 30)
                {
                    Debug.WriteLine("Number of recommended (30) remote call requests have been exceeded");
                }

                lock (_session)
                {
                    _session.SaveChanges();
                    _session.Advanced.Clear();
                    _session.Dispose();
                }
            }

            if (_asyncSession != null)
            {
                _asyncSession.SaveChangesAsync();
                _asyncSession.Advanced.Clear();
                _asyncSession.Dispose();
            }
        }

        public async Task OnActionExecutedAsync(
           HttpActionExecutedContext context, CancellationToken token)
        {
            if (context.Exception != null)
            {
                return;
            }

            if (_session != null)
            {
                if (_session.Advanced.NumberOfRequests > 30)
                {
                    Debug.WriteLine("Number of recommended (30) remote call requests have been exceeded");
                }

                lock (_session)
                {
                    _session.SaveChanges();
                    _session.Advanced.Clear();
                    _session.Dispose();
                }
            }

            if (_asyncSession != null)
            {
                await _asyncSession.SaveChangesAsync();
                _asyncSession.Advanced.Clear();
                _asyncSession.Dispose();
            }
        }
    }
}
