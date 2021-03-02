using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IBreaksApi
    {
        [Get("/Breaks")]
        Task<List<Break>> GetAll();

        [Post("/Breaks")]
        Task<ApiErrorResult> Create(IEnumerable<Break> breaks);

        [Delete("/Breaks")]
        Task<ApiErrorResult> Delete([Query] DateTime dateRangeStart, [Query] DateTime dateRangeEnd, [Query(CollectionFormat.Multi)] IEnumerable<string> salesAreaNames);

        [Delete("/Breaks/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
