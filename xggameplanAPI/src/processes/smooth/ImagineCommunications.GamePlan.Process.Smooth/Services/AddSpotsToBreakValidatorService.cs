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
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class AddSpotsToBreakValidatorService
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
            IReadOnlyList<SmoothBreak> progSmoothBreaks,
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
            SponsorshipRestrictionService sponsorshipRestrictionsService)
        {
            var result = new SmoothFailureMessagesForSpotsCollection(theSmoothBreak);

            foreach (Guid spotUid in spotsForBreak.Select(s => s.Uid))
            {
                result.InitialiseForSpot(spotUid);
            }

            if (!IsSufficientRemainingDurationToAddSpots(
                theSmoothBreak.RemainingAvailability,
                spotsForBreak))
            {
                foreach (Guid spotUid in spotsForBreak.Select(s => s.Uid))
                {
                    result.Add(
                        spotUid,
                        SmoothFailureMessages.T1_InsufficentRemainingDuration);
                }
            }

            var (spotChildClashCodeCount, spotParentClashCodeCount) =
                ClashCodesForSpotsCount(spotsForBreak, spotInfos);

            // Check basics of whether we can add this spot to the break,
            // correct break type, sufficient time remaining, product clashes,
            // campaign clashes
            var spotsAlreadyInTheBreak = theSmoothBreak.SmoothSpots
                .ConvertAll(s => s.Spot);

            var canAddSpotService = CanAddSpotService.Factory(theSmoothBreak);

            foreach (Spot spot in spotsForBreak)
            {
                ValidateWithSponsorshipRestrictions(
                    theSmoothBreak,
                    sponsorshipRestrictionsService,
                    result,
                    spotsAlreadyInTheBreak,
                    spot);

                Guid spotUid = spot.Uid;

                if (!canAddSpotService.CanAddSpotWithBreakType(spot.BreakType))
                {
                    result.Add(spotUid, SmoothFailureMessages.T1_InvalidBreakType);
                }

                if (respectSpotTime && !canAddSpotService.CanAddSpotWithTime(spot.StartDateTime, spot.EndDateTime))
                {
                    result.Add(spotUid, SmoothFailureMessages.T1_InvalidSpotTime);
                }

                var (spotHasClashExceptionIncludes, spotHasClashExceptionExcludes) =
                    HasClashExceptionIncludesAndExcludes(
                        theSmoothBreak,
                        respectClashExceptions,
                        smoothResources,
                        result,
                        spot,
                        spotUid);

                bool shouldCheckProductClashRules =
                    (
                        productClashRule == ProductClashRules.NoClashes
                        || productClashRule == ProductClashRules.LimitOnExposureCount
                    )
                    && !spotHasClashExceptionIncludes
                    && !spotHasClashExceptionExcludes;

                if (shouldCheckProductClashRules)
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
                            var output = CheckProductClashRulesWhenNoClashesAreAllowed(
                                childProductClashSpots);

                            if (output != SmoothFailureMessages.T0_NoFailure)
                            {
                                result.Add(spotUid, output);
                            }

                            break;

                        case ProductClashRules.LimitOnExposureCount:
                            CheckProductClashRulesWhenClashLimitsAreAllowed(
                                spotInfos,
                                clashesByExternalRef,
                                smoothResources,
                                clashExposureCountService,
                                result,
                                spotChildClashCodeCount,
                                spotParentClashCodeCount,
                                spotsAlreadyInTheBreak,
                                spot,
                                spotInfos[spotUid],
                                childProductClashSpots);

                            break;
                    }
                }

                if (respectCampaignClash
                    && smoothResources.CampaignClashChecker
                        .GetCampaignClashesForNewSpots(
                            new List<Spot> { spot },
                            spotsAlreadyInTheBreak)
                        .Count > 0)
                {
                    result.Add(spotUid, SmoothFailureMessages.T1_CampaignClash);
                }
            }

            var multipartSpots = spotsForBreak
                .Where(s => s.IsMultipartSpot)
                .ToList();

            var nonMultipartSpots = spotsForBreak
                .Except(multipartSpots)
                .ToList();

            if (nonMultipartSpots.Count > 0)
            {
                ValidateNonMultipartSpots(
                    progSmoothBreaks,
                    breakPositionRules,
                    requestedPositionInBreakRules,
                    canAddSpotService,
                    result,
                    nonMultipartSpots);
            }

            if (multipartSpots.Count > 0)
            {
                ValidateMultipartSpots(
                    progSmoothBreaks,
                    breakPositionRules,
                    requestedPositionInBreakRules,
                    canSplitMultipartSpotsOverBreaks,
                    canAddSpotService,
                    result,
                    spotsAlreadyInTheBreak,
                    multipartSpots);
            }

            if (respectRestrictions)
            {
                ValidateWithSpotRestrictions(
                    theSmoothBreak,
                    programme,
                    salesArea,
                    spotsForBreak,
                    smoothResources,
                    breaksBeingSmoothed,
                    scheduleProgrammes,
                    result);
            }

            IndicateIfMultipartTopTailSameBreakSpotsCannotBeAdded(
                result,
                multipartSpots);

            return RemoveDuplicateFailureMessages(spotsForBreak, result);
        }

        private static (bool spotHasClashExceptionIncludes, bool spotHasClashExceptionExcludes)
        HasClashExceptionIncludesAndExcludes(
            SmoothBreak theSmoothBreak,
            bool respectClashExceptions,
            SmoothResources smoothResources,
            SmoothFailureMessagesForSpotsCollection result,
            Spot spot,
            Guid spotUid)
        {
            (bool spotHasClashExceptionIncludes, bool spotHasClashExceptionExcludes) clashExceptionResults = (false, false);

            if (!respectClashExceptions)
            {
                return clashExceptionResults;
            }

            var checkClashExceptionsResults = smoothResources
                .ClashExceptionChecker.CheckClashExceptions(theSmoothBreak, spot);

            clashExceptionResults.spotHasClashExceptionIncludes = checkClashExceptionsResults
                .Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.I);

            clashExceptionResults.spotHasClashExceptionExcludes = checkClashExceptionsResults
                .Any(cer => cer.ClashException.IncludeOrExclude == IncludeOrExclude.E);

            if (clashExceptionResults.spotHasClashExceptionIncludes)
            {
                result.Add(spotUid, SmoothFailureMessages.T1_ProductClash);
            }

            return clashExceptionResults;
        }

        private static void ValidateWithSponsorshipRestrictions(
            SmoothBreak theSmoothBreak,
            SponsorshipRestrictionService sponsorshipRestrictionsService,
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            Spot spot)
        {
            Break theBreak = theSmoothBreak.TheBreak;

            IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)>
                serviceResult = sponsorshipRestrictionsService.CheckSponsorshipRestrictions(
                    spot,
                    theBreak.ExternalBreakRef,
                    theBreak.ScheduledDate,
                    theBreak.Duration,
                    spotsAlreadyInTheBreak
                    );

            foreach ((Guid spotUid, SmoothFailureMessages failureMessage) in serviceResult)
            {
                result.Add(spotUid, failureMessage);
            }
        }

        private static void ValidateWithSpotRestrictions(
            SmoothBreak theSmoothBreak,
            Programme programme,
            SalesArea salesArea,
            IReadOnlyCollection<Spot> spotsForBreak,
            SmoothResources smoothResources,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            SmoothFailureMessagesForSpotsCollection result)
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

        private static void ValidateMultipartSpots(
            IReadOnlyList<SmoothBreak> progSmoothBreaks,
            SpotPositionRules breakPositionRules,
            SpotPositionRules requestedPositionInBreakRules,
            bool canSplitMultipartSpotsOverBreaks,
            CanAddSpotService canAddSpotService,
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            IReadOnlyList<Spot> multipartSpots)
        {
            if (multipartSpots.Count == 0)
            {
                return;
            }

            foreach (var spot in multipartSpots)
            {
                bool canAddSpotAtBreakPosition = canAddSpotService
                    .CanAddSpotWithBreakRequestForMultipartSpotWithDefaultPosition(
                        spot.BreakRequest,
                        progSmoothBreaks,
                        breakPositionRules);

                if (!canAddSpotAtBreakPosition)
                {
                    result.Add(spot.Uid, SmoothFailureMessages.T1_BreakPosition);
                }

                // Determine if we can add at requested position in break
                // TODO: Check that multipart Spots are all linked
                if (!canAddSpotService.CanAddMultipartSpotAtRequestedPosition(
                    spot.MultipartSpot,
                    spot.MultipartSpotPosition,
                    requestedPositionInBreakRules))
                {
                    result.Add(spot.Uid, SmoothFailureMessages.T1_RequestedPositionInBreak);
                }
            }

            // For the multipart spot type then check if this is the break
            // that it must be added to
            if (canSplitMultipartSpotsOverBreaks)
            {
                return;
            }

            // Multipart spots can't be split over breaks, ensure that
            // break will contain all linked multipart spots
            if (multipartSpots[0].MultipartSpot == MultipartSpotTypes.TopTail
                && multipartSpots.Count < 2)
            {
                // We're not placing both spots, ensure that this break
                // has linked spot. Get all linked spots that have been
                // placed in this break.
                var linkedSpotsPlacedInBreak = BreakUtilities.GetLinkedMultipartSpots(
                    multipartSpots[0],
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

        private static void ValidateNonMultipartSpots(
            IReadOnlyList<SmoothBreak> progSmoothBreaks,
            SpotPositionRules breakPositionRules,
            SpotPositionRules requestedPositionInBreakRules,
            CanAddSpotService canAddSpotService,
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyCollection<Spot> nonMultipartSpots)
        {
            // Determine which break we want to position the spot in if possible
            foreach (var spot in nonMultipartSpots)
            {
                bool canAddSpotAtBreakPosition = canAddSpotService
                    .CanAddSpotWithBreakRequestForAtomicSpotWithDefaultPosition(
                        spot.BreakRequest,
                        progSmoothBreaks,
                        breakPositionRules);

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

        private static void IndicateIfMultipartTopTailSameBreakSpotsCannotBeAdded(
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyCollection<Spot> multipartSpots)
        {
            bool haveFailureMessages = multipartSpots.Any(
                spot => result[spot.Uid].Failures.Count > 0
                );

            if (!haveFailureMessages)
            {
                return;
            }

            bool isTopTailMultipartSpots = multipartSpots.Any(spot => spot.MultipartSpot == MultipartSpotTypes.TopTail);
            bool isSameBreakMultipartSpots = multipartSpots.Any(spot => spot.MultipartSpot == MultipartSpotTypes.SameBreak);

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

        private static void CheckProductClashRulesWhenClashLimitsAreAllowed(
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            SmoothResources smoothResources,
            IClashExposureCountService clashExposureCountService,
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyDictionary<string, int> spotChildClashCodeCount,
            IReadOnlyDictionary<string, int> spotParentClashCodeCount,
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            Spot spot,
            SpotInfo spotInfo,
            IReadOnlyCollection<Spot> childProductClashSpots)
        {
            if (childProductClashSpots.Count > 0 && !String.IsNullOrEmpty(spotInfo.ProductClashCode))
            {
                CheckProductClashExposureCount(
                    clashesByExternalRef,
                    clashExposureCountService,
                    result,
                    spotChildClashCodeCount,
                    spot,
                    spotInfo,
                    childProductClashSpots);
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
                CheckProductParentClashExposure(
                    clashesByExternalRef,
                    clashExposureCountService,
                    result,
                    spotParentClashCodeCount,
                    spot,
                    spotInfo,
                    parentProductClashSpots);
            }
        }

        private static void CheckProductParentClashExposure(
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IClashExposureCountService clashExposureCountService,
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyDictionary<string, int> spotParentClashCodeCount,
            Spot spot,
            SpotInfo spotInfo,
            IReadOnlyCollection<Spot> parentProductClashSpots)
        {
            if (!clashesByExternalRef.TryGetValue(spotInfo.ParentProductClashCode, out Clash parentClash))
            {
                return;
            }

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

        private static void CheckProductClashExposureCount(
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IClashExposureCountService clashExposureCountService,
            SmoothFailureMessagesForSpotsCollection result,
            IReadOnlyDictionary<string, int> spotChildClashCodeCount,
            Spot spot,
            SpotInfo spotInfo,
            IReadOnlyCollection<Spot> childProductClashSpots)
        {
            if (!clashesByExternalRef.TryGetValue(spotInfo.ProductClashCode, out Clash childClash))
            {
                return;
            }

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

        private static SmoothFailureMessages CheckProductClashRulesWhenNoClashesAreAllowed(
            IReadOnlyCollection<Spot> childProductClashSpots)
        {
            return childProductClashSpots.Count == 0
                ? SmoothFailureMessages.T0_NoFailure
                : SmoothFailureMessages.T1_ProductClash;
        }

        private static SmoothFailureMessagesForSpotsCollection RemoveDuplicateFailureMessages(
            IReadOnlyCollection<Spot> spotsForBreak,
            SmoothFailureMessagesForSpotsCollection messages)
        {
            var result = new SmoothFailureMessagesForSpotsCollection();

            foreach (var spotUid in spotsForBreak.Select(s => s.Uid))
            {
                result.InitialiseForSpot(spotUid);

                foreach (var failure in messages[spotUid].Failures
                    .OrderBy(f => Convert.ToInt32(f.FailureMessage)))
                {
                    if (result[spotUid].Failures.Any(f => f.FailureMessage == failure.FailureMessage))
                    {
                        continue;
                    }

                    result.Add(
                        spotUid,
                        failure.FailureMessage,
                        failure.Restriction);
                }
            }

            return result;
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
            IReadOnlyDictionary<string, int> spotChildClashCodeCount,
            IReadOnlyDictionary<string, int> spotParentClashCodeCount)
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
            return _restrictionReasonToSmoothFailureMessageMap.ContainsKey(restrictionReason)
                ? _restrictionReasonToSmoothFailureMessageMap[restrictionReason]
                : SmoothFailureMessages.T0_NoFailure;
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
