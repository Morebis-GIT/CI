using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.common;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Reads the SmoothConfiguration document during Smooth processing
    /// </summary>
    public class SmoothConfigurationReader : ISmoothConfiguration
    {
        public SmoothConfigurationReader(SmoothConfiguration smoothConfiguration)
            => SmoothConfiguration = smoothConfiguration;

        public SmoothConfiguration SmoothConfiguration { get; }

        public string Version => SmoothConfiguration.Version;

        public bool RestrictionCheckEnabled => SmoothConfiguration.RestrictionCheckEnabled;

        public bool ClashExceptionCheckEnabled => SmoothConfiguration.ClashExceptionCheckEnabled;

        public List<string> ExternalCampaignRefsToExclude => SmoothConfiguration.ExternalCampaignRefsToExclude;

        public bool RecommendationsForExcludedCampaigns => SmoothConfiguration.RecommendationsForExcludedCampaigns;

        public bool SmoothFailuresForExcludedCampaigns => SmoothConfiguration.SmoothFailuresForExcludedCampaigns;

        /// <summary>Returns a list of Smooth passes in sequence order.</summary>
        public IImmutableList<SmoothPass> SortedSmoothPasses
        {
            get
            {
                var smoothPasses = new List<SmoothPass>(SmoothConfiguration.Passes);

                smoothPasses.Insert(0, new SmoothPassBooked { Sequence = 0 });

                return smoothPasses
                    .OrderBy(p => p.Sequence)
                    .ToImmutableList();
            }
        }

        public List<SmoothPassDefaultIteration> GetSmoothPassDefaultIterations(
            Spot spot,
            SmoothPass smoothPass)
        {
            return SmoothConfiguration.IterationRecords
                .Where(i => IsIterationValidForSpot(i, spot, smoothPass))
                .Select(i => i.PassDefaultIteration)
                .OrderBy(i => i.Sequence)
                .ToList();
        }

        public List<SmoothPassUnplacedIteration> GetSmoothPassUnplacedIterations(
            Spot spot,
            SmoothPass smoothPass)
        {
            return SmoothConfiguration.IterationRecords
                .Where(i => IsIterationValidForSpot(i, spot, smoothPass))
                .Select(i => i.PassUnplacedIteration)
                .OrderBy(i => i.Sequence)
                .ToList();
        }

        /// <summary>
        /// Returns whether the iteration is valid for the spot
        /// </summary>
        /// <param name="record"></param>
        /// <param name="spot"></param>
        /// <param name="smoothPass"></param>
        /// <returns></returns>
        private bool IsIterationValidForSpot(
            SmoothPassIterationRecord record,
            Spot spot,
            SmoothPass smoothPass)
        {
            bool valid = true;

            if (record.PassSequences?.Count > 0)
            {
                valid = record.PassSequences.Contains(smoothPass.Sequence);
            }

            if (!valid)
            {
                return false;
            }

            if (record.SpotsCriteria != null)
            {
                valid = IsSpotsMeetSpotCriteria(
                    new List<Spot>() { spot },
                    record.SpotsCriteria);
            }

            return valid;
        }

        public IReadOnlyCollection<BestBreakFactorGroup> GetBestBreakFactorGroupsData(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothPass smoothPass,
            SmoothPassDefaultIteration smoothPassIteration
            )
        {
            List<BestBreakFactorGroupRecord> records = SmoothConfiguration.BestBreakFactorGroupRecords;

            var bestBreakFactorGroups = new List<BestBreakFactorGroup>();
            foreach (var record in records)
            {
                if (IsGroupValidForSpots(record, spots, smoothPass))
                {
                    bestBreakFactorGroups.Add(record.BestBreakFactorGroup);
                }
            }

            return bestBreakFactorGroups
                .OrderBy(g => g.Sequence)
                .ToList();
        }

        /// <summary>
        /// Returns whether the spots meet the criteria
        /// </summary>
        /// <param name="spots"></param>
        /// <param name="spotsCriteria"></param>
        /// <returns></returns>
        private bool IsSpotsMeetSpotCriteria(
            IEnumerable<Spot> spots,
            SpotsCriteria spotsCriteria)
        {
            const bool IsValid = true;
            const bool IsNotValid = !IsValid;

            // Return valid if no criteria specified
            if (spotsCriteria is null
                || (spotsCriteria.HasBreakRequest is null
                    && spotsCriteria.HasFIBORLIBRequests is null
                    && spotsCriteria.HasSponsoredSpots is null)
                )
            {
                return IsValid;
            }

            // Check spots
            bool hasSponsoredSpots = false;
            bool hasFirstInBreakRequestSpots = false;
            bool hasLastInBreakRequestSpots = false;
            bool hasBreakRequest = false;
            List<string> firstInBreakRequests = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(PositionInBreakRequests.First);
            List<string> lastInBreakRequests = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(PositionInBreakRequests.Last);

            foreach (var spot in spots)
            {
                if (spot.Sponsored)
                {
                    hasSponsoredSpots = true;
                }
                if (!String.IsNullOrEmpty(spot.BreakRequest))
                {
                    hasBreakRequest = true;
                }
                if (!String.IsNullOrEmpty(spot.RequestedPositioninBreak) && firstInBreakRequests.Contains(spot.RequestedPositioninBreak))
                {
                    hasFirstInBreakRequestSpots = true;
                }
                if (!String.IsNullOrEmpty(spot.RequestedPositioninBreak) && lastInBreakRequests.Contains(spot.RequestedPositioninBreak))
                {
                    hasLastInBreakRequestSpots = true;
                }
            }

            if (spotsCriteria.HasBreakRequest != null)
            {
                if (spotsCriteria.HasBreakRequest == true && !hasBreakRequest)
                {
                    return IsNotValid;
                }

                if (spotsCriteria.HasBreakRequest == false && hasBreakRequest)
                {
                    return IsNotValid;
                }
            }

            if (spotsCriteria.HasSponsoredSpots != null)
            {
                if (spotsCriteria.HasSponsoredSpots == true && !hasSponsoredSpots)
                {
                    return IsNotValid;
                }

                if (spotsCriteria.HasSponsoredSpots == false && hasSponsoredSpots)
                {
                    return IsNotValid;
                }
            }

            if (spotsCriteria.HasFIBORLIBRequests != null)
            {
                if (spotsCriteria.HasFIBORLIBRequests == true && !hasFirstInBreakRequestSpots && !hasLastInBreakRequestSpots)
                {
                    return IsNotValid;
                }

                if (spotsCriteria.HasFIBORLIBRequests == false && (hasFirstInBreakRequestSpots || hasLastInBreakRequestSpots))
                {
                    return IsNotValid;
                }
            }

            return IsValid;
        }

        /// <summary>
        /// Returns whether the BestBreakFactorGroup is valid for the spots
        /// </summary>
        /// <param name="record"></param>
        /// <param name="spots"></param>
        /// <param name="smoothPass"></param>
        /// <returns></returns>
        private bool IsGroupValidForSpots(
            BestBreakFactorGroupRecord record,
            IEnumerable<Spot> spots,
            SmoothPass smoothPass)
        {
            bool valid = true;

            if (record.PassSequences?.Count > 0)
            {
                valid = record.PassSequences.Contains(smoothPass.Sequence);
            }

            if (!valid)
            {
                return false;
            }

            if (record.SpotsCriteria != null)
            {
                valid = IsSpotsMeetSpotCriteria(spots, record.SpotsCriteria);
            }

            return valid;
        }

        public List<SmoothSpot> GetSpotsThatCanBeMoved(
            Spot spotToPlace,
            SmoothBreak smoothBreak
            )
        {
            var preemptLevel = spotToPlace.Preemptlevel;

            return smoothBreak.SmoothSpots
                .Where(s =>
                    s.Spot.Preemptlevel > preemptLevel
                    && s.CanMoveToOtherBreak
                    && !s.Spot.Sponsored
                    && String.IsNullOrEmpty(s.Spot.MultipartSpot)
                    && String.IsNullOrEmpty(s.Spot.BreakRequest)
                    && String.IsNullOrEmpty(s.Spot.RequestedPositioninBreak)
                    )
                .OrderBy(s => s.Spot.SpotLength.BclCompatibleTicks)
                .ToList();
        }

        public IEnumerable<Spot> SortSpotsToPlace(
            IEnumerable<Spot> spots,
            Programme programme
            )
        {
            var programmeStart = programme.StartDateTime;
            var programmeDuration = programme.Duration;

            return spots
                .OrderBy(s => NonpreemptableFirst(s.Preemptable))
                .ThenBy(s => s.Preemptlevel)
                .ThenBy(s => s.Sponsored ? 0 : 1)
                .ThenBy(s => s.ClientPicked ? 0 : 1)
                .ThenBy(s => String.IsNullOrEmpty(s.BreakRequest) ? 1 : 0)    // Pick up specific break requests before break is filled
                .ThenBy(s => SpotUtilities.HasRestrictedSpotTimes(s.StartDateTime, s.EndDateTime, programmeStart, programmeDuration) ? 0 : 1)
                .ThenBy(s =>
                    !String.IsNullOrEmpty(s.RequestedPositioninBreak) &&
                    _positionInBreakRequestOrder.ContainsKey(s.RequestedPositioninBreak)
                        ? _positionInBreakRequestOrder[s.RequestedPositioninBreak]
                        : String.IsNullOrEmpty(s.RequestedPositioninBreak)
                            ? 99
                            : 98

                )  // So that we process in normal priority order if same preempt level
                .ThenBy(s =>
                    !String.IsNullOrEmpty(s.MultipartSpotPosition) &&
                    _positionOrder.ContainsKey(
                        MultipartSpotTypes.GetSpotTypeAndPositionKey(s.MultipartSpot, s.MultipartSpotPosition)
                    )
                    ? _positionOrder[MultipartSpotTypes.GetSpotTypeAndPositionKey(s.MultipartSpot, s.MultipartSpotPosition)]
                    : 0
                )   // So that TOP appears before TAIL
                .ThenBy(s => s.ExternalSpotRef);                // So that we have a consistent order if repeating this run

            // Local functions
            int NonpreemptableFirst(bool isPreemptable) => isPreemptable ? 1 : 0;
        }

        private static readonly IReadOnlyDictionary<string, short> _positionOrder = new Dictionary<string, short>()
        {
            { String.Empty, 1 },
            { MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.TopTail, MultipartSpotPositions.TopTail_Top), 2 },
            { MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.TopTail, MultipartSpotPositions.TopTail_Tail), 3 },
            { MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Top), 4 },
            { MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Tail), 5 },
            { MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Mid), 6 },
            { MultipartSpotTypes.GetSpotTypeAndPositionKey(MultipartSpotTypes.SameBreak, MultipartSpotPositions.SameBreak_Any), 7 },
        };

        private static readonly IReadOnlyDictionary<string, short> _positionInBreakRequestOrder = new Dictionary<string, short>()
        {
            { String.Empty, 1 },
            { PositionInBreakRequests.TrueFirst, 2 },
            { PositionInBreakRequests.TrueLast, 3 },
            { PositionInBreakRequests.First, 4 },
            { PositionInBreakRequests.Last, 5 },
            { PositionInBreakRequests.SecondFromStart, 6 },
            { PositionInBreakRequests.SecondFromLast, 7 },
            { PositionInBreakRequests.ThirdFromStart, 8 },
            { PositionInBreakRequests.ThirdFromLast, 9 },
        };

        /// <summary>
        /// Diagnostic configuration
        /// </summary>
        public SmoothDiagnosticConfiguration DiagnosticConfiguration
            => SmoothConfiguration.DiagnosticConfiguration;
    }
}
