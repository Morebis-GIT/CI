using System;

namespace ImagineCommunications.GamePlan.Domain.Autopilot.Settings
{
    public class AutopilotSettings : ICloneable
    {
        public int Id { get; set; }
        public int DefaultFlexibilityLevelId { get; set; }
        public int ScenariosToGenerate { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
