using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IAutopilotSettingsApi
    {
        [Get("/AutopilotSettings/default")]
        Task<AutopilotSettingsModel> GetDefault();

        [Put("/AutopilotSettings/default/{id}")]
        Task<AutopilotSettingsModel> PutDefault(int id, UpdateAutopilotSettingsModel model);
    }
}
