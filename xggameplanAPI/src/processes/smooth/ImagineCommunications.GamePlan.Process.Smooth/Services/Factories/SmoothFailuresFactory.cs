using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Factory for providing SmoothFailure instances.
    /// </summary>
    public class SmoothFailuresFactory
    {
        private readonly ISmoothConfiguration _smoothConfiguration;

        public SmoothFailuresFactory(ISmoothConfiguration smoothConfiguration)
            => _smoothConfiguration = smoothConfiguration;

        /// <summary>
        /// <para>
        /// Creates Smooth failures for unplaced spots. For each spot then we
        /// find the results of the first placement attempt and then store the
        /// results for each of the breaks that we tried to place the spot in.
        /// We expect the lists that we search to be in chronoglogical order.
        /// </para>
        /// <para>
        /// In the main pass then we make an attempt to place every spot and so
        /// it shouldn't actually be necessary to search the results for every pass.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public List<SmoothFailure> CreateSmoothFailuresForUnplacedSpots(
            Guid runId,
            string salesAreaName,
            IReadOnlyCollection<SmoothPass> smoothPasses,
            IReadOnlyCollection<SmoothPassResult> smoothPassResults,
            IReadOnlyCollection<Spot> progSpotsNotUsed,
            IReadOnlyDictionary<string, Break> breaksByExternalRef,
            IReadOnlyDictionary<string, SpotPlacement> previousSpotPlacementsByExternalRef,
            IReadOnlyDictionary<string, (string Name, string CampaignGroup)> campaignsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalId)
        {
            if (progSpotsNotUsed.Count == 0)
            {
                return new List<SmoothFailure>();
            }

            var smoothFailures = new List<SmoothFailure>();

            // Ensure that GRID spots are excluded, Smooth shouldn't make any
            // attempt to place them
            foreach (var progSpotNotUsed in progSpotsNotUsed)
            {
                bool isExcludedCampaign = _smoothConfiguration.ExternalCampaignRefsToExclude.Contains(progSpotNotUsed.ExternalCampaignNumber);

                if (isExcludedCampaign && (!isExcludedCampaign || !_smoothConfiguration.SmoothFailuresForExcludedCampaigns))
                {
                    continue;
                }

                IOrderedEnumerable<SmoothPassResult> smoothPassesBySequence = smoothPassResults.OrderBy(p => p.Sequence);

                if (!smoothPassesBySequence.Any())
                {
                    continue;
                }

                foreach (var smoothPassResult in smoothPassesBySequence)
                {
                    // Determine what type of Smooth pass that it was
                    SmoothPass smoothPass = smoothPasses.First(sp => sp.Sequence == smoothPassResult.Sequence);
                    if (!(smoothPass is SmoothPassDefault))
                    {
                        continue;
                    }

                    bool addedSpot = false;

                    // Only default pass, exclude issues on final pass when
                    // trying to place unplaced spots
                    foreach (var placeSpotsResult in smoothPassResult.PlaceSpotsResultList)
                    {
                        // Check if this spot placement attempt is a failed one
                        // for this spot. We need to be careful and find the
                        // first instance that has failures, the spot could be
                        // valid for adding to the break but it wasn't but the
                        // best break.
                        var unplacedSpotResults = placeSpotsResult.UnplacedSpotResults
                            .Where(r => r.SpotId == progSpotNotUsed.Uid);

                        if (!unplacedSpotResults.Any())
                        {
                            continue;
                        }

                        if (!unplacedSpotResults.Any(usr =>
                            usr.ValidateAddSpotsResultForSpot?.Failures.Count > 0)
                            )
                        {
                            continue;
                        }

                        addedSpot = true;

                        foreach (var unplacedSpotResult in unplacedSpotResults
                            .Where(usr => usr.ValidateAddSpotsResultForSpot?.Failures.Count > 0)
                            )
                        {
                            Break aBreak = null;
                            if (unplacedSpotResult.ExternalBreakRef != null)
                            {
                                aBreak = breaksByExternalRef.ContainsKey(unplacedSpotResult.ExternalBreakRef)
                                    ? breaksByExternalRef[unplacedSpotResult.ExternalBreakRef]
                                    : null;
                            }

                            var (campaign, product, clash) = GetCampaignAndClashAndProductForSpot(
                                campaignsByExternalRef,
                                clashesByExternalRef,
                                productsByExternalId,
                                progSpotNotUsed);

                            var smoothFailure = new SmoothFailure
                            {
                                RunId = runId,
                                TypeId = 1,         // Unplaced at end of main pass
                                SalesArea = salesAreaName,
                                ExternalSpotRef = progSpotNotUsed.ExternalSpotRef,
                                ExternalBreakRef = unplacedSpotResult.ExternalBreakRef,  // Break that spot could not be added to
                                BreakDateTime = aBreak?.ScheduledDate ?? DateTime.MinValue,
                                SpotLength = progSpotNotUsed.SpotLength,
                                ExternalCampaignRef = progSpotNotUsed.ExternalCampaignNumber,
                                CampaignName = campaign.Name,
                                CampaignGroup = campaign.CampaignGroup,
                                AdvertiserIdentifier = product?.AdvertiserIdentifier,
                                AdvertiserName = product?.AdvertiserName,
                                ProductName = product?.Name,
                                ClashCode = product?.ClashCode,
                                ClashDescription = clash?.Description
                            };

                            foreach (var item in unplacedSpotResult.ValidateAddSpotsResultForSpot.Failures)
                            {
                                smoothFailure.MessageIds.Add((int)item.FailureMessage);
                            }

                            // Set restriction details, if failure related to restriction
                            var restrictionResult = unplacedSpotResult.ValidateAddSpotsResultForSpot.Failures
                                .Find(f => f.Restriction != null);

                            if (restrictionResult != null)
                            {
                                smoothFailure.IndustryCode = progSpotNotUsed.IndustryCode;
                                smoothFailure.ClearanceCode = restrictionResult.Restriction.ClearanceCode;
                                smoothFailure.RestrictionStartDate = restrictionResult.Restriction.StartDate;
                                smoothFailure.RestrictionStartTime = restrictionResult.Restriction.StartTime;
                                smoothFailure.RestrictionEndDate = restrictionResult.Restriction.EndDate;
                                smoothFailure.RestrictionEndTime = restrictionResult.Restriction.EndTime;
                                smoothFailure.RestrictionDays = restrictionResult.Restriction.RestrictionDays;
                            }

                            // If spot was bumped from previous run then add MessageId
                            if (progSpotNotUsed.ExternalSpotRef != null)
                            {
                                if (previousSpotPlacementsByExternalRef.TryGetValue(
                                        progSpotNotUsed.ExternalSpotRef,
                                        out SpotPlacement previousSpotPlacement))
                                {
                                    if (previousSpotPlacement != null &&
                                        !BreakUtilities.IsBreakRefNotSetOrUnused(
                                            previousSpotPlacement.ExternalBreakRef)
                                    )
                                    {
                                        smoothFailure.MessageIds.Add(
                                            (int)SmoothFailureMessages.T1_BumpedFromPreviousRun
                                            );
                                    }
                                }
                            }

                            smoothFailures.Add(smoothFailure);
                        }

                        if (addedSpot)
                        {
                            break;
                        }
                    }

                    if (addedSpot)
                    {
                        // Only generate failure for first attempt to place spot
                        break;
                    }
                }
            }

            return smoothFailures;
        }

        /// <summary>
        /// Creates Smooth failures for placed spots. Includes spots moved
        /// between breaks from previous run.
        /// </summary>
        public List<SmoothFailure> CreateSmoothFailuresForPlacedSpots(
            Guid runId,
            string salesAreaName,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyDictionary<string, Break> breaksByExternalRef,
            IReadOnlyDictionary<string, SpotPlacement> previousSpotPlacementsByExternalRef,
            IReadOnlyDictionary<string, (string Name, string CampaignGroup)> campaignsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalId)
        {
            if (progSmoothBreaks.Count == 0)
            {
                return new List<SmoothFailure>();
            }

            var smoothFailures = new List<SmoothFailure>();

            foreach (SmoothBreak progSmoothBreak in progSmoothBreaks)
            {
                var currentBreakSpots = progSmoothBreak.SmoothSpots
                    .Where(ss => ss.IsCurrent);

                if (!currentBreakSpots.Any())
                {
                    continue;
                }

                foreach (SmoothSpot placedSmoothSpot in currentBreakSpots)
                {
                    // Spots placed in this Smooth run Get previous spot
                    // placement, if exists
                    Spot spot = placedSmoothSpot.Spot;

                    if (spot?.ExternalSpotRef is null)
                    {
                        continue;
                    }

                    SpotPlacement previousSpotPlacement = previousSpotPlacementsByExternalRef.ContainsKey(spot.ExternalSpotRef)
                        ? previousSpotPlacementsByExternalRef[spot.ExternalSpotRef]
                        : null;

                    if (previousSpotPlacement?.ExternalBreakRef is null)
                    {
                        continue;
                    }

                    if (spot.ExternalBreakNo is null)
                    {
                        continue;
                    }

                    if (!BreakUtilities.IsBreakRefNotSetOrUnused(previousSpotPlacement.ExternalBreakRef)
                        && !String.Equals(
                            previousSpotPlacement.ExternalBreakRef,
                            spot.ExternalBreakNo,
                            StringComparison.InvariantCultureIgnoreCase
                        )
                    )
                    {
                        // Spot moved between breaks Get break, campaign,
                        // product & clash info for spot

                        Break aBreak = breaksByExternalRef.ContainsKey(spot.ExternalBreakNo)
                            ? breaksByExternalRef[spot.ExternalBreakNo]
                            : null;

                        var (campaign, product, clash) = GetCampaignAndClashAndProductForSpot(
                            campaignsByExternalRef,
                            clashesByExternalRef,
                            productsByExternalId,
                            spot);

                        var smoothFailure = new SmoothFailure()
                        {
                            RunId = runId,
                            TypeId = 3,
                            SalesArea = salesAreaName,
                            ExternalSpotRef = spot.ExternalSpotRef,
                            ExternalBreakRef = spot.ExternalBreakNo,
                            BreakDateTime = aBreak?.ScheduledDate ?? DateTime.MinValue,
                            SpotLength = spot.SpotLength,
                            ExternalCampaignRef = spot.ExternalCampaignNumber,
                            CampaignName = campaign.Name,
                            CampaignGroup = campaign.CampaignGroup,
                            AdvertiserIdentifier = product?.AdvertiserIdentifier,
                            AdvertiserName = product?.AdvertiserName,
                            ProductName = product?.Name,
                            ClashCode = product?.ClashCode,
                            ClashDescription = clash?.Description
                        };

                        smoothFailure.MessageIds.Add((int)SmoothFailureMessages.T3_MovedFromPreviousRun);
                        smoothFailures.Add(smoothFailure);
                    }
                }
            }

            return smoothFailures;
        }

        /// <summary>
        /// Create Smooth failures for spots where no attempt was made to place
        /// the spot. E.g. No break or programme data.
        /// </summary>
        public List<SmoothFailure> CreateSmoothFailuresForNoPlaceAttempt(
            Guid runId,
            string salesAreaName,
            IReadOnlyCollection<Spot> allSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, (string Name, string campaignGroup)> campaignsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalId
            )
        {
            // Ensure that GRID spots are excluded, Smooth shouldn't make any
            // attempt to place them
            var spotInfosNoPlaceAttempt = spotInfos.Where(si => si.Value.NoPlaceAttempt);
            if (!spotInfosNoPlaceAttempt.Any())
            {
                return new List<SmoothFailure>();
            }

            var smoothFailures = new List<SmoothFailure>();
            IReadOnlyDictionary<Guid, Spot> spotsByUId = SpotHelper.IndexListByUid(allSpots);

            // Process each spot, add failure, need to exclude any spots that
            // were already placed at the start
            foreach (var spotInfo in spotInfosNoPlaceAttempt)
            {
                bool isExcludedCampaign = false;

                if (spotsByUId.TryGetValue(spotInfo.Key, out Spot spot))
                {
                    isExcludedCampaign = _smoothConfiguration.ExternalCampaignRefsToExclude
                        .Contains(spot.ExternalCampaignNumber);
                }

                if (spot is null || spot.IsBooked())
                {
                    continue;
                }

                if (!isExcludedCampaign
                    || (isExcludedCampaign && _smoothConfiguration.SmoothFailuresForExcludedCampaigns)
                    )
                {
                    var (campaign, product, clash) = GetCampaignAndClashAndProductForSpot(
                        campaignsByExternalRef,
                        clashesByExternalRef,
                        productsByExternalId,
                        spot);

                    var smoothFailureTop = new SmoothFailure
                    {
                        RunId = runId,
                        TypeId = 2,
                        SalesArea = salesAreaName,
                        ExternalSpotRef = spot.ExternalSpotRef,
                        ExternalBreakRef = null,        // No attempt to place in break
                        BreakDateTime = DateTime.MinValue,
                        SpotLength = spot.SpotLength,
                        ExternalCampaignRef = spot.ExternalCampaignNumber,
                        CampaignName = campaign.Name,
                        CampaignGroup = campaign.CampaignGroup,
                        AdvertiserIdentifier = product?.AdvertiserIdentifier,
                        AdvertiserName = product?.AdvertiserName,
                        ProductName = product?.Name,
                        ClashCode = clash?.Externalref,
                        ClashDescription = clash?.Description,
                        MessageIds = new List<int>() {
                            (int)SmoothFailureMessages.T2_NoBreakOrProgrammeData
                        }
                    };

                    smoothFailures.Add(smoothFailureTop);
                }
            }

            return smoothFailures;
        }

        private static ((string Name, string CampaignGroup) campaign, Product product, Clash clash)
        GetCampaignAndClashAndProductForSpot(
            IReadOnlyDictionary<string, (string Name, string campaignGroup)> campaignsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalId,
            Spot spot)
        {
            (string, string) campaign = (null, null);
            Clash clash = null;
            Product product = null;

            if (spot.ExternalCampaignNumber != null)
            {
                _ = campaignsByExternalRef.TryGetValue(spot.ExternalCampaignNumber, out campaign);
            }

            if (spot.Product != null)
            {
                _ = productsByExternalId.TryGetValue(spot.Product, out product);

                if (product?.ClashCode != null)
                {
                    _ = clashesByExternalRef.TryGetValue(product.ClashCode, out clash);
                }
            }

            return (campaign, product, clash);
        }
    }
}
