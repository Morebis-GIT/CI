using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Channels;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IChannelsApi
    {
        [Get("/Channels")]
        Task<List<Channel>> GetAll();

        [Post("/Channels")]
        Task<ApiErrorResult> Create(Channel channel);

        [Delete("/Channels")]
        Task<ApiErrorResult> Delete(int id);
    }
}
