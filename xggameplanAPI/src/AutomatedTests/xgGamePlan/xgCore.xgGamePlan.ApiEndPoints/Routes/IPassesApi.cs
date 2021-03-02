using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IPassesApi
    {
        [Get("/Passes")]
        Task<IEnumerable<Pass>> GetAll();

        [Get("/Passes/{id}")]
        Task<Pass> GetById(int id);

        [Delete("/Passes/{id}")]
        Task<ApiErrorResult> DeleteById(int id);

        [Post("/Passes")]
        Task<Pass> Create(Pass pass);

        [Put("/Passes/{id}")]
        Task<Pass> Update(int id, Pass pass);

        [Get("/Passes/search")]
        Task<SearchResultModel<Pass>> Search([Query] PassSearchQueryModel passSearchQuery);
    }
}
