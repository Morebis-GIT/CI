using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IScenariosApi
    {
        [Get("/Scenarios")]
        Task<IEnumerable<Scenario>> GetAll();

        [Get("/Scenarios/{id}")]
        Task<Scenario> GetById(Guid id);

        [Get("/Scenarios/{id}")]
        Task<ApiResponse<Scenario>> GetByIdResponse(Guid id);

        [Get("/Scenarios/default")]
        Task<DefaultScenarioResponse> GetDefault();

        [Post("/Scenarios")]
        Task<Scenario> Create(Scenario scenario);

        [Put("/Scenarios/{id}")]
        Task<Scenario> UpdateById(Guid id, Scenario scenario);

        [Put("/Scenarios/default/{id}")]
        Task<ApiErrorResult> UpdateDefault(Guid id);

        [Get("/Scenarios/search")]
        Task<SearchResultModel<Scenario>> Search([Query] ScenarioSearchQueryModel scenarioSearchQuery);

        [Delete("/Scenarios/{id}")]
        Task<ApiErrorResult> DeleteById(Guid id);        
    }
}
