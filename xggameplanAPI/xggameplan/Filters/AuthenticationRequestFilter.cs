using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using xggameplan.Areas.System.Auth;
using xggameplan.Errors;
using xggameplan.Extensions;

namespace xggameplan.Filters
{
    public class AuthenticationRequestFilter : IAuthenticationFilter
    {
        private const string BearerSchemeKey = "Bearer";
        public string Action { get; set; }
        public bool AllowMultiple { get; }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = HttpContext.Current.GetCurrentUser();
                if (currentUser != null)
                {
                    return Task.FromResult(true);
                }
                var authorization = context.Request.Headers.Authorization;
                if (authorization == null)
                {
                    // no credentials, do nothing
                    return Task.FromResult(0);
                }
                if (authorization.Scheme != BearerSchemeKey)
                {
                    // unsupported scheme. should be or skipped or throw error
                    context.ErrorResult = new AuthenticationFailureResult("Unsupported scheme", context.Request);
                    return Task.FromResult(0);
                }

                var token = authorization.Parameter;
                if (string.IsNullOrEmpty(token))
                {
                    context.ErrorResult = new AuthenticationFailureResult("Token parameter is missing", context.Request);
                    return Task.FromResult(0);
                }

                var requestScope = context.Request.GetDependencyScope();
                var authenticationManager = requestScope.GetService(typeof(IAuthenticationManager)) as IAuthenticationManager;

                var user = authenticationManager.GetAuthenticatedUser(token);
                if (user == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("User does not exist, blocked or use expired token", context.Request);
                }
                else
                {
                    HttpContext.Current.SetCurrentUser(user);
                }
            }
            catch(Exception ex)
            {
                context.ErrorResult = new AuthenticationFailureResult(ex.Message, context.Request, "Exception during authentification");
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken) => Task.FromResult(0);
    }

    internal class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string message, HttpRequestMessage request, string reasonPhrase = "" )
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
            Message = message;
        }

        public string Message { get; private set; }
        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = Request.CreateUnauthrorizedError(Message); //new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}
