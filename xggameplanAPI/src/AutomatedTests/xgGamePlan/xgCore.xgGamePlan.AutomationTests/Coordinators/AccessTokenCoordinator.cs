using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Api;
using xgCore.xgGamePlan.ApiEndPoints.Models.AccessTokens;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Extensions;
using xgCore.xgGamePlan.AutomationTests.Infrastructure;

namespace xgCore.xgGamePlan.AutomationTests.Coordinators
{
    [Binding]
    public class AccessTokenCoordinator
    {
        private readonly IAccessTokensApi _accessTokensApi;

        public AccessTokenCoordinator(IAccessTokensApi accessTokensApi)
        {
            _accessTokensApi = accessTokensApi;
        }

        public async Task<AccessTokenModel> CreateAccessToken(UserCredentials userCredentials)
        {
            return await _accessTokensApi
                .Create((userCredentials ?? throw new ArgumentNullException(nameof(userCredentials)))
                    .ToAccessTokenCommand()).ConfigureAwait(false);
        }

        public async Task<AccessTokenResponse> TryCreateAccessToken(UserCredentials userCredentials)
        {
            var response = await _accessTokensApi
                .CreateResponse((userCredentials ?? throw new ArgumentNullException(nameof(userCredentials)))
                    .ToAccessTokenCommand()).ConfigureAwait(false);

            var responseErrorInfo = response.Error != null ? response.Error.ToResponseErrorInfo() : null;
            return new AccessTokenResponse
            {
                Token = response.Content?.Token,
                ResponseStatus = response.StatusCode,
                ServerExceptionType = responseErrorInfo?.ExceptionType
            };
        }
    }
}
