using System;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes
{
    public class RuleType : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool AllowedForAutopilot { get; set; }

        /// <summary>
        /// Custom rule types do not contain rules
        /// All rules are defined by user at the Pass level
        /// </summary>
        public bool IsCustom { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
