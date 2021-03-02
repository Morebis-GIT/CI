using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ISalesAreasApi
    {
        [Get("/SalesAreas")]
        Task<IEnumerable<SalesArea>> GetAll();

        [Get("/SalesAreas/id")]
        Task<SalesArea> GetById(int id);

        [Get("/SalesAreas/id")]
        Task<ApiResponse<SalesArea>> GetByIdResponse(int id);

        [Post("/SalesAreas")]
        Task<SalesArea> Create(SalesArea salesArea);

        [Put("/SalesAreas")]
        Task<ApiErrorResult> Update(SalesArea salesArea);

        [Delete("/SalesAreas")]
        Task<ApiErrorResult> DeleteById(int id);
    }
}
