using System.Net;

namespace xgCore.xgGamePlan.AutomationTests.Infrastructure
{
    public class AccessTokenResponse
    {
        public string Token { get; set; }

        public HttpStatusCode ResponseStatus { get; set; }

        public string ServerExceptionType { get; set; }
    }
}
