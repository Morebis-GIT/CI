using System;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using NodaTime;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public interface ISpotForBreakAvailCalculation
    {
        /// <summary>
        /// Can be null, the value of <see cref="Globals.UnplacedBreakString"/> or the
        /// <see cref="Break.ExternalBreakRef"/>.
        /// </summary>
        string ExternalBreakNo { get; }
        
        bool IsBooked { get; }
        
        string SalesArea { get; }

        Duration SpotLength { get; }

        /// <summary>
        /// Either the start of the break if the spot is placed, or the start of the programme
        /// if the spot is <see cref="Spot.ClientPicked"/> and unplaced.
        /// </summary>
        DateTime StartDateTime { get; }

        bool ClientPicked { get; }
    }
}
