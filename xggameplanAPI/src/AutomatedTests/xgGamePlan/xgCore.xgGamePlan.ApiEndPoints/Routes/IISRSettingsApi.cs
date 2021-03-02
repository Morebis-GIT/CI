using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ISRSettings;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IISRSettingsApi
    {
        [Get("/Runs/ISRSettings")]
        Task<ISRSettingsModel> GetISRSettings([Query] string salesArea);

        [Post("/Runs/ISRSettings")]
        Task<ISRSettingsModel> Create(ISRSettingsModel iSRSettingsModel);

        [Put("/Runs/ISRSettings")]
        Task<ISRSettingsModel> Update(ISRSettingsModel iSRSettingsModel);

        [Get("/Runs/ISRSettings/compare")]
        Task<ISRSettingsCompareModel> Compare();
    }
}
