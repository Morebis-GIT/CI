using System;

namespace ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels
{
    public class FlexibilityLevel : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortIndex { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
