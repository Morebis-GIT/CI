using System;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Multitenant;
using xggameplan.Areas.System.Auth;
using xggameplan.Extensions;

namespace xggameplan.Services
{
    public class TenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        private const string AuthorizationHeaderKey = "Authorization";
        private const string BearerKey = "Bearer";

        private readonly IContainer _container;
        private readonly string[] _masterControllerPrefixes = { "/tenants", "/users", "/accessTokens" };

        public TenantIdentificationStrategy(IContainer container)
        {
            _container = container;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            try
            {
                var httpContext = HttpContext.Current;

                tenantId = httpContext.GetTenantId();
                if (tenantId != null)
                {
                    if (tenantId is int && (int)tenantId == -1)
                    {
                        return false;
                    }

                    return true;
                }

                try
                {
                    if (_masterControllerPrefixes.Any(s => httpContext.Request.RawUrl.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                }
                catch (HttpException)
                {
                    return false;
                }

                var accessToken = GetAccessToken(httpContext.Request);
                if (accessToken == null)
                {
                    return false;
                }

                using var scope = _container.BeginLifetimeScope();
                var user = scope.Resolve<IAuthenticationManager>().GetAuthenticatedUser(accessToken);
                tenantId = user?.TenantId;
                httpContext.SetTenantId(tenantId ?? -1);

                // Following lines should not be here after implementation of getting tenantid from hostname / header param/ configuration param/ accessToken
                if (user != null)
                {
                    httpContext.SetCurrentUser(user);
                }

                return tenantId != null;
            }
            catch (Exception ex)
            {
                tenantId = null;
                if (ex.Message.Equals("Request is not available in this context"))
                {
                    return false; // Ignore startup errors (Application_Start);
                }

                throw ex;
            }
        }

        private string GetAccessToken(HttpRequest httpRequest)
        {
            string authorizationHeaderValue = string.Empty;
            authorizationHeaderValue = httpRequest.Headers[AuthorizationHeaderKey];
            if (string.IsNullOrEmpty(authorizationHeaderValue))
            {
                return null;
            }

            var authHeader = authorizationHeaderValue.Split(' ');
            if (authHeader.Length != 2 || authHeader[0] != BearerKey || String.IsNullOrWhiteSpace(authHeader[1]))
            {
                return null;
            }

            return authHeader[1];
        }
    }
}
