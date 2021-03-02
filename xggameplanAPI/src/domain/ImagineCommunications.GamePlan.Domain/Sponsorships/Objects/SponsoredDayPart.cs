using System;

namespace ImagineCommunications.GamePlan.Domain.Sponsorships.Objects
{
    public class SponsoredDayPart : ICloneable
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string[] DaysOfWeek { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
