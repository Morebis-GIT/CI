using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Passes;

namespace xggameplan.Model
{
    public class ToleranceModel
    {
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public int Under { get; set; }
        public int Over { get; set; }
        public bool Ignore { get; set; }
        public bool BookTargetArea { get; set; }

        /// <summary>
        /// ‘ForceUnderOver’ is stored in the DB as ‘ForceOverUnder’ to avoid conflicts with the
        /// existing property – ‘ForceUnderOver’ that was a Boolean and its now removed but we want the runs which
        /// used that property to load properly in the manage runs. Hence the change in naming in the actual class and DB
        /// </summary>
        public ForceOverUnder ForceUnderOver { get; set; }
        public string Type { get; set; }
        public CampaignDeliveryType CampaignType { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
