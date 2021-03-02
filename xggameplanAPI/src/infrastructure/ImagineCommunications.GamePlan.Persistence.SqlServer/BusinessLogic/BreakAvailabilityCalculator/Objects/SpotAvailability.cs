using System;
using ImagineCommunications.GamePlan.Domain.Spots;
using NodaTime;
using xggameplan.core.RunManagement.BreakAvailabilityCalculator;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Objects
{
    public class SpotAvailability : ISpotForBreakAvailCalculation
    {
        public string ExternalBreakNo { get; set; }
        public bool IsBooked => SpotHelper.IsBooked(ExternalBreakNo);
        public string SalesArea { get; set; }
        public Duration SpotLength { get; set; }
        public DateTime StartDateTime { get; set; }
        public bool ClientPicked { get; set; }
    }
}
