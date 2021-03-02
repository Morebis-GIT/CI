using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.Outputfiles;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IOutputFilesApi
    {
        [Get("/OutputFiles")]
        Task<IEnumerable<OutputFileModel>> GetAll();
    }
}
