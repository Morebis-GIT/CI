using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class SpotPlacementService
    {
        internal static void FlagSpotAsNotUsed(Spot spot, ICollection<Guid> spotUidList)
        {
            spotUidList.AddDistinct(spot.Uid);
        }

        internal static void FlagSpotAsUsed(Spot spot, ICollection<Guid> spotUidList)
        {
            spotUidList.AddDistinct(spot.Uid);
        }

        // Mark all spots as no placement was attempted. Will reduce the list as
        // we attempt, generate Smooth failures at the end for any where no
        // attempt was made to place it (e.g. no breaks or programmes).
        public static void MarkSpotsAsNoPlacementAttempted(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos
            )
        {
            MarkSpotsNoPlaceAttemptAs(spots, spotInfos, true);
        }

        public static void MarkSpotsAsPlacementWasAttempted(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos
            )
        {
            MarkSpotsNoPlaceAttemptAs(spots, spotInfos, false);
        }

        private static void MarkSpotsNoPlaceAttemptAs(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            bool noPlaceAttempt
            )
        {
            if (spots is null)
            {
                return;
            }

            if (spotInfos is null)
            {
                return;
            }

            foreach (Guid spotUid in spots.Select(s => s.Uid))
            {
                if (!spotInfos.ContainsKey(spotUid))
                {
                    continue;
                }

                spotInfos[spotUid].NoPlaceAttempt = noPlaceAttempt;
            }
        }

        public static void RemoveSpotFromBreak(
            SmoothBreak smoothBreak,
            Spot spot,
            ICollection<Guid> spotIdsUsed,
            SponsorshipRestrictionService sponsorshipRestrictionService
            )
        {
            smoothBreak.RemoveSpot(spot);

            if (spotIdsUsed.Contains(spot.Uid))
            {
                _ = spotIdsUsed.Remove(spot.Uid);
            }

            sponsorshipRestrictionService.TriggerRecalculationOfAllowedRestrictionLimits(
                SpotAction.RemoveSpot,
                spot,
                smoothBreak.TheBreak
                );
        }

        /// <summary>
        /// <para> Adds spots to break. If multiple spots are being
        /// added then they're typically related. E.g. TOP & TAIL.
        /// </para>
        /// <para> The break sequence will be updated later when the break has
        /// been filled, will be updated to start from 1. For specific positions
        /// (FIB, SIB etc) then the break sequence is assigned as a large positive
        /// or negative number and everything else is inserted between.
        /// </para>
        /// </summary>
        public static List<SmoothSpot> AddSpotsToBreak(
            SmoothBreak smoothBreak,
            int smoothPassSequence,
            int smoothPassIterationSequence,
            IReadOnlyCollection<Spot> spots,
            SpotPositionRules passRequestedPositionInBreakRules,
            bool canMoveSpotToOtherBreak,
            ICollection<Guid> spotIdsUsed,
            string bestBreakFactorGroupName,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            SponsorshipRestrictionService sponsorshipRestrictionService
            )
        {
            var smoothSpots = new List<SmoothSpot>();

            foreach (Spot spot in spots)
            {
                IReadOnlyDictionary<string, bool> hasSpotPositions = smoothBreak.GetSpotPositions();
                int breakSeq = SpotPositioning.GetBreakSequenceNumber(
                    smoothBreak,
                    passRequestedPositionInBreakRules,
                    spot,
                    hasSpotPositions
                );

                // Default break sequence to middle of break
                if (breakSeq == 0)
                {
                    breakSeq = SpotPositioning.GetBreakSeqForMiddleOfBreak(smoothBreak);
                }

                smoothSpots.Add(
                    smoothBreak.AddSpot(
                        spot,
                        smoothPassSequence,
                        smoothPassIterationSequence,
                        breakSeq,
                        currentSpot: true,
                        canMoveSpotToOtherBreak,
                        bestBreakFactorGroupName,
                        spotInfos[spot.Uid].ExternalBreakRefAtRunStart
                        )
                    );

                sponsorshipRestrictionService.TriggerRecalculationOfAllowedRestrictionLimits(
                    SpotAction.AddSpot,
                    spot,
                    smoothBreak.TheBreak
                    );

                FlagSpotAsUsed(spot, spotIdsUsed);
            }

            return smoothSpots;
        }

        /// <summary>
        /// Add booked spots to break to stop overbooking
        /// </summary>
        public static List<SmoothSpot> AddBookedSpotsToBreak(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ISet<Guid> spotIdsUsed,
            SponsorshipRestrictionService sponsorshipRestrictionService
            )
        {
            if (smoothBreak is null)
            {
                throw new ArgumentNullException(nameof(smoothBreak));
            }

            if (spots is null)
            {
                throw new ArgumentNullException(nameof(spots));
            }

            if (spotInfos is null)
            {
                throw new ArgumentNullException(nameof(spotInfos));
            }

            if (spotIdsUsed is null)
            {
                throw new ArgumentNullException(nameof(spotIdsUsed));
            }

            var smoothSpots = new List<SmoothSpot>();

            foreach (var spot in spots)
            {
                int breakSeq;

                if (spot.IsMultipartSpot)
                {
                    breakSeq = SpotPositioning.GetBreakSeqFromMultipartSpotPosition(
                        spot.MultipartSpot,
                        spot.MultipartSpotPosition);
                }
                else
                {
                    breakSeq = SpotPositioning.GetBreakSeqFromRequestedPositionInBreakOrActualPositionInBreak(
                        spot.RequestedPositioninBreak,
                        spot.ActualPositioninBreak);
                }

                smoothSpots.Add(
                    smoothBreak.AddSpot(
                        spot,
                        smoothPassSequence: 0,
                        smoothPassIterationSequence: 0,
                        breakSeq,
                        currentSpot: false,
                        canMoveSpotToOtherBreak: false,
                        bestBreakFactorGroupName: null,
                        spotInfos[spot.Uid].ExternalBreakRefAtRunStart)
                    );

                sponsorshipRestrictionService.TriggerRecalculationOfAllowedRestrictionLimits(
                    SpotAction.AddSpot,
                    spot,
                    smoothBreak.TheBreak
                    );

                FlagSpotAsUsed(spot, spotIdsUsed);
            }

            return smoothSpots;
        }
    }
}
