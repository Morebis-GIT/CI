using System;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using NodaTime;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public sealed class SpotSubsetForBreakAvailCalculation : ISpotForBreakAvailCalculation
    {
        public SpotSubsetForBreakAvailCalculation(Spot spot)
        {
            ClientPicked = spot.ClientPicked;
            ExternalBreakNo = spot.ExternalBreakNo;
            IsBooked = spot.IsBooked();
            SalesArea = spot.SalesArea;
            SpotLength = spot.SpotLength;
            StartDateTime = spot.StartDateTime;
        }

        /// <summary>
        /// Can be null, the value of <see cref="Globals.UnplacedBreakString"/> or the
        /// <see cref="Break.ExternalBreakRef"/>.
        /// </summary>
        public string ExternalBreakNo { get; }
        public bool IsBooked { get; }
        public string SalesArea { get; }

        public Duration SpotLength { get; }

        /// <summary>
        /// Either the start of the break if the spot is placed, or the start of the programme
        /// if the spot is <see cref="Spot.ClientPicked"/> and unplaced.
        /// </summary>
        public DateTime StartDateTime { get; }

        public bool ClientPicked { get; }
    }
}
