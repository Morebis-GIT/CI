using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.BRS;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IBRSTemplateApi
    {
        [Get("/brs-templates")]
        Task<IEnumerable<BRSConfigurationTemplateModel>> GetAll();

        [Get("/brs-templates/{id}")]
        Task<BRSConfigurationTemplateModel> GetById(int id);

        [Post("/brs-templates")]
        Task<BRSConfigurationTemplateModel> Create(CreateOrUpdateBRSConfigurationTemplateModel model);

        [Put("/brs-templates/{id}")]
        Task<BRSConfigurationTemplateModel> Update(int id, CreateOrUpdateBRSConfigurationTemplateModel model);

        [Delete("/brs-templates/{id}")]
        Task Delete(int id);

        [Put("/brs-templates/default/{id}")]
        Task SetAsDefault(int id);
    }
}
