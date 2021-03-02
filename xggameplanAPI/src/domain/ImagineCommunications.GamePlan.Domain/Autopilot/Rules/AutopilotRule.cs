using System;

namespace ImagineCommunications.GamePlan.Domain.Autopilot.Rules
{
    public class AutopilotRule : ICloneable
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int RuleTypeId { get; set; }
        public int FlexibilityLevelId { get; set; }
        public bool Enabled { get; set; }
        public int LoosenBit { get; set; }
        public int LoosenLot { get; set; }
        public int TightenBit { get; set; }
        public int TightenLot { get; set; }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public string UniqueRuleKey => RuleId + "_" + RuleTypeId;

        public static AutopilotRule Create(int flexibilityLevelId, int ruleId, int ruleTypeId, int tightenBit = 0, int loosenBit = 0, int tightenLot = 0, int loosenLot = 0)
        {
            return new AutopilotRule
            {
                Id = 0,
                FlexibilityLevelId = flexibilityLevelId,
                Enabled = true,
                RuleId = ruleId,
                RuleTypeId = ruleTypeId,
                LoosenBit = loosenBit,
                LoosenLot = loosenLot,
                TightenBit = tightenBit,
                TightenLot = tightenLot
            };
        }

        public object Clone() => MemberwiseClone();
    }
}
