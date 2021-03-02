using System;
using ImagineCommunications.GamePlan.Domain.Breaks;
using NodaTime;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Objects
{
    public class BreakAvailability : IBreakAvailability
    {
        public Guid Id { get; set; }
        public string ExternalBreakRef { get; set; }
        public DateTime ScheduledDate { get; set; }
        public Duration Duration { get; set; }
        public Duration Avail { get; set; }
        public Duration OptimizerAvail { get; set; }
    }
}
