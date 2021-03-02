using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;

namespace xggameplan.core.Services
{
    public class AnalysisGroupCampaignQuery : IAnalysisGroupCampaignQuery
    {
        private readonly ICampaignRepository _campaignRepository;

        public AnalysisGroupCampaignQuery(ICampaignRepository campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        public IEnumerable<Guid> GetAnalysisGroupCampaigns(AnalysisGroupFilter filter)
        {
            var mapperFilter = new CampaignSearchQueryModel()
            {
                BusinessTypes = filter.BusinessTypes.ToList(),
                ClashCodes = filter.ClashExternalRefs.ToList(),
                CampaignIds = filter.CampaignExternalIds.ToList(),
                ProductIds = filter.ProductExternalIds.ToList(),
                AgencyIds = filter.AgencyExternalIds.ToList(),
                AdvertiserIds = filter.AdvertiserExternalIds.ToList(),
                ReportingCategories = filter.ReportingCategories.ToList(),
                MediaSalesGroupIds = filter.AgencyGroupCodes.ToList(),
                ProductAssigneeIds = filter.SalesExecExternalIds.Select(c => c.ToString()).ToList()
            };

            var campaigns = _campaignRepository.GetWithProduct(mapperFilter);

            return campaigns.Items.Select(c => c.Uid)
                .Distinct()
                .ToArray();
        }
    }
}
