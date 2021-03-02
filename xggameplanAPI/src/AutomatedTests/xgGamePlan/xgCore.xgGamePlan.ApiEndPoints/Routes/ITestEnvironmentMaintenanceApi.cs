using System;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.Runs;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ITestEnvironmentMaintenanceApi
    {
        [Post("/api/Tests/SmoothConfiguration")]
        Task<int> PopulateSmoothConfiguration();

        [Post("/api/Tests/RunResult")]
        Task<TestEnvironmentRunResult> PopulateRunResultData();
    }
}
