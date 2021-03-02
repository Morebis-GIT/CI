using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace xggameplan.Model
{
    public class AutopilotRuleModel
    {
        public string UniqueRuleKey { get; set; }
        public string RuleType { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public CampaignDeliveryType? CampaignType { get; set; }
    }
}
