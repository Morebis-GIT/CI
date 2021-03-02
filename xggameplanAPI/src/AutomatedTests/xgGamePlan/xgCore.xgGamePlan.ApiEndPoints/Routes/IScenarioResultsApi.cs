using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IScenarioResultsApi
    {
        [Get("/ScenarioResults/{id}")]
        Task<ScenarioResultModel> GetResultByScenarioId(Guid id);

        [Get("/ScenarioResults/{scenarioId}/topfailures")]
        Task<IEnumerable<TopFailureModel>> GetTopFailures(Guid scenarioId);

        [Get("/ScenarioResults/{scenarioId}/failures")]
        Task<IEnumerable<FailureModel>> GetFailures(Guid scenarioId);

        [Get("/ScenarioResults/{scenarioId}/metrics")]
        Task<ScenarioResult> GetMetrics(Guid scenarioId);

        [Get("/ScenarioResults/{scenarioId}/drilldown")]
        Task<IEnumerable<GroupResult>> GetDrilldown(Guid scenarioId, [Query] string groupbynames);

        [Get("/ScenarioResults/recommendations/simple")]
        Task<IEnumerable<RecommendationSimpleModel>> GetSimleRecommendationsForScenarios([Query(CollectionFormat.Multi)] IEnumerable<Guid> scenarioIds);

        [Get("/ScenarioResults/{scenarioId}/recommendations/simple")]
        Task<IEnumerable<RecommendationSimpleModel>> GetSimleRecommendationsForScenario(Guid scenarioId);

        [Get("/ScenarioResults/{scenarioId}/recommendations/aggregate")]
        Task<IEnumerable<RecommendationAggregateModel>> GetAggregatedReccomendations(Guid scenarioId);

        [Delete("/ScenarioResults/{id}")]
        Task<ScenarioResultModel> DeleteResultByScenarioId(Guid id);
        
        [Get("/ScenarioResults/{id}/outputfiles/{fileId}")]
        Task<HttpContent> GetOutputFilesById(Guid id, string fileId);

        [Get("/ScenarioResults/{id}/outputfiles/{fileId}")]
        Task<ApiResponse<HttpContent>> GetOutputFilesByIdResponse(Guid id, string fileId);
    }
}
