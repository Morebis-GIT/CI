using System;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    public class General : ICloneable
    {
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
