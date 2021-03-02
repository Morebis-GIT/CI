using System;
using NodaTime;
using xggameplan.core.RunManagement.BreakAvailabilityCalculator;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Objects
{
    public class ProgrammeAvailability : IProgrammeForBreakAvailCalculation
    {
        public Guid ProgrammeId { get; set; }
        public DateTime StartDateTime { get; set; }
        public Duration Duration { get; set; }
        public string ExternalReference { get; set; }
        public string SalesArea { get; set; }
    }
}
