using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Universes;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IUniversesApi
    {
        [Get("/Universes")]
        Task<List<Universe>> GetAll();

        [Post("/Universes")]
        Task<ApiErrorResult> Create(List<Universe> universes);

        [Delete("/Universes/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
