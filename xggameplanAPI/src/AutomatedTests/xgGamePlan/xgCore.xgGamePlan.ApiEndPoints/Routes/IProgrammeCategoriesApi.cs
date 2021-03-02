using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.ProgrammeCategory;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IProgrammeCategoriesApi
    {
        [Get("/ProgrammeCategories")]
        Task<IEnumerable<ProgrammeCategoryHierarchy>> GetAll();

        [Post("/ProgrammeCategories")]
        Task<ApiErrorResult> Create(List<ProgrammeCategoryHierarchy> programmeCategories);

        [Delete("/ProgrammeCategories/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
