using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiConnectivity;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    public interface IApiConnectivity
    {
        [Get("/api/Version")]
        Task<ApiVersionResult> Version();

        [Get("/api/TestVerb")]
        Task<TestVerbResult> TestGetVerb();

        [Post("/api/TestVerb")]
        Task<TestVerbResult> TestPostVerb();

        [Put("/api/TestVerb")]
        Task<TestVerbResult> TestPutVerb();

        [Delete("/api/TestVerb")]
        Task<TestVerbResult> TestDeleteVerb();

        [Patch("/api/TestVerb")]
        Task<TestVerbResult> TestPatchVerb();
    }
}
