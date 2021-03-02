using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Clashes;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IClashesApi
    {
        [Get("/Clash")]
        Task<List<Clash>> GetAll();

        [Get("/Clash/{id}")]
        Task<Clash> GetById(string id);

        [Post("/Clash")]
        Task<ApiErrorResult> Create(IEnumerable<CreateClash> clashes);

        [Put("/Clash")]
        Task<ApiResponse<Clash>> Update([Body]UpdateClashModel model, Guid id, bool applyGlobally);

        [Get("/Clash/SearchAll")]
        Task<SearchResultModel<Clash>> Search(ClashSearchQueryModel searchModel);

        [Delete("/Clash/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();

        [Delete("/Clash/External/{externalReference}")]
        Task<ApiResponse<string>> DeleteByExternalReference(string externalReference);
    }
}
