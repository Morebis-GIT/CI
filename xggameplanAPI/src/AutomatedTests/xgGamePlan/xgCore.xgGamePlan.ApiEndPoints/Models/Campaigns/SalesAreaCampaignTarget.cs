using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class SalesAreaCampaignTarget
    {
        public string SalesArea { get; set; }
        public SalesAreaGroup SalesAreaGroup { get; set; }
        public IEnumerable<Multipart> Multiparts { get; set; }
        public IEnumerable<CampaignTarget> CampaignTargets { get; set; }
    }
}
