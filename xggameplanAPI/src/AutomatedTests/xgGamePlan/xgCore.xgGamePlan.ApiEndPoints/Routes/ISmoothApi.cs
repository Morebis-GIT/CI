using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.Smooth;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ISmoothApi
    {
        [Post("/Smooth/Configuration/{id}/Validate")]
        Task<SmoothValidationResult> ValidateSmoothConfiguration(int id);

        [Get("/Smooth/Configuration/{id}/Passes/Export")]
        Task ExportSmoothConfigurationForPasses(int id);

        [Get("/Smooth/Configuration/{id}/BestBreakFactorGroups/Export")]
        Task ExportSmoothConfigurationForBestBreakFactorGroups(int id);
    }
}
