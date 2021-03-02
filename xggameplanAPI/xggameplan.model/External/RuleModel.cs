using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace xggameplan.Model
{
    public class RuleModel
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int RuleTypeId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public CampaignDeliveryType? CampaignType { get; set; }
        public string UniqueRuleKey => RuleId + "_" + RuleTypeId;

        public object Clone() => MemberwiseClone();
    }
}
