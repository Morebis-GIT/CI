using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Programmes;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IProgrammesApi
    {
        [Get("/Programmes")]
        Task<IEnumerable<Programme>> GetAll();

        [Post("/Programmes")]
        Task<ApiErrorResult> Create(IEnumerable<Programme> programmes);

        [Delete("/Programmes/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
