using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.AccessTokens;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IAccessTokensApi
    {
        [Post("/accesstokens")]
        Task<AccessTokenModel> Create(AccessTokenCommand accessTokenCommand);

        [Post("/accesstokens")]
        Task<ApiResponse<AccessTokenModel>> CreateResponse(AccessTokenCommand accessTokenCommand);

        [Delete("/accesstokens/{token}")]
        Task Delete(string token);
    }
}
