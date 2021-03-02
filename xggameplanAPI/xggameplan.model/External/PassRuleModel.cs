using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace xggameplan.Model
{
    public class PassRuleModel
    {
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Ignore current slotting controls rule
        /// </summary>
        public bool Ignore { get; set; }
        public string Value { get; set; }
        public string PeakValue { get; set; }
        public string Type { get; set; }
        public CampaignDeliveryType CampaignType { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
