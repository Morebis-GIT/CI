using xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class PassPriorityModel
    {
        public int PassId { get; set; }
        public string PassName { get; set; }
        public CampaignPassPriorityType Priority { get; set; }
    }
}
