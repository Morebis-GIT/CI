using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ISponsorshipsApi
    {
        [Get("/Sponsorships")]
        Task<List<SponsorshipModel>> GetAll(string orderBy = "");

        [Post("/Sponsorships")]
        Task<ApiErrorResult> Create(IEnumerable<CreateSponsorshipModel> createSponsorshipModel);

        [Delete("/Sponsorships/{externalReferenceId}")]
        Task<ApiErrorResult> Delete(string externalReferenceId);

        [Delete("/Sponsorships/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();

        [Put("/Sponsorships")]
        Task<ApiErrorResult> Update(CreateSponsorshipModel updateSponsorshipModel);
    }
}
