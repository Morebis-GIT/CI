using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.RSSettings;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IRSSettingsApi
    {
        [Get("/Runs/RSSettings")]
        Task<RSSettingsModel> GetBySalesArea([Query]string salesArea);

        [Post("/Runs/RSSettings")]
        Task<RSSettingsModel> Create(RSSettingsModel command);

        [Put("/Runs/RSSettings")]
        Task<RSSettingsModel> Update(RSSettingsModel command, [Query]int updateMode = 0);

        [Delete("/Runs/RSSettings")]
        Task<ApiErrorResult> Delete([Query]string salesArea);

        [Get("/Runs/RSSettings/compare")]
        Task<ApiErrorResult> Compare([Query]int compareMode = 0);
    }
}
