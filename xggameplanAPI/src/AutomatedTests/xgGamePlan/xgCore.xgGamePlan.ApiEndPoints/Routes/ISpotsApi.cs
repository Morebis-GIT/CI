using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Spots;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ISpotsApi
    {
        [Get("/Spots")]
        Task<IEnumerable<Spot>> GetAll();

        [Get("/Spots/Search")]
        Task<IEnumerable<SpotModel>> Search([Query] SearchSpotsQuery query);

        [Get("/Spots/BreakAndProgrammeInfo")]
        Task<IEnumerable<SpotWithBreakAndProgrammeInfo>> SearchWithBreakAndProgrammeInfo([Query] SearchSpotsQuery query);

        [Post("/Spots")]
        Task<ApiErrorResult> Create(IEnumerable<Spot> spots);

        [Delete("/Spots")]
        Task<ApiErrorResult> Delete([Query] DateTime dateRangeStart, [Query] DateTime dateRangeEnd, [Query(CollectionFormat.Multi)] IEnumerable<string> salesAreaNames);

        [Delete("/Spots/externalRef")]
        Task<ApiErrorResult> Delete([Query(CollectionFormat.Multi)] List<string> externalSpotRef);

        [Delete("/Spots/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();

        [Put("/Spots/externalRef/{externalId}")]
        Task<ApiResponse<Spot>> Put(string externalId, CreateUpdateSpot spot);
    }
}
