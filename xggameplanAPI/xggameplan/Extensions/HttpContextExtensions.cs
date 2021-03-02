using System.Web;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;

namespace xggameplan.Extensions
{
    public static class HttpContextExtensions
    {
        const string TenantIdKey = "tenantId";
        const string TokenKey = "token";
        const string UserKey = "user";
      
        public static object GetTenantId(this HttpContext httpContext) =>
                httpContext.Items.Contains(TenantIdKey)
                            ? httpContext.Items[TenantIdKey]
                            : null;
        
        public static void SetTenantId(this HttpContext httpContext, object tenantId) =>
            httpContext.Items[TenantIdKey] = tenantId;


        public static User GetCurrentUser(this HttpContext httpContext) =>
            httpContext.Items.Contains(UserKey) ? (User)httpContext.Items[UserKey] : null;

        public static void SetCurrentUser(this HttpContext httpContext, User user) =>
            httpContext.Items[UserKey] = user;
    }
}
