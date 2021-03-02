using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using xggameplan.common;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Details of a Break within Smooth. Contains the Break plus Smooth specific properites.
    /// </summary>
    [DebuggerDisplay(nameof(Break.ExternalBreakRef) + " = {" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class SmoothBreak
        : ICloneable
    {
        private readonly List<SmoothSpot> _smoothSpots = new List<SmoothSpot>();

        private string GetDebuggerDisplay() => TheBreak.ExternalBreakRef;

        public SmoothBreak(Break theBreak, int position)
        {
            TheBreak = theBreak;
            Position = position;
            RemainingAvailability = TheBreak.Avail;
        }

        public SmoothBreak(Break theBreak, int position, List<SmoothSpot> smoothSpots)
            : this(theBreak, position) => _smoothSpots = smoothSpots;

        public Break TheBreak { get; }

        /// <summary>
        /// Spots for break
        /// </summary>
        public IEnumerable<SmoothSpot> SmoothSpots => _smoothSpots;

        /// <summary>
        /// A list of the Spots held within the SmoothSpots in this SmoothBreak.
        /// </summary>
        private List<Spot> SpotsInSmoothSpots => _smoothSpots
            .Select(s => s.Spot)
            .ToList();

        /// <summary>
        /// The remaining time availiabilty of the break.
        /// </summary>
        public Duration RemainingAvailability { get; set; }

        /// <summary>
        /// Break position
        /// </summary>
        public int Position { get; }

        public object Clone()
        {
            var smoothSpots = new List<SmoothSpot>();

            foreach (var smoothSpot in _smoothSpots)
            {
                smoothSpots.Add(
                    (SmoothSpot)smoothSpot.Clone()
                    );
            }

            Break breakClone = TheBreak is null
                ? null
                : (Break)TheBreak.Clone();

            return new SmoothBreak(breakClone, Position, smoothSpots)
            {
                RemainingAvailability = RemainingAvailability,
            };
        }

        /// <summary>
        /// Returns spots for same sponsors as input spot
        /// </summary>
        /// <param name="spots"></param>
        /// <param name="spotInfos"></param>
        /// <returns></returns>
        public IReadOnlyCollection<Spot> GetSponsorSpots(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos
            )
        {
            return SpotUtilities.GetSpotsWithSameSponsor(
                spots,
                SpotsInSmoothSpots,
                spotInfos);
        }

        /// <summary>
        /// Returns list of spot positions that are filled.
        /// </summary>
        public IReadOnlyDictionary<string, bool> GetSpotPositions()
        {
            var spotPositions = new Dictionary<string, bool>();
            var spotPositioning = new SpotPositioning();

            // Determine if we have any booked spots for ordinal positions (E.g.
            // 1st, 2nd, 3rd). These spots can't be moved. We can't really do
            // any checks for positions relative to last break as we don't know
            // how many spots that there should be.
            bool[] hasBookedSpotForPosition = new bool[4];
            for (int index = 0; index < hasBookedSpotForPosition.Length; index++)
            {
                string actualPosition = (index + 1).ToString();

                hasBookedSpotForPosition[index] = _smoothSpots.Any(s =>
                    !s.IsCurrent && s.Spot.ActualPositioninBreak == actualPosition);
            }

            // Check other spot positions. We don't check
            // Spot.RequestedPositionInBreak because the spot may not have been
            // placed in the break due to the particular PIB request.
            var hasSpotsForPositionInBreakRequest = new Dictionary<string, bool>();
            foreach (string currentPositionInBreakRequest in PositionInBreakRequests.All)
            {
                hasSpotsForPositionInBreakRequest[currentPositionInBreakRequest] = _smoothSpots
                    .Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromRequestedPositionInBreak(currentPositionInBreakRequest));
            }

            spotPositions.Add("1ST_START", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.First] || hasSpotsForPositionInBreakRequest[PositionInBreakRequests.TrueFirst] || hasBookedSpotForPosition[0]);
            spotPositions.Add("2ND_START", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.SecondFromStart] || hasBookedSpotForPosition[1]);
            spotPositions.Add("3RD_START", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.ThirdFromStart] || hasBookedSpotForPosition[2]);
            spotPositions.Add("4TH_START", hasBookedSpotForPosition[3]);
            spotPositions.Add("3RD_LAST", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.ThirdFromLast]);
            spotPositions.Add("2ND_LAST", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.SecondFromLast]);
            spotPositions.Add("LAST", hasSpotsForPositionInBreakRequest[PositionInBreakRequests.Last] || hasSpotsForPositionInBreakRequest[PositionInBreakRequests.TrueLast]);

            // Check for Multipart Top/Tail spots
            spotPositions.Add(
                "TT|TOP",
                _smoothSpots.Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                    MultipartSpotTypes.TopTail,
                    MultipartSpotPositions.TopTail_Top)
                ));

            spotPositions.Add(
                "TT|TAIL",
                _smoothSpots.Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                    MultipartSpotTypes.TopTail,
                    MultipartSpotPositions.TopTail_Tail)
                ));

            // Check for Multipart Same Break spots
            spotPositions.Add("SB|TOP", _smoothSpots
                .Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                    MultipartSpotTypes.SameBreak,
                    MultipartSpotPositions.SameBreak_Top)
                ));

            spotPositions.Add("SB|TAIL", _smoothSpots
                .Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                    MultipartSpotTypes.SameBreak,
                    MultipartSpotPositions.SameBreak_Tail)
                ));

            spotPositions.Add("SB|MID", _smoothSpots
                .Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                    MultipartSpotTypes.SameBreak,
                    MultipartSpotPositions.SameBreak_Mid)
                ));

            spotPositions.Add("SB|ANY", _smoothSpots
                .Any(s => s.BreakSequence == spotPositioning.GetBreakSeqFromMultipartSpotPosition(
                    MultipartSpotTypes.SameBreak,
                    MultipartSpotPositions.SameBreak_Any)
                ));

            return spotPositions;
        }

        /// <summary>
        /// Adds spot to break with specified break sequence. Spot may have been
        /// added in this Smooth run or a previous run.
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="smoothPassSequence"></param>
        /// <param name="breakSeq"></param>
        /// <param name="currentSpot"></param>
        /// <param name="canMoveSpotToOtherBreak"></param>
        public SmoothSpot AddSpot(
            Spot spot,
            int smoothPassSequence,
            int smoothPassIterationSequence,
            int breakSeq,
            bool currentSpot,
            bool canMoveSpotToOtherBreak,
            string bestBreakFactorGroupName,
            string externalBreakRefAtStart)
        {
            if (currentSpot)
            {
                spot.ExternalBreakNo = TheBreak.ExternalBreakRef;
            }

            var smoothSpot = new SmoothSpot(spot, smoothPassSequence, smoothPassIterationSequence)
            {
                IsCurrent = currentSpot,
                CanMoveToOtherBreak = canMoveSpotToOtherBreak,
                BestBreakFactorGroupName = bestBreakFactorGroupName,
                BreakSequence = breakSeq,
                ExternalBreakRefAtStart = externalBreakRefAtStart
            };

            // Insert in ordinal position
            bool added = false;
            for (int spotIndex = 0; spotIndex < _smoothSpots.Count; spotIndex++)
            {
                // Insert before this one
                if (breakSeq < _smoothSpots[spotIndex].BreakSequence)
                {
                    _smoothSpots.Insert(spotIndex, smoothSpot);
                    added = true;

                    break;
                }
            }

            if (!added)
            {
                _smoothSpots.Add(smoothSpot);
            }

            RemainingAvailability -= spot.SpotLength;

            return smoothSpot;
        }

        /// <summary>
        /// Removes spot from break
        /// </summary>
        /// <param name="spot"></param>
        public void RemoveSpot(Spot spot)
        {
            spot.ExternalBreakNo = null;
            spot.ActualPositioninBreak = null;

            SmoothSpot smoothSpot = _smoothSpots.Find(s => s.Spot.Uid == spot.Uid);
            _ = _smoothSpots.Remove(smoothSpot);

            RemainingAvailability += spot.SpotLength;
        }

        /// <summary>
        /// Renumbers actual position in break to reflect ordinal position of
        /// spots. This is necessary because during smoothing we may set large
        /// gaps of sequences so that we can insert spots in the gaps.
        /// </summary>
        public void RenumberActualPositionInBreak()
        {
            // Get BreakSeqType for Same Break Mid
            var spotPositioning = new SpotPositioning();
            var breakSeqTypeForMultipartSameBreakMid = spotPositioning.GetBreakSeqTypeForMultipartSpot(
                MultipartSpotTypes.SameBreak,
                MultipartSpotPositions.SameBreak_Mid);

            // Get all spots except Same Break Mid multipart spot, they need to
            // be placed in the middle of the break
            var mainSpotsInBreakSequenceOrder = _smoothSpots
                .Where(s => s.BreakSequence < breakSeqTypeForMultipartSameBreakMid.MinSequence || s.BreakSequence > breakSeqTypeForMultipartSameBreakMid.MaxSequence)
                .OrderBy(s => s.BreakSequence)
                .ToList();

            // Place the Same Break Mid multipart spot(s) in the middle, should
            // only be one but handle multiple. For odd number of spots then it
            // is placed in the exact middle. For even number of spots then in
            // either of the two middle spots.
            var multipartSameBreakMidSpots = _smoothSpots
                .Where(s => s.BreakSequence >= breakSeqTypeForMultipartSameBreakMid.MinSequence && s.BreakSequence <= breakSeqTypeForMultipartSameBreakMid.MaxSequence)
                .OrderBy(s => s.BreakSequence)
                .ToList();

            while (multipartSameBreakMidSpots.Count > 0)
            {
                var multipartSpot = multipartSameBreakMidSpots[0];
                _ = multipartSameBreakMidSpots.Remove(multipartSpot);

                int placeMultipartSpotIndex = mainSpotsInBreakSequenceOrder.Count / 2;
                mainSpotsInBreakSequenceOrder.Insert(placeMultipartSpotIndex, multipartSpot);
            }

            int actualPositionInBreak = 1;

            // Set Spot.ActualPositionInBreak. Spot.ActualPositioninBreak cannot
            // be modified for booked spots.
            foreach (SmoothSpot smoothSpot in mainSpotsInBreakSequenceOrder.Where(s => s.IsCurrent))
            {
                string position = actualPositionInBreak.ToString();

                // Ensure that break sequence is unique if already used by a
                // booked spot.
                while (_smoothSpots.Any(s => !s.IsCurrent && s.Spot.ActualPositioninBreak == position))
                {
                    actualPositionInBreak++;
                    position = actualPositionInBreak.ToString();
                }

                smoothSpot.Spot.ActualPositioninBreak = position;
                actualPositionInBreak++;
            }
        }

        /// <summary>
        /// Whether it is possible to add multipart spot at requested position
        /// </summary>
        /// <param name="multipartSpot"></param>
        /// <param name="multipartSpotPosition"></param>
        /// <returns></returns>
        public bool CanAddMultipartSpotAtRequestedPosition(
            string multipartSpot,
            string multipartSpotPosition,
            SpotPositionRules requestedPositionInBreakRule)
        {
            IReadOnlyDictionary<string, bool> hasSpotPositions = GetSpotPositions();

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

            IReadOnlyDictionary<string, bool> hasSpotPositions = GetSpotPositions();

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
            ICampaignClashChecker campaignClashChecker
            )
        {
            if (!respectCampaignClash)
            {
                return true;
            }

            foreach (var spot in spots)
            {
                if (campaignClashChecker.GetCampaignClashesForNewSpots(
                        new List<Spot> { spot },
                        SpotsInSmoothSpots)
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
            ClashExceptionChecker clashExceptionChecker,
            IClashExposureCountService clashExposureCount
            )
        {
            // Count spots per parent & child clash code
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
                    List<CheckClashExceptionResult> checkClashExceptionsResults = clashExceptionChecker.CheckClashExceptions(this, spot);
                    spotHasClashExceptionIncludes[spot.Uid] = checkClashExceptionsResults.Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.I);
                    spotHasClashExceptionExcludes[spot.Uid] = checkClashExceptionsResults.Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.E);

                    if (spotHasClashExceptionIncludes[spot.Uid])    // Any include means no spot can be added
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
                var childProductClashSpots = productClashChecker
                    .GetProductClashesForSingleSpot(
                        spot,
                        SpotsInSmoothSpots,
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
                                SpotsInSmoothSpots,
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
        /// Whether spots can be added to break with restriction rule
        /// </summary>
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
            foreach (var spot in spots)
            {
                var restrictionCheckerResults = restrictionChecker
                    .CheckRestrictions(
                        smoothProg.Prog,
                        TheBreak,
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
    }
}
