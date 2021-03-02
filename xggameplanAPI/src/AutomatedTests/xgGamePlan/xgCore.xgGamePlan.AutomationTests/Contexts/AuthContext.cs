using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.AccessTokens;
using xgCore.xgGamePlan.AutomationTests.Infrastructure;

namespace xgCore.xgGamePlan.AutomationTests.Contexts
{
    [Binding]
    public class AuthContext
    {
        public UserCredentials UserCredentials { get; set; }

        public AccessTokenModel AccessToken { get; set; }

        public AccessTokenResponse AccessTokenResponse { get; set; }
    }
}
