using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.AnalysisGroups;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IAnalysisGroupApi
    {
        [Get("/analysis-groups")]
        Task<IEnumerable<AnalysisGroupModel>> GetAll();

        [Get("/analysis-groups/{id}")]
        Task<AnalysisGroupModel> GetById(int id);

        [Post("/analysis-groups")]
        Task<AnalysisGroupModel> Create(CreateAnalysisGroupModel model);

        [Put("/analysis-groups/{id}")]
        Task<AnalysisGroupModel> Update(int id, CreateAnalysisGroupModel model);

        [Delete("/analysis-groups/{id}")]
        Task Delete(int id);
    }
}
