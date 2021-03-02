using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Demographics;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IDemographicsApi
    {
        [Get("/Demographics")]
        Task<IEnumerable<Demographic>> GetAll();

        [Get("/Demographics/Gameplan")]
        Task<IEnumerable<Demographic>> GetAllGamePlan();

        [Post("/Demographics")]
        Task<ApiErrorResult> Create(IEnumerable<Demographic> demographic);

        [Get("/Demographics/externalref/{externalRef}")]
        Task<Demographic> GetByExternalRef(string externalRef);
    }
}
