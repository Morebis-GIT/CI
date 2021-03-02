using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    internal static class AddSpotsToBreakValidatorService
    {
        /// <summary>
        /// Evaluate whether spots can be added to break. We consider time
        /// remaining, break type, product clashes, campaign clashes, spot end
        /// time (some spots may required to be position in the first N mins of
        /// the programme), sponsor rules.
        /// </summary>
        public static SmoothFailureMessagesForSpotsCollection ValidateAddSpots(
            SmoothBreak theSmoothBreak,
            Programme programme,
            SalesArea salesArea,
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            int programmeBreakCount,
            ProductClashRules productClashRule,
            bool respectCampaignClash,
            bool respectSpotTime,
            bool respectRestrictions,
            bool respectClashExceptions,
            SpotPositionRules breakPositionRules,
            SpotPositionRules requestedPositionInBreakRules,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            bool canSplitMultipartSpotsOverBreaks,
            SmoothResources smoothResources,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            IClashExposureCountService clashExposureCountService,
            SponsorshipRestrictionService sponsorshipRestrictionsService,
            ICanAddSpot canAddSpotService)
        {
            var result = new SmoothFailureMessagesForSpotsCollection(theSmoothBreak);

            foreach (var spotUid in spotsForBreak.Select(s => s.Uid))
            {
                result.InitialiseForSpot(spotUid);
            }

            if (!IsSufficientRemainingDurationToAddSpots(
                    theSmoothBreak.RemainingAvailability,
                    spotsForBreak))
            {
                foreach (var spot in spotsForBreak)
                {
                    result.Add(
                        spot.Uid,
                        SmoothFailureMessages.T1_InsufficentRemainingDuration);
                }
            }

            var (spotChildClashCodeCount, spotParentClashCodeCount) =
                ClashCodesForSpotsCount(spotsForBreak, spotInfos);

            // Check basics of whether we can add this spot to the break,
            // correct break type, sufficient time remaining, product clashes,
            // campaign clashes
            var spotsAlreadyInTheBreak = theSmoothBreak.SmoothSpots
                .Select(s => s.Spot)
                .ToList();

            foreach (var spot in spotsForBreak)
            {
                IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)>
                    serviceResult = sponsorshipRestrictionsService.CheckSponsorshipRestrictions(
                        spot,
                        theSmoothBreak.TheBreak.ExternalBreakRef,
                        theSmoothBreak.TheBreak.ScheduledDate,
                        theSmoothBreak.TheBreak.Duration,
                        spotsAlreadyInTheBreak
                        );

                foreach (var item in serviceResult)
                {
                    result.Add(item.spotUid, item.failureMessage);
                }

                Guid spotUid = spot.Uid;
                SpotInfo spotInfo = spotInfos[spotUid];

                if (!canAddSpotService.CanAddSpotWithBreakType(spot.BreakType))
                {
                    result.Add(spotUid, SmoothFailureMessages.T1_InvalidBreakType);
                }

                if (respectSpotTime && !canAddSpotService.CanAddSpotWithTime(spot.StartDateTime, spot.EndDateTime))
                {
                    result.Add(spotUid, SmoothFailureMessages.T1_InvalidSpotTime);
                }

                // Check clash exceptions Include overrides any Exclude.
                var checkClashExceptionsResults = new List<CheckClashExceptionResult>();
                bool spotHasClashExceptionIncludes = false;
                bool spotHasClashExceptionExcludes = false;

                if (respectClashExceptions)
                {
                    checkClashExceptionsResults = smoothResources
                        .ClashExceptionChecker.CheckClashExceptions(theSmoothBreak, spot);

                    spotHasClashExceptionIncludes = checkClashExceptionsResults
                        .Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.I);

                    spotHasClashExceptionExcludes = checkClashExceptionsResults
                        .Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.E);

                    if (spotHasClashExceptionIncludes)
                    {
                        result.Add(spot.Uid, SmoothFailureMessages.T1_ProductClash);
                    }

                    // No action if Clash exception Excludes (and no Includes),
                    // not a clash.
                }

                // Check product clash rules
                if (productClashRule == ProductClashRules.NoClashes || productClashRule == ProductClashRules.LimitOnExposureCount)
                {
                    // Check clashes at child/parent level
                    if (!spotHasClashExceptionIncludes && !spotHasClashExceptionExcludes)
                    {
                        var childProductClashSpots = smoothResources.ProductClashChecker
                            .GetProductClashesForSingleSpot(
                                spot,
                                spotsAlreadyInTheBreak,
                                spotInfos,
                                ClashCodeLevel.Child);

                        switch (productClashRule)
                        {
                            case ProductClashRules.NoClashes:
                                if (childProductClashSpots.Count > 0)
                                {
                                    result.Add(spot.Uid, SmoothFailureMessages.T1_ProductClash);
                                }

                                break;

                            case ProductClashRules.LimitOnExposureCount:
                                if (childProductClashSpots.Count > 0 && !String.IsNullOrEmpty(spotInfo.ProductClashCode))
                                {
                                    if (clashesByExternalRef.TryGetValue(spotInfo.ProductClashCode, out Clash childClash))
                                    {
                                        int exposureCount = clashExposureCountService.Calculate(
                                            childClash.Differences,
                                            (childClash.DefaultPeakExposureCount, childClash.DefaultOffPeakExposureCount),
                                            (spot.StartDateTime, spot.SalesArea)
                                            );

                                        if (exposureCount > 0
                                            && childProductClashSpots.Count + spotChildClashCodeCount[spotInfo.ProductClashCode] > exposureCount)
                                        {
                                            result.Add(spot.Uid, SmoothFailureMessages.T1_ProductClash);
                                        }
                                    }
                                }

                                // Check parent clashes
                                var parentProductClashSpots = smoothResources.ProductClashChecker
                                    .GetProductClashesForSingleSpot(
                                        spot,
                                        spotsAlreadyInTheBreak,
                                        spotInfos,
                                        ClashCodeLevel.Parent)
                                    .ToList();

                                if (parentProductClashSpots.Count > 0 && !String.IsNullOrEmpty(spotInfo.ParentProductClashCode))
                                {
                                    if (clashesByExternalRef.TryGetValue(spotInfo.ParentProductClashCode, out Clash parentClash))
                                    {
                                        int exposureCount = clashExposureCountService.Calculate(
                                            parentClash.Differences,
                                            (parentClash.DefaultPeakExposureCount, parentClash.DefaultOffPeakExposureCount),
                                            (spot.StartDateTime, spot.SalesArea)
                                            );

                                        if (exposureCount > 0
                                            && parentProductClashSpots.Count + spotParentClashCodeCount[spotInfo.ParentProductClashCode] > exposureCount)
                                        {
                                            result.Add(spot.Uid, SmoothFailureMessages.T1_ProductClash);
                                        }
                                    }
                                }

                                break;
                        }
                    }
                }

                if (respectCampaignClash
                    && smoothResources.CampaignClashChecker
                        .GetCampaignClashesForNewSpots(
                            new List<Spot> { spot },
                            spotsAlreadyInTheBreak)
                        .Count > 0)
                {
                    result.Add(spot.Uid, SmoothFailureMessages.T1_CampaignClash);
                }
            }

            var multipartSpots = spotsForBreak
                .Where(s => s.IsMultipartSpot);

            var nonMultipartSpots = spotsForBreak
                .Except(multipartSpots);

            bool isTopTailMultipartSpots = multipartSpots.Any(spot => spot.MultipartSpot == MultipartSpotTypes.TopTail);
            bool isSameBreakMultipartSpots = multipartSpots.Any(spot => spot.MultipartSpot == MultipartSpotTypes.SameBreak);

            if (nonMultipartSpots.Any())
            {
                // Determine which break we want to position the spot in if possible
                foreach (var spot in nonMultipartSpots)
                {
                    bool canAddSpotAtBreakPosition = false;

                    if (!String.IsNullOrEmpty(spot.BreakRequest))
                    {
                        if (spot.BreakRequest == theSmoothBreak.TheBreak.ExternalBreakRef)
                        {
                            canAddSpotAtBreakPosition = true;
                        }
                        else
                        {
                            if (!Int32.TryParse(spot.BreakRequest, out int requestedBreakPosition))
                            {
                                requestedBreakPosition = 0;
                            }

                            requestedBreakPosition = CanAddSpotService.GetActualBreakPositionFromRelativePosition(requestedBreakPosition, programmeBreakCount);
                            canAddSpotAtBreakPosition = canAddSpotService.CanAddSpotAtBreakPosition(requestedBreakPosition, breakPositionRules);
                        }
                    }
                    else
                    {
                        canAddSpotAtBreakPosition = true;
                    }

                    if (!canAddSpotAtBreakPosition)
                    {
                        result.Add(spot.Uid, SmoothFailureMessages.T1_BreakPosition);
                    }

                    // Determine if we can add at requested position in break
                    if (!String.IsNullOrEmpty(spot.RequestedPositioninBreak)
                        && !canAddSpotService.CanAddSpotAtRequestedPosition(spot.RequestedPositioninBreak, requestedPositionInBreakRules))
                    {
                        result.Add(spot.Uid, SmoothFailureMessages.T1_RequestedPositionInBreak);
                    }
                }
            }

            if (multipartSpots.Any())
            {
                foreach (var spot in multipartSpots)
                {
                    if (!String.IsNullOrEmpty(spot.BreakRequest))
                    {
                        int requestedBreakPosition = 0;

                        if (spot.BreakRequest == theSmoothBreak.TheBreak.ExternalBreakRef)
                        {
                            requestedBreakPosition = theSmoothBreak.Position;
                        }
                        else
                        {
                            if (Int32.TryParse(spot.BreakRequest, out requestedBreakPosition))
                            {
                                requestedBreakPosition = CanAddSpotService.GetActualBreakPositionFromRelativePosition(
                                    requestedBreakPosition,
                                    programmeBreakCount);
                            }
                            else
                            {
                                const int TreatUnknownBreakPositionAsAnyPosition = int.MinValue;

                                requestedBreakPosition = TreatUnknownBreakPositionAsAnyPosition;
                            }
                        }

                        bool canAddSpotAtBreakPosition = canAddSpotService.CanAddSpotAtBreakPosition(
                            requestedBreakPosition,
                            breakPositionRules);

                        if (!canAddSpotAtBreakPosition)
                        {
                            result.Add(spot.Uid, SmoothFailureMessages.T1_BreakPosition);
                        }
                    }

                    // Determine if we can add at requested position in break
                    // TODO: Check that multipartSpots are all linked
                    if (!theSmoothBreak.CanAddMultipartSpotAtRequestedPosition(
                        spot.MultipartSpot,
                        spot.MultipartSpotPosition,
                        requestedPositionInBreakRules))
                    {
                        result.Add(spot.Uid, SmoothFailureMessages.T1_RequestedPositionInBreak);
                    }
                }

                // For the multipart spot type then check if this is the break
                // that it must be added to
                if (!canSplitMultipartSpotsOverBreaks)
                {
                    // Multipart spots can't be split over breaks, ensure that
                    // break will contain all linked multipart spots
                    if (multipartSpots.First().MultipartSpot == MultipartSpotTypes.TopTail
                        && multipartSpots.Count() < 2)
                    {
                        // We're not placing both spots, ensure that this break
                        // has linked spot. Get all linked spots that have been
                        // placed in this break.
                        var linkedSpotsPlacedInBreak = BreakUtilities.GetLinkedMultipartSpots(
                            multipartSpots.First(),
                            spotsAlreadyInTheBreak,
                            includeInputSpotInOutput: false
                            );

                        if (linkedSpotsPlacedInBreak.Count == 0)
                        {
                            // Break doesn't contain linked spots, can't add to
                            // this break
                            foreach (var spot in multipartSpots)
                            {
                                result.Add(
                                    spot.Uid,
                                    SmoothFailureMessages.T1_CantAddTopAndTailToSameBreak);
                            }
                        }
                    }
                }
            }

            if (respectRestrictions)
            {
                var anyRestrictions = CheckRestrictionsForSpots(
                    spotsForBreak,
                    theSmoothBreak.TheBreak,
                    programme,
                    salesArea,
                    smoothResources,
                    breaksBeingSmoothed,
                    scheduleProgrammes);

                foreach (var item in anyRestrictions)
                {
                    foreach (var failure in item.Value.Failures)
                    {
                        result.Add(item.Key, failure.FailureMessage, failure.Restriction);
                    }
                }
            }

            // Indicate if multiplart Top & Tail/Same Break can't be added
            if (multipartSpots.Any())
            {
                int countFailureMessages = 0;

                foreach (var spot in multipartSpots)
                {
                    countFailureMessages += result[spot.Uid].Failures.Count;
                }

                if (countFailureMessages > 0)
                {
                    if (isTopTailMultipartSpots)
                    {
                        foreach (var spot in multipartSpots)
                        {
                            result.Add(spot.Uid, SmoothFailureMessages.T1_CantAddTopAndTailToSameBreak);
                        }
                    }
                    else if (isSameBreakMultipartSpots)
                    {
                        foreach (var spot in multipartSpots)
                        {
                            result.Add(spot.Uid, SmoothFailureMessages.T1_CantAddSameBreakToSameBreak);
                        }
                    }
                }
            }

            // Return results, remove duplicate failure messages
            var resultFinal = new SmoothFailureMessagesForSpotsCollection();

            foreach (var spotUid in spotsForBreak.Select(s => s.Uid))
            {
                resultFinal.InitialiseForSpot(spotUid);

                foreach (var failure in result[spotUid]
                    .Failures
                    .OrderBy(f => Convert.ToInt32(f.FailureMessage)))
                {
                    if (resultFinal[spotUid].Failures.Any(f => f.FailureMessage == failure.FailureMessage))
                    {
                        continue;
                    }

                    resultFinal.Add(
                        spotUid,
                        failure.FailureMessage,
                        failure.Restriction);
                }
            }

            return resultFinal;
        }

        private static SmoothFailureMessagesForSpotsCollection CheckRestrictionsForSpots(
            IReadOnlyCollection<Spot> spotsForBreak,
            Break theBreak,
            Programme programme,
            SalesArea salesArea,
            SmoothResources smoothResources,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            var result = new SmoothFailureMessagesForSpotsCollection();

            foreach (var spot in spotsForBreak)
            {
                var restrictionCheckerResults = smoothResources
                    .RestrictionChecker.CheckRestrictions(
                        programme,
                        theBreak,
                        spot,
                        salesArea,
                        breaksBeingSmoothed,
                        scheduleProgrammes
                        );

                foreach (var restrictionCheckerResult in restrictionCheckerResults
                    .Where(r => r.Reason != RestrictionReasons.None)
                    )
                {
                    var failureMessage = Map(restrictionCheckerResult.Reason);
                    if (failureMessage == SmoothFailureMessages.T0_NoFailure)
                    {
                        continue;
                    }

                    result.Add(
                        spot.Uid,
                        failureMessage,
                        restrictionCheckerResult.Restriction
                        );
                }
            }

            return result;
        }

        /// <summary>
        /// Whether sufficient duration for adding the spots
        /// </summary>
        /// <param name="spots"></param>
        /// <returns></returns>
        [Pure]
        private static bool IsSufficientRemainingDurationToAddSpots(
            Duration smoothBreakRemainingDuration,
            IReadOnlyCollection<Spot> spots)
        {
            Duration spotsDuration = spots.Aggregate(
                Duration.Zero,
                (current, spot) => current.Plus(spot.SpotLength)
                );

            return smoothBreakRemainingDuration >= spotsDuration;
        }

        /// <summary>
        /// Counts the number of unique child and parent clash codes attached to
        /// the given spots.
        /// </summary>
        /// <param name="spots"></param>
        /// <param name="spotInfos"></param>
        /// <returns></returns>
        private static (
            IDictionary<string, int> spotChildClashCodeCount,
            IDictionary<string, int> spotParentClashCodeCount)
        ClashCodesForSpotsCount(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            var countNewSpotsPerChildClashCode = new Dictionary<string, int>();
            var countNewSpotsPerParentClashCode = new Dictionary<string, int>();

            foreach (var spot in spots)
            {
                SpotInfo spotInfo = spotInfos[spot.Uid];

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

            return (
                countNewSpotsPerChildClashCode,
                countNewSpotsPerParentClashCode
                );
        }

        /// <summary>
        /// Map a restriction reason to a Smooth failure message.
        /// </summary>
        /// <param name="restrictionReason"></param>
        /// <returns></returns>
        private static SmoothFailureMessages Map(RestrictionReasons restrictionReason)
        {
            if (_restrictionReasonToSmoothFailureMessageMap.ContainsKey(restrictionReason))
            {
                return _restrictionReasonToSmoothFailureMessageMap[restrictionReason];
            }

            return SmoothFailureMessages.T0_NoFailure;
        }

        private static readonly Dictionary<RestrictionReasons, SmoothFailureMessages> _restrictionReasonToSmoothFailureMessageMap
            = new Dictionary<RestrictionReasons, SmoothFailureMessages>
            {
                { RestrictionReasons.ClearanceCodeRestrictionForClearanceCode, SmoothFailureMessages.T1_ClearanceCodeRestrictionForClearanceCode },
                { RestrictionReasons.ClearanceCodeRestrictionForCopy, SmoothFailureMessages.T1_ClearanceCodeRestrictionForCopy },
                { RestrictionReasons.ClearanceCodeRestrictionForProduct, SmoothFailureMessages.T1_ClearanceCodeRestrictionForProduct },
                { RestrictionReasons.ClearanceCodeRestrictionForClash, SmoothFailureMessages.T1_ClearanceCodeRestrictionForClash },
                { RestrictionReasons.IndexRestrictionForClearanceCode, SmoothFailureMessages.T1_IndexRestrictionForClearanceCode },
                { RestrictionReasons.IndexRestrictionForCopy, SmoothFailureMessages.T1_IndexRestrictionForCopy },
                { RestrictionReasons.IndexRestrictionForProduct, SmoothFailureMessages.T1_IndexRestrictionForProduct },
                { RestrictionReasons.IndexRestrictionForClash, SmoothFailureMessages.T1_IndexRestrictionForClash },
                { RestrictionReasons.ProgrammeCategoryRestrictionForClearanceCode, SmoothFailureMessages.T1_ProgrammeCategoryRestrictionForClearanceCode },
                { RestrictionReasons.ProgrammeCategoryRestrictionForCopy, SmoothFailureMessages.T1_ProgrammeCategoryRestrictionForCopy },
                { RestrictionReasons.ProgrammeCategoryRestrictionForProduct, SmoothFailureMessages.T1_ProgrammeCategoryRestrictionForProduct },
                { RestrictionReasons.ProgrammeCategoryRestrictionForClash, SmoothFailureMessages.T1_ProgrammeCategoryRestrictionForClash },
                { RestrictionReasons.ProgrammeClassificationRestrictionForClearanceCode, SmoothFailureMessages.T1_ProgrammeClassificationRestrictionForClearanceCode },
                { RestrictionReasons.ProgrammeClassificationRestrictionForCopy, SmoothFailureMessages.T1_ProgrammeClassificationRestrictionForCopy },
                { RestrictionReasons.ProgrammeClassificationRestrictionForProduct, SmoothFailureMessages.T1_ProgrammeClassificationRestrictionForProduct },
                { RestrictionReasons.ProgrammeClassificationRestrictionForClash, SmoothFailureMessages.T1_ProgrammeClassificationRestrictionForClash },
                { RestrictionReasons.ProgrammeRestrictionForClearanceCode, SmoothFailureMessages.T1_ProgrammeRestrictionForClearanceCode },
                { RestrictionReasons.ProgrammeRestrictionForCopy, SmoothFailureMessages.T1_ProgrammeRestrictionForCopy },
                { RestrictionReasons.ProgrammeRestrictionForProduct, SmoothFailureMessages.T1_ProgrammeRestrictionForProduct },
                { RestrictionReasons.ProgrammeRestrictionForClash, SmoothFailureMessages.T1_ProgrammeRestrictionForClash },
                { RestrictionReasons.TimeRestrictionForClearanceCode, SmoothFailureMessages.T1_TimeRestrictionForClearanceCode },
                { RestrictionReasons.TimeRestrictionForCopy, SmoothFailureMessages.T1_TimeRestrictionForCopy },
                { RestrictionReasons.TimeRestrictionForProduct, SmoothFailureMessages.T1_TimeRestrictionForProduct },
                { RestrictionReasons.TimeRestrictionForClash, SmoothFailureMessages.T1_TimeRestrictionForClash }
            };
    }
}
