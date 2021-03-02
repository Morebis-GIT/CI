using System;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using NodaTime;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public sealed class ProgrammeSubsetForBreakAvailCalculation : IProgrammeForBreakAvailCalculation
    {
        public ProgrammeSubsetForBreakAvailCalculation(Programme programme)
        {
            Duration = programme.Duration;
            ExternalReference = programme.ExternalReference;
            ProgrammeId = programme.Id;
            SalesArea = programme.SalesArea;
            StartDateTime = programme.StartDateTime;
        }

        private DateTime _endDateTime;

        public Duration Duration { get; }
        public string ExternalReference { get; }
        public Guid ProgrammeId { get; }
        public string SalesArea { get; }
        public DateTime StartDateTime { get; }
    }
}
