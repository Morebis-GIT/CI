using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using xggameplan.Areas.System.Auth;
using xggameplan.Errors;
using xggameplan.Extensions;

namespace xggameplan.Filters
{
    /// <summary>
    /// Attribute for securing API endpoints by checking the access token in the
    /// Authorize header.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeRequestAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Identification of the action the user is trying to execute. Based on the
        /// action being performed, the access can be granted or revoked.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AuthorizeRequestAttribute(string action) => Action = action;

        /// <summary>
        /// Checks whether the current request contains and access token in its
        /// Authorization header and if the token is valid.
        /// </summary>
        /// <param name="actionContext">Current action token</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var requestScope = actionContext.Request.GetDependencyScope();

            var user = HttpContext.Current.GetCurrentUser();
            if (user is null)
            {
                actionContext.Response = actionContext.Request.CreateUnauthrorizedError(
                    "User is not valid"
                    );

                return;
            }

            var authorizationManager = requestScope
                .GetService(typeof(IAuthorizationManager)) as IAuthorizationManager;

            if (!authorizationManager.IsAuthorized(user, Action))
            {
                actionContext.Response = actionContext.Request.CreateUnauthrorizedError(
                    "User is not authorized to perform the requested operation"
                    );
            }
        }
    }
}
