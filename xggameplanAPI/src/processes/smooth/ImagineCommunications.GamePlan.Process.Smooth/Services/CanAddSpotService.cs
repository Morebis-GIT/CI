using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// A service that can determine whether a spot can be added to a break.
    /// </summary>
    public class CanAddSpotService
        : ICanAddSpot
    {
        // A list of break positions near to break one's position.
        private static readonly int[] _positionsNearToBreakOne = new int[] { 2, 3 };

        private readonly SmoothBreak _smoothBreak;

        public CanAddSpotService(SmoothBreak smoothBreak) => _smoothBreak = smoothBreak;

        public bool CanAddMultipartSpotAtRequestedPosition(
            string multipartSpot,
            string multipartSpotPosition,
            SpotPositionRules requestedPositionInBreakRule)
        {
            return _smoothBreak.CanAddMultipartSpotAtRequestedPosition(
                multipartSpot,
                multipartSpotPosition,
                requestedPositionInBreakRule
                );
        }

        public bool CanAddSpotAtBreakPosition(
            int spotRequestedBreakPosition,
            SpotPositionRules breakPositionRules)
        {
            int breakPosition = _smoothBreak.Position;

            switch (breakPositionRules)
            {
                case SpotPositionRules.Exact:
                    return spotRequestedBreakPosition == breakPosition;

                case SpotPositionRules.Near:
                    int[] nearPositions = NearBreakPositions(breakPosition);
                    return spotRequestedBreakPosition ==
                        breakPosition ||
                        nearPositions.Contains(spotRequestedBreakPosition);

                case SpotPositionRules.Anywhere:
                default:
                    return true;
            }

            //----------------
            // Local functions

            // Returns list of break positions that are near to break's position
            static int[] NearBreakPositions(int breakPosition) =>
                breakPosition == 1
                    ? _positionsNearToBreakOne
                    : (new int[] { breakPosition - 1, breakPosition + 1 });
        }

        public bool CanAddSpotAtRequestedPosition(
            string requestedPositionInBreak,
            SpotPositionRules requestedPositionInBreakRule)
        {
            return _smoothBreak.CanAddSpotAtRequestedPosition(requestedPositionInBreak, requestedPositionInBreakRule);
        }

        public bool CanAddSpotsWithCampaignClashRule(
            IReadOnlyCollection<Spot> spots,
            bool respectCampaignClash,
            ICampaignClashChecker campaignClashChecker)
        {
            return _smoothBreak.CanAddSpotsWithCampaignClashRule(
                spots,
                respectCampaignClash,
                campaignClashChecker
                );
        }

        public bool CanAddSpotsWithProductClashRule(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IImmutableDictionary<string, Clash> clashesByExternalRef,
            ProductClashRules productClashRule,
            bool respectClashExceptions,
            IProductClashChecker productClashChecker,
            ClashExceptionChecker clashExceptionChecker,
            IClashExposureCountService clashExposureCount)
        {
            return _smoothBreak.CanAddSpotsWithProductClashRule(
                spots,
                spotInfos,
                clashesByExternalRef,
                productClashRule,
                respectClashExceptions,
                productClashChecker,
                clashExceptionChecker,
                clashExposureCount
                );
        }

        public bool CanAddSpotsWithRestrictionRule(
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Spot> spots,
            bool respectRestrictions,
            RestrictionChecker restrictionChecker,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            return _smoothBreak.CanAddSpotsWithRestrictionRule(
                smoothProg,
                spots,
                respectRestrictions,
                restrictionChecker,
                breaksBeingSmoothed,
                scheduleProgrammes
                );
        }

        public bool CanAddSpotWithBreakRequest(
            string breakRequest,
            int progBreakCount,
            SpotPositionRules breakPositionRules)
        {
            if (String.IsNullOrEmpty(breakRequest))
            {
                return true;
            }

            if (breakRequest == _smoothBreak.TheBreak.ExternalBreakRef)
            {
                return true;
            }

            if (Int32.TryParse(breakRequest, out int requestedBreakPosition))
            {
                requestedBreakPosition = GetActualBreakPositionFromRelativePosition(requestedBreakPosition, progBreakCount);

                return CanAddSpotAtBreakPosition(requestedBreakPosition, breakPositionRules);
            }

            return false;
        }

        /// <summary>
        /// Determines whether a Spot can be placed in a Break by comparing break types.
        /// </summary>
        /// <param name="spotBreakType">The break type of the Spot.</param>
        /// <returns>
        ///   Returns <c>true</c> if the spot can be added to the break; otherwise, <c>false</c>.
        /// </returns>
        public bool CanAddSpotWithBreakType(string spotBreakType) =>
            String.IsNullOrWhiteSpace(spotBreakType) ||
                _smoothBreak.TheBreak.BreakType.Equals(spotBreakType, StringComparison.OrdinalIgnoreCase);

        public bool CanAddSpotWithTime(
            DateTime spotStartDateTime,
            DateTime spotEndDateTime)
        {
            if (spotEndDateTime >= spotStartDateTime)
            {
                return DateHelper.IsRangesOverlap(
                    _smoothBreak.TheBreak.ScheduledDate,
                    _smoothBreak.TheBreak.ScheduledDate.Add(_smoothBreak.TheBreak.Duration.ToTimeSpan()),
                    spotStartDateTime,
                    spotEndDateTime);
            }

            return spotStartDateTime <= _smoothBreak.TheBreak.ScheduledDate;
        }

        /// <inheritdoc/>
        public bool IsSpotEligibleWhenIgnoringSpotTime(Spot spot) =>
            CanAddSpotWithBreakType(spot.BreakType);

        /// <inheritdoc/>
        public bool IsSpotEligibleWhenRespectingSpotTime(Spot spot) =>
            CanAddSpotWithBreakType(spot.BreakType) &&
            CanAddSpotWithTime(spot.StartDateTime, spot.EndDateTime);

        /// <summary>
        /// Returns actual break position, removes any relative positions.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="progBreakCount"></param>
        /// <returns></returns>
        /// <remarks>Method might not be in the best location. Consider moving.</remarks>
        public static int GetActualBreakPositionFromRelativePosition(int position, int progBreakCount) =>
            position switch
            {
                97 => progBreakCount - 2,
                98 => progBreakCount - 1,
                99 => progBreakCount,
                _ => position,
            };
    }
}
