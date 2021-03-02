using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.FunctionalAreas;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IFunctionalAreasApi
    {
        [Get("/FunctionalAreas")]
        Task<IEnumerable<FunctionalArea>> GetAll();
    }
}
