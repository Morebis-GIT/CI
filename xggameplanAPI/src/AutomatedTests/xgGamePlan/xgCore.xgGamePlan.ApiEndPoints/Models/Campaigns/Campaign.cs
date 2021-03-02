using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class Campaign
    {
        public Guid Uid { get; set; }
        public string ExternalId { get; set; }
        public string CampaignGroup { get; set; }
        public string Status { get; set; }
    }
}
