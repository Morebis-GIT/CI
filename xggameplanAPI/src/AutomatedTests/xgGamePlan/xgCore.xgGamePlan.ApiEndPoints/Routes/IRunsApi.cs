using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings;
using xgCore.xgGamePlan.ApiEndPoints.Models.Runs;
using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IRunsApi
    {
        [Get("/Runs")]
        Task<List<Run>> GetAll(string orderBy = "");

        [Post("/Runs")]
        Task<Run> Create(CreateRunModel createRunModel);

        [Get("/Runs/search")]
        Task<SearchResultModel<RunSearchResult>> Search([Query] RunSearchQueryModel runSearchQuery);

        [Get("/Runs/{id}")]
        Task<Run> GetById(Guid id);

        [Delete("/Runs/{id}")]
        Task<SearchResultModel<Run>> DeleteById(Guid id);

        [Put("/Runs/{id}")]
        Task<Run> UpdateById(Guid id, Run run);

        [Get("/Runs/{runId}/metrics")]
        Task<IEnumerable<ScenarioMetricsResultModel>> GetMetrics(Guid runId);

        [Post("/Runs/AutopilotScenarios")]
        Task<List<Scenario>> PostAutopilotScenarios(AutopilotEngageModel model);
    }
}
