using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using xggameplan.common;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// A service that can determine whether a spot can be added to a break.
    /// </summary>
    public class CanAddSpotService
    {
        // A list of break positions near to break one's position.
        private static readonly int[] _positionsNearToBreakOne = new int[] { 2, 3 };

        protected readonly SmoothBreak _smoothBreak;

        protected CanAddSpotService(SmoothBreak smoothBreak) => _smoothBreak = smoothBreak;

        public static CanAddSpotService Factory(SmoothBreak smoothBreak) =>
            ContainerReferenceService.IsContainerReference(smoothBreak.TheBreak.ExternalBreakRef)
                ? new CanAddSpotToContainerService(smoothBreak)
                : new CanAddSpotService(smoothBreak);

        /// <summary>
        /// Whether it is possible to add multipart spot at requested position
        /// </summary>
        public bool CanAddMultipartSpotAtRequestedPosition(
        string multipartSpot,
            string multipartSpotPosition,
            SpotPositionRules requestedPositionInBreakRule)
        {
            IReadOnlyDictionary<string, bool> hasSpotPositions = _smoothBreak.GetSpotPositions();

            switch (requestedPositionInBreakRule)
            {
                case SpotPositionRules.Exact:
                    switch (multipartSpot)
                    {
                        case MultipartSpotTypes.TopTail:
                            switch (multipartSpotPosition)
                            {
                                case MultipartSpotPositions.TopTail_Top:
                                    return !hasSpotPositions["TT|TOP"] && !hasSpotPositions["SB|TOP"] && !hasSpotPositions["1ST_START"];

                                case MultipartSpotPositions.TopTail_Tail:
                                    return !hasSpotPositions["TT|TAIL"] && !hasSpotPositions["SB|TAIL"] && !hasSpotPositions["LAST"];
                            }

                            break;

                        case MultipartSpotTypes.SameBreak:
                            switch (multipartSpotPosition)
                            {
                                case MultipartSpotPositions.SameBreak_Top:
                                    return !hasSpotPositions["TT|TOP"] && !hasSpotPositions["SB|TOP"] && !hasSpotPositions["1ST_START"];

                                case MultipartSpotPositions.SameBreak_Mid:
                                    return !hasSpotPositions["SB|MID"];

                                case MultipartSpotPositions.SameBreak_Tail:
                                    return !hasSpotPositions["TT|TAIL"] && !hasSpotPositions["SB|TAIL"] && !hasSpotPositions["LAST"];

                                case MultipartSpotPositions.SameBreak_Any:
                                    return true;
                            }

                            break;
                    }

                    break;

                case SpotPositionRules.Near:
                    if (multipartSpot == MultipartSpotTypes.SameBreak)
                    {
                        // Don't allow near position for Same Break spots
                        return false;
                    }

                    return true;

                case SpotPositionRules.Anywhere:
                    return true;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the spot can be placed in the requested position.
        /// Note: for containers, only position rules for exact and any position
        /// are allowed.
        /// </summary>
        /// <param name="spotRequestedBreakPosition">The spot requested break position.</param>
        /// <param name="breakPosition">The break position.</param>
        /// <param name="breakPositionRules">The break position rules.</param>
        /// <returns>
        ///   <c>true</c> if this spot can be placed at the requested position;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public bool CanAddSpotAtBreakPosition(
            int spotRequestedBreakPosition,
            int breakPosition,
            SpotPositionRules breakPositionRules)
        {
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

        /// <summary>
        /// Whether it is possible to add spot at position in break, governed by
        /// positioning rule.
        /// </summary>
        /// <param name="requestedPositionInBreak"></param>
        /// <param name="requestedPositionInBreakRule"></param>
        public bool CanAddSpotAtRequestedPosition(
            string requestedPositionInBreak,
            SpotPositionRules requestedPositionInBreakRule)
        {
            if (String.IsNullOrEmpty(requestedPositionInBreak))
            {
                // No position requested so we can inserted it
                return true;
            }

            IReadOnlyDictionary<string, bool> hasSpotPositions = _smoothBreak.GetSpotPositions();

            switch (requestedPositionInBreakRule)
            {
                case SpotPositionRules.Exact:
                    switch (requestedPositionInBreak)
                    {
                        case PositionInBreakRequests.TrueFirst:
                        case PositionInBreakRequests.First:
                            return !hasSpotPositions["1ST_START"]
                                && !hasSpotPositions["TT|TOP"]
                                && !hasSpotPositions["SB|TOP"];

                        case PositionInBreakRequests.SecondFromStart:
                            return !hasSpotPositions["2ND_START"];

                        case PositionInBreakRequests.ThirdFromStart:
                            return !hasSpotPositions["3RD_START"];

                        case PositionInBreakRequests.ThirdFromLast:
                            return !hasSpotPositions["3RD_LAST"];

                        case PositionInBreakRequests.SecondFromLast:
                            return !hasSpotPositions["2ND_LAST"];

                        case PositionInBreakRequests.Last:
                        case PositionInBreakRequests.TrueLast:
                            return !hasSpotPositions["LAST"]
                                && !hasSpotPositions["TT|TAIL"]
                                && !hasSpotPositions["SB|TAIL"];

                        default:
                            return false;
                    }

                // For near we can almost always find a position
                case SpotPositionRules.Near:
                    switch (requestedPositionInBreak)
                    {
                        case PositionInBreakRequests.TrueFirst:
                        case PositionInBreakRequests.First:
                            return (!hasSpotPositions["1ST_START"] && !hasSpotPositions["TT|TOP"] && !hasSpotPositions["SB|TOP"])
                                || !hasSpotPositions["2ND_START"]
                                || !hasSpotPositions["3RD_START"];

                        case PositionInBreakRequests.SecondFromStart:
                            return !hasSpotPositions["2ND_START"]
                                || !hasSpotPositions["3RD_START"];

                        case PositionInBreakRequests.ThirdFromStart:
                            return !hasSpotPositions["3RD_START"]
                                || !hasSpotPositions["2ND_START"]
                                || !hasSpotPositions["4TH_START"];

                        case PositionInBreakRequests.ThirdFromLast:
                            return !hasSpotPositions["3RD_LAST"]
                                || !hasSpotPositions["2ND_LAST"];

                        case PositionInBreakRequests.SecondFromLast:
                            return !hasSpotPositions["2ND_LAST"]
                                || !hasSpotPositions["3RD_LAST"];

                        case PositionInBreakRequests.Last:
                        case PositionInBreakRequests.TrueLast:
                            return (!hasSpotPositions["LAST"] && !hasSpotPositions["TT|TAIL"] && !hasSpotPositions["SB|TAIL"])
                                || !hasSpotPositions["2ND_LAST"];

                        default:
                            return false;
                    }

                case SpotPositionRules.Anywhere:
                    return true;
            }

            // No position requested so we can inserted it
            return true;
        }

        /// <summary>
        /// Whether spots can be added to break with campaign clash rule
        /// </summary>
        public bool CanAddSpotsWithCampaignClashRule(
            IReadOnlyCollection<Spot> spots,
            bool respectCampaignClash,
            ICampaignClashChecker campaignClashChecker)
        {
            if (!respectCampaignClash)
            {
                return true;
            }

            var spotsInBreak = _smoothBreak.SmoothSpots.ConvertAll(s => s.Spot);

            foreach (var spot in spots)
            {
                var placeboList = new List<Spot> { spot };

                if (campaignClashChecker
                    .GetCampaignClashesForNewSpots(placeboList, spotsInBreak)
                    .Count > 0
                    )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether spots can be added to break with product clash rule
        /// </summary>
        public bool CanAddSpotsWithProductClashRule(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IImmutableDictionary<string, Clash> clashesByExternalRef,
            ProductClashRules productClashRule,
            bool respectClashExceptions,
            IProductClashChecker productClashChecker,
            IClashExceptionChecker clashExceptionChecker,
            IClashExposureCountService clashExposureCount)
        {
            // Count spots per parent and child clash code
            var countNewSpotsPerChildClashCode = new Dictionary<string, int>();
            var countNewSpotsPerParentClashCode = new Dictionary<string, int>();

            foreach (Guid spotUid in spots.Select(s => s.Uid))
            {
                SpotInfo spotInfo = spotInfos[spotUid];

                if (!String.IsNullOrEmpty(spotInfo.ProductClashCode))
                {
                    if (!countNewSpotsPerChildClashCode.ContainsKey(spotInfo.ProductClashCode))
                    {
                        countNewSpotsPerChildClashCode.Add(spotInfo.ProductClashCode, 0);
                    }

                    countNewSpotsPerChildClashCode[spotInfo.ProductClashCode]++;
                }

                if (!String.IsNullOrEmpty(spotInfo.ParentProductClashCode))
                {
                    if (!countNewSpotsPerParentClashCode.ContainsKey(spotInfo.ParentProductClashCode))
                    {
                        countNewSpotsPerParentClashCode.Add(spotInfo.ParentProductClashCode, 0);
                    }

                    countNewSpotsPerParentClashCode[spotInfo.ParentProductClashCode]++;
                }
            }

            // Check clash exceptions Include overrides any Exclude.
            var spotHasClashExceptionIncludes = new Dictionary<Guid, bool>();
            var spotHasClashExceptionExcludes = new Dictionary<Guid, bool>();

            foreach (Guid spotUid in spots.Select(s => s.Uid))
            {
                spotHasClashExceptionIncludes.Add(spotUid, false);
                spotHasClashExceptionExcludes.Add(spotUid, false);
            }

            if (respectClashExceptions)
            {
                foreach (var spot in spots)
                {
                    List<CheckClashExceptionResult> checkClashExceptionsResults = clashExceptionChecker.CheckClashExceptions(_smoothBreak, spot);
                    spotHasClashExceptionIncludes[spot.Uid] = checkClashExceptionsResults.Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.I);
                    spotHasClashExceptionExcludes[spot.Uid] = checkClashExceptionsResults.Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.E);

                    // Any include means no spot can be added
                    if (spotHasClashExceptionIncludes[spot.Uid])
                    {
                        return false;
                    }
                }
            }

            if (productClashRule != ProductClashRules.NoClashes && productClashRule != ProductClashRules.LimitOnExposureCount)
            {
                return true;
            }

            bool canAddSpots = true;

            foreach (var spot in spots)
            {
                SpotInfo spotInfo = spotInfos[spot.Uid];

                if (respectClashExceptions && spotHasClashExceptionIncludes[spot.Uid])
                {
                    // Spot has includes, cannot add spot
                    return false;
                }

                // Spot has excludes, no need to check parent/child clashes
                if (respectClashExceptions && spotHasClashExceptionExcludes[spot.Uid])
                {
                    continue;
                }

                // Get clashes at child level
                var spotsInBreak = _smoothBreak.SmoothSpots.ConvertAll(s => s.Spot);

                var childProductClashSpots = productClashChecker
                    .GetProductClashesForSingleSpot(
                        spot,
                        spotsInBreak,
                        spotInfos,
                        ClashCodeLevel.Child);

                switch (productClashRule)
                {
                    case ProductClashRules.NoClashes:
                        if (childProductClashSpots.Count > 0)
                        {
                            canAddSpots = false;
                        }
                        break;

                    case ProductClashRules.LimitOnExposureCount:
                        if (childProductClashSpots.Count > 0 && !String.IsNullOrEmpty(spotInfo.ProductClashCode))
                        {
                            if (clashesByExternalRef.TryGetValue(spotInfo.ProductClashCode, out Clash childClash))
                            {
                                int exposureCount = clashExposureCount.Calculate(
                                    childClash.Differences,
                                    (childClash.DefaultPeakExposureCount, childClash.DefaultOffPeakExposureCount),
                                    (spot.StartDateTime, spot.SalesArea)
                                );

                                if (exposureCount > 0
                                    && childProductClashSpots.Count + countNewSpotsPerChildClashCode[spotInfo.ProductClashCode] > exposureCount)
                                {
                                    canAddSpots = false;
                                }
                            }
                        }

                        var parentProductClashSpots = productClashChecker
                            .GetProductClashesForSingleSpot(
                                spot,
                                spotsInBreak,
                                spotInfos,
                                ClashCodeLevel.Parent);

                        if (parentProductClashSpots.Count > 0 && !String.IsNullOrEmpty(spotInfo.ParentProductClashCode))
                        {
                            if (clashesByExternalRef.TryGetValue(spotInfo.ParentProductClashCode, out Clash parentClash))
                            {
                                int exposureCount = clashExposureCount.Calculate(
                                    parentClash.Differences,
                                    (parentClash.DefaultPeakExposureCount, parentClash.DefaultOffPeakExposureCount),
                                    (spot.StartDateTime, spot.SalesArea)
                                );

                                if (exposureCount > 0
                                    && parentProductClashSpots.Count + countNewSpotsPerParentClashCode[spotInfo.ParentProductClashCode] > exposureCount)
                                {
                                    canAddSpots = false;
                                }
                            }
                        }

                        break;
                }
            }

            return canAddSpots;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="smoothProg"></param>
        /// <param name="spots"></param>
        /// <param name="respectRestrictions"></param>
        /// <param name="restrictionChecker"></param>
        /// <param name="breaksBeingSmoothed"></param>
        /// <param name="scheduleProgrammes"></param>
        /// <returns></returns>
        public bool CanAddSpotsWithRestrictionRule(
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Spot> spots,
            bool respectRestrictions,
            IRestrictionChecker restrictionChecker,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            if (!respectRestrictions)
            {
                return true;
            }

            bool canAddSpots = true;
            Break theBreak = _smoothBreak.TheBreak;

            foreach (var spot in spots)
            {
                var restrictionCheckerResults = restrictionChecker
                    .CheckRestrictions(
                        smoothProg.Programme,
                        theBreak,
                        spot,
                        smoothProg.SalesArea,
                        breaksBeingSmoothed,
                        scheduleProgrammes)
                    .ToList();

                if (restrictionCheckerResults.Count > 0)
                {
                    canAddSpots = false;
                    break;
                }
            }

            return canAddSpots;
        }

        /// <summary>
        /// Determines whether this Spot instance can be added to the requested
        /// break or container.
        /// </summary>
        /// <param name="spotPositionRequest">
        /// <para>
        /// The spot position request within the programme. This can be one of
        /// the following values.
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item>
        /// <term>
        /// Empty, white space or null
        /// </term>
        /// <description>
        /// No specific request is being made; place anywhere suitable.
        /// </description></item>
        /// <item>
        /// <term>A break external reference</term>
        /// <description>A specific break is requested.</description>
        /// </item>
        /// <item>
        /// <term>A break container reference</term>
        /// <description>A specific break container is requested.</description>
        /// </item>
        /// <item>
        /// <term>An integer value</term>
        /// <description>A specific break or container number within the programme.
        /// For example, 1 would be the first break or container, 3 would be the
        /// third, and so on.</description>
        /// </item>
        /// </list>
        /// </para>
        /// </param>
        /// <param name="programmeBreaks">The programme breaks.</param>
        /// <param name="breakPositionRules">The break position rules.</param>
        /// <returns>
        ///   <c>true</c> if this Spot instance can be added to the specified
        ///   break; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanAddSpotWithBreakRequest(
            string spotPositionRequest,
            IReadOnlyList<SmoothBreak> programmeBreaks,
            SpotPositionRules breakPositionRules)
        {
            if (String.IsNullOrWhiteSpace(spotPositionRequest))
            {
                return true;
            }

            if (spotPositionRequest == _smoothBreak.TheBreak.ExternalBreakRef)
            {
                return true;
            }

            if (!Int32.TryParse(spotPositionRequest, out int requestedBreakNumber))
            {
                return false;
            }

            requestedBreakNumber = GetActualBreakPositionFromRelativePosition(
                requestedBreakNumber,
                programmeBreaks.Count);

            return CanAddSpotAtBreakPosition(
                requestedBreakNumber,
                _smoothBreak.Position,
                breakPositionRules);
        }

        /// <summary>
        /// <para>
        /// Note: I'm using the word Atomic to mean a single non-multipart spot.
        /// Also note, this isn't the most efficient list of method parameters;
        /// I'm trying to align this method to <see cref="CanAddSpotWithBreakRequest"/>.
        /// </para>
        /// <para>
        /// The "WithDefaultPosition" refers to using a default position of the
        /// first break.
        /// </para>
        ///</summary>
        /// <remarks>
        /// This should be replaced by calls to CanAddSpotWithBreakRequest() but
        /// needs confirming as the logic is different.
        /// </remarks>
        public virtual bool CanAddSpotWithBreakRequestForAtomicSpotWithDefaultPosition(
            string spotPositionRequest,
            IReadOnlyList<SmoothBreak> programmeBreaks,
            SpotPositionRules breakPositionRules)
        {
            if (String.IsNullOrWhiteSpace(spotPositionRequest))
            {
                return true;
            }

            if (spotPositionRequest == _smoothBreak.TheBreak.ExternalBreakRef)
            {
                return true;
            }

            if (!Int32.TryParse(spotPositionRequest, out int requestedBreakPosition))
            {
                requestedBreakPosition = 0;
            }

            requestedBreakPosition = GetActualBreakPositionFromRelativePosition(
                requestedBreakPosition,
                programmeBreaks.Count);

            return CanAddSpotAtBreakPosition(
                requestedBreakPosition,
                _smoothBreak.Position,
                breakPositionRules);
        }

        /// <summary>
        /// <para>
        /// Note: This isn't the most efficient list of method parameters;
        /// I'm trying to align this method to <see cref="CanAddSpotWithBreakRequest"/>.
        /// </para>
        /// <para>
        /// The "WithDefaultPosition" refers to using a default position of the
        /// first break.
        /// </para>
        ///</summary>
        /// <remarks>
        /// This should be replaced by calls to CanAddSpotWithBreakRequest() but
        /// needs confirming as the logic is different.
        /// </remarks>
        public virtual bool CanAddSpotWithBreakRequestForMultipartSpotWithDefaultPosition(
            string spotPositionRequest,
            IReadOnlyList<SmoothBreak> programmeBreaks,
            SpotPositionRules breakPositionRules)
        {
            if (String.IsNullOrWhiteSpace(spotPositionRequest))
            {
                return true;
            }

            int requestedBreakPosition;
            if (spotPositionRequest == _smoothBreak.TheBreak.ExternalBreakRef)
            {
                requestedBreakPosition = _smoothBreak.Position;
            }
            else if (Int32.TryParse(spotPositionRequest, out requestedBreakPosition))
            {
                requestedBreakPosition = GetActualBreakPositionFromRelativePosition(
                    requestedBreakPosition,
                    programmeBreaks.Count);
            }
            else
            {
                requestedBreakPosition = 0;
            }

            return CanAddSpotAtBreakPosition(
                requestedBreakPosition,
                _smoothBreak.Position,
                breakPositionRules);
        }

        /// <summary>
        /// Determines whether a Spot can be placed in a Break by comparing break types.
        /// </summary>
        /// <param name="spotBreakType">The break type of the Spot.</param>
        /// <returns>
        /// Returns <c>true</c> if the spot can be added to the break;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool CanAddSpotWithBreakType(string spotBreakType) =>
            String.IsNullOrWhiteSpace(spotBreakType) ||
                _smoothBreak.TheBreak.BreakType.Equals(spotBreakType, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="spotStartDateTime"></param>
        /// <param name="spotEndDateTime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Whether the spot meets basic eligibility rules for adding to a Break.
        /// </summary>
        public bool IsSpotEligibleWhenIgnoringSpotTime(Spot spot) =>
            CanAddSpotWithBreakType(spot.BreakType);

        /// <summary>
        /// Whether the spot meets basic eligibility rules for adding to a Break.
        /// </summary>
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
        public static int GetActualBreakPositionFromRelativePosition(
            int position,
            int progBreakCount) =>
            position switch
            {
                97 => progBreakCount - 2,
                98 => progBreakCount - 1,
                99 => progBreakCount,
                _ => position,
            };
    }
}
