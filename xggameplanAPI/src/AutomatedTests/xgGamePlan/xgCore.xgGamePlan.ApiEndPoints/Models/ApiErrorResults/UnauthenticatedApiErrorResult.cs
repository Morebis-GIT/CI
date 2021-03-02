using System.Net;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults
{
    public class UnauthenticatedApiErrorResult
        : ApiErrorResult
    {
        public UnauthenticatedApiErrorResult()
            : base(HttpStatusCode.Unauthorized, "Unauthenticated")
        {
            /* Empty */
        }
    }
}
