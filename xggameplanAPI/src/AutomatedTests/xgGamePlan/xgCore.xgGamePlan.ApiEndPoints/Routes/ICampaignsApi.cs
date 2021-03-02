using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ICampaignsApi
    {
        [Get("/Campaigns")]
        Task<List<Campaign>> GetAll();

        [Post("/Campaigns")]
        Task<ApiResponse<object>> Create(List<CreateCampaign> universes);

        [Delete("/Campaigns/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();

        [Get("/Campaigns/{id}")]
        Task<Campaign> GetById(Guid id);

        [Get("/Campaigns/externalRef/{externalId}")]
        Task<Campaign> GetByExternalRef(string externalId);

        [Get("/Campaigns/Group/{group}")]
        Task<List<Campaign>> GetByGroup(string group);

        [Get("/Campaigns/Search")]
        Task<SearchResultModel<Campaign>> Search([Query] CampaignSearchQueryModel model);

        [Put("/Campaigns/externalRef/{externalId}")]
        Task<Campaign> Put(string externalId, CreateCampaign command);
    }
}
