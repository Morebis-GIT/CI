using System;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Breaks
{
    public interface IBreakAvailability
    {
        Guid Id { get; }

        string ExternalBreakRef { get; }

        DateTime ScheduledDate { get; set; }

        Duration Duration { get; }

        Duration Avail { get; set; }

        Duration OptimizerAvail { get; set; }
    }
}
