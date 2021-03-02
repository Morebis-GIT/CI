using System;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Details of a spot within Smooth. Contains the Spot plus Smooth specific properties.
    /// </summary>
    public struct SmoothSpot
        : ICloneable
    {
        /// <summary>
        /// Actions for a Smooth spot
        /// </summary>
        public enum SmoothSpotActions
        {
            PlaceSpotInBreak = 0,
            RemoveSpotFromBreak = 1,
            ValidateAddSpotToBreak = 2
        }

        public SmoothSpot(Spot spot, int smoothPassSequence, int smoothPassIterationSequence)
        {
            Spot = spot;
            SmoothPassSequence = smoothPassSequence;
            SmoothPassIterationSequence = smoothPassIterationSequence;

            BreakSequence = default;
            BestBreakFactorGroupName = default;
            IsCurrent = default;
            CanMoveToOtherBreak = default;
            BreakPositionMovedFrom = default;
            ExternalBreakRefAtStart = default;
        }

        public object Clone()
        {
            Spot spotClone = Spot is null
                ? null
                : (Spot)Spot.Clone();

            return new SmoothSpot(spotClone, SmoothPassSequence, SmoothPassIterationSequence)
            {
                BestBreakFactorGroupName = BestBreakFactorGroupName,
                BreakSequence = BreakSequence,
                BreakPositionMovedFrom = BreakPositionMovedFrom,
                CanMoveToOtherBreak = CanMoveToOtherBreak,
                IsCurrent = IsCurrent,
                ExternalBreakRefAtStart = ExternalBreakRefAtStart
            };
        }

        public Spot Spot { get; }

        /// <summary>
        /// Break sequence (Internal)
        /// </summary>
        public int BreakSequence { get; set; }

        /// <summary>
        /// Pass on which spot was assigned to break
        /// </summary>
        public int SmoothPassSequence { get; }

        public int SmoothPassIterationSequence { get; }

        public string BestBreakFactorGroupName { get; set; }

        /// <summary>
        /// Whether spot has been currently smoothed in this run
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Whether spot can be moved to another break after initial placing.
        /// For example, previously booked spots cannot be moved.
        /// </summary>
        public bool CanMoveToOtherBreak { get; set; }

        /// <summary>
        /// For moved spots then the break that it was moved from. For diagnostics
        /// </summary>
        public int BreakPositionMovedFrom { get; set; }

        /// <summary>
        /// ExternalBreakRef for spot at start
        /// </summary>
        public string ExternalBreakRefAtStart { get; set; }
    }
}
