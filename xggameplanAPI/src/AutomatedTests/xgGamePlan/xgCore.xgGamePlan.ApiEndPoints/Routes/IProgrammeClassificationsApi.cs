using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.ProgrammeClassifications;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IProgrammeClassificationsApi
    {
        [Get("/Programmes/Classifications")]
        Task<IEnumerable<ProgrammeClassification>> GetAll();

        [Post("/Programmes/Classifications")]
        Task<ApiErrorResult> Create(IEnumerable<ProgrammeClassification> programmeClassifications);

        [Delete("/Programmes/Classifications/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
