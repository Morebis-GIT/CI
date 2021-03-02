using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using xggameplan.common;
using xggameplan.Common;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class SpotPlacementService
    {
        private static readonly SpotPositioning _spotPositioning = new SpotPositioning();

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
            MarkSpotsPlaceAttemptAs(spots, spotInfos, true);
        }

        public static void MarkSpotsAsPlacementWasAttempted(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos
            )
        {
            MarkSpotsPlaceAttemptAs(spots, spotInfos, false);
        }

        private static void MarkSpotsPlaceAttemptAs(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            bool value
            )
        {
            foreach (Guid spotUid in spots.Select(s => s.Uid))
            {
                spotInfos[spotUid].NoPlaceAttempt = value;
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

        /// <summary> <para> Adds spots to break. If multiple spots are being
        /// added then they're typically related. E.g. TOP & TAIL. </para>
        /// <para> The break sequence will be updated later when the break has
        /// been filled, will be updated to start from 1. For specific positions
        /// (FIB, SIB etc) then the break sequence is assigned as a large +'ve
        /// or -'ve number and everything else is inserted between. </para> </summary>
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
                int breakSeq = 0;

                IReadOnlyDictionary<string, bool> hasSpotPositions = smoothBreak.GetSpotPositions();

                switch (passRequestedPositionInBreakRules)
                {
                    case SpotPositionRules.Exact:
                        if (spot.IsMultipartSpot)
                        {
                            breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                        }
                        else
                        {
                            breakSeq = String.IsNullOrEmpty(spot.RequestedPositioninBreak) ?
                                _spotPositioning.GetBreakSeqForMiddleOfBreak(smoothBreak) :
                                _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(spot.RequestedPositioninBreak);
                        }

                        break;

                    case SpotPositionRules.Near:
                        if (String.IsNullOrEmpty(spot.MultipartSpot))
                        {
                            switch (spot.RequestedPositioninBreak)
                            {
                                case PositionInBreakRequests.TrueFirst:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    string[] spotPositions1 = hasSpotPositions["1ST_START"] || hasSpotPositions["TT|TOP"] || hasSpotPositions["SB|TOP"]
                                        ? new[] { "2ND_START", "3RD_START" }
                                        : new[] { "1ST_START", "2ND_START", "3RD_START" };

                                    foreach (string spotPosition in spotPositions1)
                                    {
                                        if (!hasSpotPositions[spotPosition])
                                        {
                                            string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(spotPosition, PositionInBreakRequests.TrueFirst, PositionInBreakRequests.TrueLast);
                                            breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);

                                            break;
                                        }
                                    }

                                    break;

                                case PositionInBreakRequests.First:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    string[] spotPositions2 = hasSpotPositions["1ST_START"] || hasSpotPositions["TT|TOP"] || hasSpotPositions["SB|TOP"]
                                        ? new[] { "2ND_START", "3RD_START" }
                                        : new[] { "1ST_START", "2ND_START", "3RD_START" };

                                    foreach (string spotPosition in spotPositions2)
                                    {
                                        if (!hasSpotPositions[spotPosition])
                                        {
                                            string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(spotPosition, PositionInBreakRequests.First, PositionInBreakRequests.Last);
                                            breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);
                                            break;
                                        }
                                    }

                                    break;

                                case PositionInBreakRequests.SecondFromStart:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    string[] spotPositions3 = hasSpotPositions["2ND_START"]
                                        ? new[] { "3RD_START" }
                                        : new[] { "2ND_START", "3RD_START" };

                                    foreach (string spotPosition in spotPositions3)
                                    {
                                        if (!hasSpotPositions[spotPosition])
                                        {
                                            string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(spotPosition, PositionInBreakRequests.First, PositionInBreakRequests.Last);
                                            breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);
                                            break;
                                        }
                                    }

                                    break;

                                case PositionInBreakRequests.SecondFromLast:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    foreach (string spotPosition in new[] { "2ND_LAST", "3RD_LAST" })
                                    {
                                        if (!hasSpotPositions[spotPosition])
                                        {
                                            string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(spotPosition, PositionInBreakRequests.First, PositionInBreakRequests.Last);
                                            breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);

                                            break;
                                        }
                                    }

                                    break;

                                case PositionInBreakRequests.ThirdFromStart:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    string[] spotPositions5 = hasSpotPositions["3RD_START"]
                                        ? new[] { "2ND_START" }
                                        : new[] { "3RD_START", "2ND_START" };

                                    foreach (string spotPosition in spotPositions5)
                                    {
                                        if (hasSpotPositions[spotPosition])
                                        {
                                            continue;
                                        }

                                        string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(
                                            spotPosition,
                                            PositionInBreakRequests.First,
                                            PositionInBreakRequests.Last);

                                        breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);

                                        break;
                                    }

                                    break;

                                case PositionInBreakRequests.ThirdFromLast:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    foreach (string spotPosition in new[] { "3RD_LAST", "2ND_LAST" })
                                    {
                                        if (!hasSpotPositions[spotPosition])
                                        {
                                            string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(spotPosition, PositionInBreakRequests.First, PositionInBreakRequests.Last);
                                            breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);

                                            break;
                                        }
                                    }

                                    break;

                                case PositionInBreakRequests.TrueLast:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    string[] spotPositions7 = hasSpotPositions["LAST"] || hasSpotPositions["TT|TAIL"] || hasSpotPositions["SB|TAIL"]
                                        ? new[] { "2ND_LAST", "3RD_LAST" }
                                        : new[] { "LAST", "2ND_LAST", "3RD_LAST" };

                                    foreach (string spotPosition in spotPositions7)
                                    {
                                        if (!hasSpotPositions[spotPosition])
                                        {
                                            string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(
                                                spotPosition,
                                                PositionInBreakRequests.TrueFirst,
                                                PositionInBreakRequests.TrueLast);

                                            breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);

                                            break;
                                        }
                                    }

                                    break;

                                case PositionInBreakRequests.Last:
                                    // Check existing PIB requests in priority
                                    // order from most to least desired
                                    string[] spotPositions8 = hasSpotPositions["LAST"] || hasSpotPositions["TT|TAIL"] || hasSpotPositions["SB|TAIL"]
                                        ? new[] { "2ND_LAST", "3RD_LAST" }
                                        : new[] { "LAST", "2ND_LAST", "3RD_LAST" };

                                    foreach (string spotPosition in spotPositions8)
                                    {
                                        if (hasSpotPositions[spotPosition])
                                        {
                                            continue;
                                        }

                                        string requestedPositionInBreak = _spotPositioning.GetRequestedPositionInBreakFromSpotPosition(
                                            spotPosition,
                                            PositionInBreakRequests.First,
                                            PositionInBreakRequests.Last);

                                        breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreak(requestedPositionInBreak);

                                        break;
                                    }

                                    break;

                                default:
                                    // No PIB request
                                    breakSeq = _spotPositioning.GetBreakSeqForMiddleOfBreak(smoothBreak);
                                    break;
                            }
                        }
                        else
                        {
                            switch (spot.MultipartSpot)
                            {
                                case MultipartSpotTypes.TopTail:
                                    switch (spot.MultipartSpotPosition)
                                    {
                                        case MultipartSpotPositions.TopTail_Top:
                                            if (!hasSpotPositions["1ST_START"])
                                            {
                                                breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                                            }

                                            break;

                                        case MultipartSpotPositions.TopTail_Tail:
                                            if (!hasSpotPositions["LAST"])
                                            {
                                                breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                                            }

                                            break;
                                    }

                                    break;

                                case MultipartSpotTypes.SameBreak:
                                    switch (spot.MultipartSpotPosition)
                                    {
                                        case MultipartSpotPositions.SameBreak_Top:
                                            if (!hasSpotPositions["1ST_START"])
                                            {
                                                breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                                            }

                                            break;

                                        case MultipartSpotPositions.SameBreak_Mid:
                                            breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);

                                            break;

                                        case MultipartSpotPositions.SameBreak_Tail:
                                            if (!hasSpotPositions["LAST"])
                                            {
                                                breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                                            }

                                            break;

                                        case MultipartSpotPositions.SameBreak_Any:
                                            breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(spot.MultipartSpot, spot.MultipartSpotPosition);
                                            break;
                                    }

                                    break;
                            }

                            // Default to middle of break
                            if (breakSeq == 0)
                            {
                                breakSeq = _spotPositioning.GetBreakSeqForMiddleOfBreak(smoothBreak);
                            }
                        }

                        break;

                    case SpotPositionRules.Anywhere:
                        breakSeq = _spotPositioning.GetBreakSeqForMiddleOfBreak(smoothBreak);
                        break;
                }

                // Default break sequence to middle of break
                if (breakSeq == 0)
                {
                    breakSeq = _spotPositioning.GetBreakSeqForMiddleOfBreak(smoothBreak);
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
                    breakSeq = _spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                        spot.MultipartSpot,
                        spot.MultipartSpotPosition);
                }
                else
                {
                    breakSeq = _spotPositioning.GetBreakSeqFromRequestedPositionInBreakOrActualPositionInBreak(
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
