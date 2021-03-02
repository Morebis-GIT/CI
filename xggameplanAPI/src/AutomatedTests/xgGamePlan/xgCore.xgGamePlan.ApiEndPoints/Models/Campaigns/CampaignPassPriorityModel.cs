using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class CampaignPassPriorityModel
    {
        public string CampaignExternalId { get; set; }
        public CampaignListItemModel Campaign { get; set; }
        public IEnumerable<PassPriorityModel> PassPriorities { get; set; }
    }
}
