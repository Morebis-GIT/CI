using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ClearanceCodes;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IClearanceCodeApi
    {
        [Get("/ClearanceCode")]
        Task<List<ClearanceCode>> GetAll();

        [Post("/ClearanceCode")]
        Task<List<ClearanceCodeResult>> Create(List<ClearanceCode> clearanceCodes);
    }
}
