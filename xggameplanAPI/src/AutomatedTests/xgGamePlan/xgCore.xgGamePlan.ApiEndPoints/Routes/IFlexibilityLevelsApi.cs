using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.FlexibilityLevels;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IFlexibilityLevelsApi
    {
        [Get("/FlexibilityLevels")]
        Task<List<FlexibilityLevel>> GetAll();
    }
}
