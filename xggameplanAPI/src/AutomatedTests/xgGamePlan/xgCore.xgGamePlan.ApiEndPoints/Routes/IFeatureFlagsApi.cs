using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.Features;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IFeatureFlagsApi
    {
        [Get("/FeatureFlags")]
        Task<List<FeatureStateModel>> GetAll();
    }
}
