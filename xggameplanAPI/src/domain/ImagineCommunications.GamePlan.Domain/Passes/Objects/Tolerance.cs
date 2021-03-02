using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    public class Tolerance : ICloneable
    {
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public int Under { get; set; }
        public int Over { get; set; }
        public bool Ignore { get; set; }
        public bool BookTargetArea { get; set; }
        public ForceOverUnder ForceOverUnder { get; set; }
        public string Type { get; set; }
        public CampaignDeliveryType CampaignType { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
