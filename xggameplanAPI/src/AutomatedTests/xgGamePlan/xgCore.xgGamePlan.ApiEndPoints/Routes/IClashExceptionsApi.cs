using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.ClashExceptions;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IClashExceptionsApi
    {
        [Get("/ClashException")]
        Task<GetClashExceptionsResult> GetAll([Query] ClashExceptionsGetQueryModel queryModel);

        [Post("/ClashException")]
        Task<List<ClashException>> Create(IEnumerable<ClashException> clashException);

        [Get("/ClashException/{id}")]
        Task<ClashException> GetById(int id);

        [Get("/ClashException/{id}")]
        Task<ApiResponse<ClashException>> GetByIdResponse(int id);

        [Put("/ClashException/{id}")]
        Task<ApiResponse<ClashException>> Update(int id, ClashExceptionUpdateModel model);

        [Delete("/ClashException/{id}")]
        Task<ApiResponse<string>> Delete(int id);

        [Delete("/ClashException")]
        Task<ApiResponse<string>> DeleteAllByDate(DateTime dateRangeStart, DateTime dateRangeEnd);
    }
}
