using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.AccessTokens;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Contexts;
using xgCore.xgGamePlan.AutomationTests.Coordinators;

namespace xgCore.xgGamePlan.AutomationTests.Features.AccessToken
{
    [Binding]
    public class AccessTokensSteps
    {
        private readonly IAccessTokensApi _accessTokensApi;
        private readonly AuthContext _authContext;
        private readonly AccessTokenCoordinator _accessTokenCoordinator;
        private readonly UsersCoordinator _usersCoordinator;

        public AccessTokensSteps(
            IAccessTokensApi accessTokensApi,
            AuthContext authContext,
            AccessTokenCoordinator accessTokenCoordinator,
            UsersCoordinator usersCoordinator)
        {
            _accessTokensApi = accessTokensApi;
            _authContext = authContext;
            _accessTokenCoordinator = accessTokenCoordinator;
            _usersCoordinator = usersCoordinator;
        }

        [When(@"I add new AccessToken")]
        public async Task WhenIAddNewAccessTokenAsync()
        {
            _authContext.AccessToken = await _accessTokenCoordinator.CreateAccessToken(_authContext.UserCredentials)
                .ConfigureAwait(false);
        }

        [Then(@"new AccessToken returned")]
        public void ThenNewAccessTokenReturned()
        {
            Assert.IsNotNull(_authContext.AccessToken?.Token);
        }

        [When(@"I try to add new AccessToken")]
        public async Task WhenITryToAddNewAccessTokenAsync()
        {
            _authContext.AccessTokenResponse = await _accessTokenCoordinator.TryCreateAccessToken(_authContext.UserCredentials)
                .ConfigureAwait(false);
        }

        [Then(@"Unauthorized is returned by server")]
        public void ThenUnauthorizedStatusReturned()
        {
            Assert.AreEqual(HttpStatusCode.Unauthorized, _authContext.AccessTokenResponse?.ResponseStatus);
        }

        [Given(@"I have created new AccessToken")]
        public async Task GivenIHaveCreatedNewAccessTokenAsync()
        {
            _authContext.AccessToken = await _accessTokenCoordinator
                .CreateAccessToken(await _usersCoordinator.CreateUserWithCredentials().ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [When(@"I remove AccessToken")]
        public async Task WhenIRemoveAccessTokenAsync()
        {
            await _accessTokensApi.Delete(_authContext.AccessToken?.Token).ConfigureAwait(false);
        }
    }
}
