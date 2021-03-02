using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.common;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Default Smooth pass executer. Gets spots matching the filter in priority
    /// order, attempts to find the best break using specific rules (i.e. Break
    /// selection rules, spot position rules etc).
    /// </summary>
    internal class SmoothPassDefaultExecuter : SmoothPassExecuter
    {
        private readonly IClashExposureCountService _clashExposureCountService;
        private readonly SponsorshipRestrictionService _sponsorshipRestrictionService;
        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly SmoothResources _smoothResources;

        private Action<string, Exception> RaiseException { get; }

        private static TimeSpan DefaultSpotDuration => TimeSpan.FromSeconds(30);

        public SmoothPassDefaultExecuter(
            ISmoothDiagnostics smoothDiagnostics,
            SmoothResources smoothResources,
            ISmoothConfiguration smoothConfiguration,
            IClashExposureCountService clashExposureCountService,
            SponsorshipRestrictionService sponsorshipRestrictionService,
            Action<string, Exception> raiseException
            )
        {
            _clashExposureCountService = clashExposureCountService;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;
            _smoothConfiguration = smoothConfiguration;
            _smoothDiagnostics = smoothDiagnostics;
            _smoothResources = smoothResources;

            RaiseException = raiseException;
        }

        /// <summary>
        /// Tries to find spots to place, includes the default spot (first spot)
        /// plus other spots of a particular durations
        /// </summary>
        /// <returns></returns>
        private static List<Spot> GetSpotsToPlaceForSpotDurations(
            IReadOnlyCollection<Spot> spotsToProcess,
            IReadOnlyCollection<TimeSpan> spotDurations,
            Spot defaultSpot)
        {
            if (spotDurations.Count == 0 || spotsToProcess.Count - 1 < spotDurations.Count)
            {
                return new List<Spot>();
            }

            var spotDurationsToFind = new List<TimeSpan>(spotDurations);
            var spotsToPlace = new List<Spot> { defaultSpot };

            foreach (Spot spotToProcess in spotsToProcess.Where(s => s != defaultSpot))
            {
                var spotLengthInSeconds = (long)spotToProcess
                    .SpotLength
                    .ToTimeSpan()
                    .TotalSeconds;

                var (isLengthWeWant, spotDurationIndex) = CheckIfSpotLengthIsOneWeWant(
                    spotLengthInSeconds,
                    spotDurationsToFind);

                if (!isLengthWeWant)
                {
                    continue;
                }

                spotsToPlace.Add(spotToProcess);
                spotDurationsToFind.RemoveAt(spotDurationIndex);

                if (spotDurationsToFind.Count == 0)
                {
                    // All spot durations found
                    return spotsToPlace;
                }
            }

            // No spots found
            return new List<Spot>();
        }

        [Pure]
        private static (bool isLengthWeWant, int index) CheckIfSpotLengthIsOneWeWant(
            long spotLengthTotalSeconds,
            List<TimeSpan> spotDurationsToFind)
        {
            int spotDurationIndex = spotDurationsToFind.FindIndex(sd =>
                (long)sd.TotalSeconds == spotLengthTotalSeconds
            );

            return (spotDurationIndex != -1, spotDurationIndex);
        }

        /// <summary>
        /// Returns spot filter for placing multiple spots at the same time as
        /// the default spot while respecting pass rules.
        /// </summary>
        private SpotFilter GetSpotFilterForPlacingShortSpots(
            IReadOnlyCollection<SmoothPassDefaultIteration> smoothPassIterations,
            Spot spotDefault,
            TimeSpan maxSpotLength,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef
            )
        {
            var spotDefaultSpotInfo = spotInfos[spotDefault.Uid];

            var spotFilter = new SpotFilter()
            {
                HasMultipartSpots = false,                                  // Exclude multipart spots as we want to place them together
                MinPreemptLevel = spotDefault.Preemptlevel,                 // Same preempt level
                MaxPreemptLevel = spotDefault.Preemptlevel,                 // Same preempt level
                SpotIdsToExclude = new HashSet<Guid>() { spotDefault.Uid }, // Exclude default spot
                MaxSpotLength = maxSpotLength,                              // Spot length limit, no point in considering spots that are bigger than the duration to fill
                ProductAdvertiserIdentifiersToExclude = new List<string>() { spotInfos[spotDefault.Uid].ProductAdvertiserIdentifier }    // Ignore spots for same advertiser
            };

            // Exclude for same campaign
            if (smoothPassIterations.Any(i => i.RespectCampaignClash) && !String.IsNullOrEmpty(spotDefault.ExternalCampaignNumber))
            {
                spotFilter.ExternalCampaignRefsToExclude = new List<string>() {
                    spotDefault.ExternalCampaignNumber
                };
            }

            // Exclude for same product clash code
            bool LimitOnExposureCount(SmoothPassDefaultIteration i) =>
                i.ProductClashRules == ProductClashRules.LimitOnExposureCount;

            bool NoProductClashesOrLimitOnExposureCount(SmoothPassDefaultIteration i) =>
                LimitOnExposureCount(i) ||
                i.ProductClashRules == ProductClashRules.NoClashes;

            if (smoothPassIterations.Any(NoProductClashesOrLimitOnExposureCount) && !String.IsNullOrEmpty(spotDefaultSpotInfo.ProductClashCode))
            {
                // Use most restrictive rules
                SmoothPassDefaultIteration iteration = smoothPassIterations
                    .Where(NoProductClashesOrLimitOnExposureCount)
                    .OrderBy(i => LimitOnExposureCount(i) ? 0 : 1)
                    .First();

                switch (iteration.ProductClashRules)
                {
                    case ProductClashRules.NoClashes:
                        spotFilter.ProductClashCodesToExclude = new List<string>
                        {
                            spotDefaultSpotInfo.ProductClashCode
                        };

                        break;

                    case ProductClashRules.LimitOnExposureCount:
                        if (clashesByExternalRef.TryGetValue(spotDefaultSpotInfo.ProductClashCode, out Clash clash))
                        {
                            int exposureCount = _clashExposureCountService.Calculate(
                                clash.Differences,
                                (clash.DefaultPeakExposureCount, clash.DefaultOffPeakExposureCount),
                                (spotDefault.StartDateTime, spotDefault.SalesArea)
                                );

                            if (exposureCount == 1)
                            {
                                spotFilter.ProductClashCodesToExclude = new List<string>
                                {
                                    spotDefaultSpotInfo.ProductClashCode
                                };
                            }
                        }
                        else
                        {
                            spotFilter.ProductClashCodesToExclude = new List<string>
                            {
                                spotDefaultSpotInfo.ProductClashCode
                            };
                        }

                        break;
                }
            }

            // Include no break request or same break
            if (smoothPassIterations.Any(spr => spr.BreakPositionRules == SpotPositionRules.Exact) && !String.IsNullOrEmpty(spotDefault.BreakRequest))
            {
                spotFilter.BreakRequests = new List<string>() { null, spotDefault.BreakRequest };
            }

            // Exclude for same position in break request
            if (smoothPassIterations.Any(spr => spr.RequestedPositionInBreakRules == SpotPositionRules.Exact) && !String.IsNullOrEmpty(spotDefault.RequestedPositioninBreak))
            {
                spotFilter.PositionInBreakRequestsToExclude = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(spotDefault.RequestedPositioninBreak);
            }

            return spotFilter;
        }

        /// <summary>
        /// Gets list of spots to place, either a single spot (first spot), a
        /// pair of multipart spots (first spot + linked spot) or a few short
        /// spots (first spot + N others).
        /// </summary>
        private List<Spot> GetSpotsToPlace(
            IReadOnlyCollection<SmoothPassDefaultIteration> smoothPassIterations,
            IReadOnlyCollection<Spot> spotsToProcess,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef
            )
        {
            var spotsToPlace = new List<Spot>();
            var spotDefault = spotsToProcess.First();

            if (spotDefault.IsMultipartSpot)
            {
                // Multipart spot, get linked spots.
                spotsToPlace.AddRange(
                    BreakUtilities.GetLinkedMultipartSpots(
                        spotDefault,
                        spotsToProcess,
                        includeInputSpotInOutput: true)
                    );
            }
            // See if we need to place multiple spots
            else if (spotsToProcess.Count > 1)
            {
                spotsToPlace.AddRange(
                    GetMultipleSpotsToPlace(
                        smoothPassIterations,
                        spotsToProcess,
                        spotInfos,
                        clashesByExternalRef,
                        spotDefault)
                );
            }

            // If no spots then just place single spot.
            if (spotsToPlace.Count == 0)
            {
                spotsToPlace.Add(spotDefault);
            }

            return spotsToPlace;
        }

        private IReadOnlyCollection<Spot> GetMultipleSpotsToPlace(
            IReadOnlyCollection<SmoothPassDefaultIteration> smoothPassIterations,
            IReadOnlyCollection<Spot> spotsToProcess,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            Spot spotDefault)
        {
            // For the default spot then set durations of other spots to find to
            // make up 30/60 seconds.
            ref readonly IReadOnlyList<TimeSpan[]> spotLengthsToFind = ref SpotLengths.GetSpotLengthsCombinationsToFind(spotDefault);

            if (spotLengthsToFind.Count == 0)
            {
                return Enumerable.Empty<Spot>().ToList();
            }

            // Need to place default spot + other spots set max spot length to
            // look for.
            TimeSpan maxSpotLength = spotDefault.SpotLength.ToTimeSpan().TotalSeconds > DefaultSpotDuration.TotalSeconds
                ? TimeSpan.FromSeconds(2 * DefaultSpotDuration.TotalSeconds)
                : DefaultSpotDuration;

            maxSpotLength -= spotDefault.SpotLength.ToTimeSpan();

            // Get spot filter for placing multiple short spots
            SpotFilter spotFilter = GetSpotFilterForPlacingShortSpots(
                smoothPassIterations,
                spotDefault,
                maxSpotLength,
                spotInfos,
                clashesByExternalRef);

            // Set list of functions to return the spots that we should pass
            // through the spot filter in priority order
            var getSpotsToFilterFunctions = new List<Func<IReadOnlyCollection<Spot>, IReadOnlyCollection<Spot>>>();

            getSpotsToFilterFunctions.Add(spots =>
                spots
                    .Where(s => s.Uid != spotDefault.Uid)
                    .OrderBy(s => s.Preemptable ? 0 : 1)
                    .ThenBy(s => s.Preemptlevel)
                    .ThenBy(s => s.Sponsored ? 0 : 1)
                    .ToList()
                );

            var spotsToPlace = new List<Spot>();
            foreach (var getSpotsToFilterFunction in getSpotsToFilterFunctions)
            {
                // Get spots to consider
                var spotsToProcessFiltered = GetSpots(
                        spotFilter,
                        getSpotsToFilterFunction(spotsToProcess),
                        spotInfos)
                    .ToList();

                // Sanity check
                if (!spotsToProcessFiltered.Contains(spotDefault))
                {
                    spotsToProcessFiltered.Insert(0, spotDefault);
                }

                // For each group of spot lengths then try and find spots of the
                // required length
                foreach (TimeSpan[] spotLengths in spotLengthsToFind
                    .Where(sl => sl.Length >= spotsToProcess.Count - 1))
                {
                    var spotsOfCorrectLength = GetSpotsToPlaceForSpotDurations(
                        spotsToProcessFiltered,
                        spotLengths,
                        spotDefault);

                    if (spotsOfCorrectLength.Count == 0)
                    {
                        continue;
                    }

                    spotsToPlace.AddRange(spotsOfCorrectLength);
                    break;
                }

                if (spotsToPlace.Count > 0)
                {
                    break;
                }
            }

            return spotsToPlace;
        }

        /// <summary>
        /// <para>Executes a Smooth pass.</para>
        /// <para>
        /// We find all spots that match the pass filters, order them and then
        /// attempt to place them. For multipart spots then if we can't place
        /// them together then we try and split them.
        /// </para>
        /// </summary>
        public SmoothPassResult Execute(
            SmoothPassDefault smoothPass,
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Spot> progSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            ICollection<Guid> spotIdsUsed)
        {
            var smoothPassResult = new SmoothPassResult(smoothPass.Sequence);

            try
            {
                // Get all spots for pass, order by priority for processing
                var spotFilter = new SpotFilter()
                {
                    Sponsored = smoothPass.Sponsored,
                    Preemptable = smoothPass.Preemptable,
                    MinPreemptLevel = null,     // Any
                    MaxPreemptLevel = null,     // Any
                    HasBreakRequest = null,     // Any
                    BreakRequests = smoothPass.BreakRequests,
                    HasPositionInBreakRequest = null,       // Any
                    PositionInBreakRequestsToExclude = null,        // Any
                    HasMultipartSpots = smoothPass.HasMultipartSpots,
                    MultipartSpots = null,      // Any
                    HasProductClashCode = smoothPass.HasProductClashCode,
                    ProductClashCodesToExclude = null,
                    ExternalCampaignRefsToExclude = _smoothConfiguration.ExternalCampaignRefsToExclude, // Always exclude these campaigns
                    HasSpotEndTime = null,
                    MinSpotLength = null,
                    MaxSpotLength = null,
                    SpotIdsToExclude = spotIdsUsed
                };

                var spotIdsProcessed = new HashSet<Guid>();

                while (true)
                {
                    // Get spots to process, need to do this on every loop
                    // because spots may have been unplaced and we need to
                    // process them in this pass in the correct preempt level order.
                    IEnumerable<Spot> spots = GetSpots(spotFilter, progSpots, spotInfos)
                        .Where(s => !spotIdsProcessed.Contains(s.Uid));

                    var sortedSpots = _smoothConfiguration
                        .SortSpotsToPlace(spots, smoothProg.Prog);

                    if (!sortedSpots.Any())
                    {
                        break;
                    }

                    var spotsToProcess = sortedSpots.ToList();

                    var smoothPassIterations = _smoothConfiguration
                        .GetSmoothPassDefaultIterations(
                            spotsToProcess[0],
                            smoothPass);

                    // Some passes may only have iterations for particular spot types.
                    if (smoothPassIterations.Count == 0)
                    {
                        // No pass iterations, remove spot so that we can
                        // process next one.
                        spotsToProcess.RemoveAt(0);
                        continue;
                    }

                    // Get the spots to add to break, a single spot, a pair
                    // of multipart spots or a few short spots
                    var spotsForBreak = GetSpotsToPlace(
                        smoothPassIterations,
                        spotsToProcess,
                        spotInfos,
                        clashesByExternalRef);

                    foreach (Guid spotUid in spotsForBreak.Select(s => s.Uid))
                    {
                        spotIdsProcessed.AddDistinct(spotUid);
                    }

                    // Set whether we can split spots
                    bool canSplitSpotsOverBreaks = true;
                    bool anyMultipartSpots = spotsForBreak.Any(spot => spot.IsMultipartSpot);

                    if (anyMultipartSpots)
                    {
                        canSplitSpotsOverBreaks = spotsForBreak.Count > 1 && smoothPass.CanSplitMultipartSpots;
                    }
                    else
                    {
                        canSplitSpotsOverBreaks = spotsForBreak.Count > 1;
                    }

                    PlaceSpotsResult placeSpotsResult = null;
                    try
                    {
                        placeSpotsResult = PlaceSpots(
                            smoothPass,
                            scheduleProgrammes,
                            smoothProg,
                            breaksBeingSmoothed,
                            spotsForBreak,
                            spotInfos,
                            clashesByExternalRef,
                            productsByExternalRef,
                            spotIdsUsed,
                            canSplitSpotsOverBreaks);
                    }
                    catch (Exception exception)
                    {
                        RaiseException($"Error placing spot {spotsForBreak[0].ExternalSpotRef} on pass {smoothPass.Sequence.ToString()}", exception);
#if DEBUG
                        // Do not hide the exception when debugging.
                        throw;
#endif
                    }
                    finally
                    {
                        // Remove spots that we tried to place from queue
                        if (placeSpotsResult?.PlacedSpotResults.Count > 0)
                        {
                            _ = spotsToProcess.RemoveAll(stp =>
                                    placeSpotsResult.PlacedSpotResults.Any(s
                                        => s.SpotId == stp.Uid)
                            );
                        }
                        else
                        {
                            // No spots placed, discard spot (if multipart
                            // then we'll process the linked spot on next loop)
                            spotsToProcess.RemoveAt(0);
                        }

                        // Store details of placing attempt
                        if (placeSpotsResult != null)
                        {
                            smoothPassResult.PlaceSpotsResultList.Add(placeSpotsResult);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Error executing Smooth pass {smoothPass.Sequence.ToString()}", exception);
            }

            return smoothPassResult;
        }

        /// <summary>
        /// Attempts to place spots, either a single spot or the linked
        /// multipart spots. Positioning rules are respected in the priority
        /// order defined (typically exact, near then anywhere).
        /// </summary>
        private PlaceSpotsResult PlaceSpots(
            SmoothPassDefault smoothPass,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            ICollection<Guid> spotIdsUsed,
            bool canSplitSpotsOverBreaks)
        {
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks = smoothProg.ProgSmoothBreaks;

            // Default to unplaced
            var placeSpotsResult = new PlaceSpotsResult();
            PlaceSpotsResult firstFailedPlaceSpotsResultForRule = null;

            // Check each iteration, try and find a break
            var smoothPassIterations = _smoothConfiguration.GetSmoothPassDefaultIterations(spotsForBreak.First(), smoothPass);

            // Get list of scenarios to try
            var smoothScenarioCreator = new SmoothScenarioCreator(_smoothResources, spotInfos);
            var smoothScenarioExecutor = new SmoothScenarioExecutor(
                _smoothDiagnostics,
                spotInfos,
                _sponsorshipRestrictionService);

            // Iterate through each pass iteration
            bool addedSpots = false;
            foreach (SmoothPassDefaultIteration smoothPassIteration in smoothPassIterations.OrderBy(i => i.Sequence))
            {
                // Validate adding spot to each break in the current state. This
                // is necessary so that we can identify scenarios that allow us
                // to place the spot in the ideal position. E.g. If insufficient
                // time in ideal break then we can create a scenario that moves
                // other spots from the break
                IReadOnlyCollection<SmoothScenario> smoothScenarios =
                    GetSmoothScenariosForSpotsValidForTheBreak(
                        scheduleProgrammes,
                        smoothProg,
                        breaksBeingSmoothed,
                        spotsForBreak,
                        spotInfos,
                        clashesByExternalRef,
                        progSmoothBreaks,
                        smoothScenarioCreator,
                        smoothPassIteration,
                        canSplitSpotsOverBreaks);

                // Check each scenario and work out the best break
                var spotPlacementResultsByScenarioId = new Dictionary<Guid, SpotPlacementResult>();

                var (bestSmoothScenario, failedPlaceSpotsResultForRule) = FindBestSmoothScenario(
                    useFirstSpotOnly: false,
                    smoothPass,
                    scheduleProgrammes,
                    smoothProg,
                    breaksBeingSmoothed,
                    spotsForBreak,
                    spotInfos,
                    clashesByExternalRef,
                    productsByExternalRef,
                    spotIdsUsed,
                    progSmoothBreaks,
                    smoothScenarioCreator,
                    smoothScenarioExecutor,
                    smoothPassIteration,
                    smoothScenarios,
                    spotPlacementResultsByScenarioId,
                    canSplitSpotsOverBreaks);

                firstFailedPlaceSpotsResultForRule = failedPlaceSpotsResultForRule;

                if (bestSmoothScenario is null)
                {
                    continue;
                }

                SpotPlacementResult bestSpotPlacementResult = spotPlacementResultsByScenarioId[bestSmoothScenario.Id];
                PlaceSpotsResult placeSpotsResultForRule = bestSpotPlacementResult.PlaceSpotsResult;
                placeSpotsResultForRule.SmoothBreak = bestSpotPlacementResult.BestBreakResult.SmoothBreak;

                bool foundBestBreakForSpot = placeSpotsResultForRule.SmoothBreak != null;
                if (!foundBestBreakForSpot)
                {
                    continue;
                }

                // Execute best scenario against the actual data (Spots, Breaks
                // etc). Log spot actions.
                smoothScenarioExecutor.Execute(
                    smoothPass,
                    smoothPassIteration.Sequence,
                    bestSmoothScenario,
                    progSmoothBreaks,
                    spotIdsUsed,
                    logSpotAction: true);

                // Set which break and placed/unplaced spots. Note that any
                // object references for scenario data (Spots, Breaks etc) refer
                // to the copy and not the original. We need to ensure that we
                // return the original reference.
                //
                // placeSpotsResultForRule.SmoothBreak refers to clone of the
                // actual break.
                placeSpotsResult.SmoothBreak = progSmoothBreaks.First(b => b.TheBreak.ExternalBreakRef == placeSpotsResultForRule.SmoothBreak.TheBreak.ExternalBreakRef);
                placeSpotsResult.BestBreakFactorGroupName = placeSpotsResultForRule.BestBreakFactorGroupName;
                placeSpotsResult.PlacedSpotResults.AddRange(placeSpotsResultForRule.PlacedSpotResults);
                placeSpotsResult.UnplacedSpotResults.AddRange(placeSpotsResultForRule.UnplacedSpotResults);

                var smoothSpotsAddedToBreak = SpotPlacementService.AddSpotsToBreak(
                    placeSpotsResult.SmoothBreak,
                    smoothPass.Sequence,
                    smoothPassIteration.Sequence,
                    spotsForBreak,
                    smoothPassIteration.RequestedPositionInBreakRules,
                    canMoveSpotToOtherBreak: true,
                    spotIdsUsed,
                    placeSpotsResult.BestBreakFactorGroupName,
                    spotInfos,
                    _sponsorshipRestrictionService);

                // Log spot action for diagnostic
                smoothSpotsAddedToBreak.ForEach(smoothSpot =>
                    _smoothDiagnostics.LogSpotAction(
                        smoothPass,
                        smoothPassIteration.Sequence,
                        smoothSpot.Spot,
                        placeSpotsResult.SmoothBreak,
                        SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                        "Best break")
                    );

                addedSpots = true;
                break;
            }

            if (!addedSpots && spotsForBreak.Count > 1 && canSplitSpotsOverBreaks)
            {
                var firstSpotForBreak = new List<Spot>() { spotsForBreak.First() };

                foreach (SmoothPassDefaultIteration smoothPassIteration in smoothPassIterations.OrderBy(i => i.Sequence))
                {
                    // Validate adding spot to each break in the current state.
                    // This is necessary so that we can identify scenarios that
                    // allow us to place the spot in the ideal position. E.g. If
                    // insufficient time in ideal break then we can create a
                    // scenario that moves other spots from the break
                    IReadOnlyCollection<SmoothScenario> smoothScenariosSingleSpot =
                        GetSmoothScenariosForSpotsValidForTheBreak(
                            scheduleProgrammes,
                            smoothProg,
                            breaksBeingSmoothed,
                            firstSpotForBreak,
                            spotInfos,
                            clashesByExternalRef,
                            progSmoothBreaks,
                            smoothScenarioCreator,
                            smoothPassIteration,
                            canSplitSpotsOverBreaks);

                    // Check each scenario and work out the best break
                    var spotPlacementResultsByScenarioId = new Dictionary<Guid, SpotPlacementResult>();

                    var (bestSmoothScenario, failedPlaceSpotsResultForRule) = FindBestSmoothScenario(
                        useFirstSpotOnly: true,
                        smoothPass,
                        scheduleProgrammes,
                        smoothProg,
                        breaksBeingSmoothed,
                        spotsForBreak,
                        spotInfos,
                        clashesByExternalRef,
                        productsByExternalRef,
                        spotIdsUsed,
                        progSmoothBreaks,
                        smoothScenarioCreator,
                        smoothScenarioExecutor,
                        smoothPassIteration,
                        smoothScenariosSingleSpot,
                        spotPlacementResultsByScenarioId,
                        canSplitSpotsOverBreaks);

                    firstFailedPlaceSpotsResultForRule = failedPlaceSpotsResultForRule;

                    if (bestSmoothScenario is null)
                    {
                        continue;
                    }

                    SpotPlacementResult bestSpotPlacementResult = spotPlacementResultsByScenarioId[bestSmoothScenario.Id];
                    PlaceSpotsResult firstSpotPlaceSpotsResultForRule = bestSpotPlacementResult.PlaceSpotsResult;

                    bool foundBestBreakForSpot = firstSpotPlaceSpotsResultForRule.SmoothBreak != null;
                    if (!foundBestBreakForSpot)
                    {
                        continue;
                    }

                    // Execute best scenario against the actual data (Spots,
                    // Breaks etc). Log spot actions.
                    smoothScenarioExecutor.Execute(
                        smoothPass,
                        smoothPassIteration.Sequence,
                        bestSmoothScenario,
                        progSmoothBreaks,
                        spotIdsUsed,
                        logSpotAction: true);

                    // Apply results to return, we only placed one spot
                    placeSpotsResult.SmoothBreak = progSmoothBreaks.First(b => b.TheBreak.ExternalBreakRef == firstSpotPlaceSpotsResultForRule.SmoothBreak.TheBreak.ExternalBreakRef);    // placeSpotsResultForRule.SmoothBreak refers to clone of the actual break
                    placeSpotsResult.BestBreakFactorGroupName = firstSpotPlaceSpotsResultForRule.BestBreakFactorGroupName;
                    placeSpotsResult.PlacedSpotResults.AddRange(firstSpotPlaceSpotsResultForRule.PlacedSpotResults);
                    placeSpotsResult.UnplacedSpotResults.AddRange(firstSpotPlaceSpotsResultForRule.UnplacedSpotResults);

                    var smoothSpotsAddedToBreak = SpotPlacementService.AddSpotsToBreak(
                        placeSpotsResult.SmoothBreak,
                        smoothPass.Sequence,
                        smoothPassIteration.Sequence,
                        firstSpotForBreak,
                        smoothPassIteration.RequestedPositionInBreakRules,
                        canMoveSpotToOtherBreak: true,
                        spotIdsUsed,
                        placeSpotsResult.BestBreakFactorGroupName,
                        spotInfos,
                        _sponsorshipRestrictionService);

                    // Log spot action for diagnostic
                    smoothSpotsAddedToBreak.ForEach(smoothSpot =>
                        _smoothDiagnostics.LogSpotAction(
                            smoothPass,
                            smoothPassIteration.Sequence,
                            smoothSpot.Spot,
                            firstSpotPlaceSpotsResultForRule.SmoothBreak,
                            SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                            "Best break (Single spot)")
                        );

                    addedSpots = true;
                    break;
                }
            }

            // If not placed in any break then return first attempt to place spots
            if (placeSpotsResult.SmoothBreak == null && firstFailedPlaceSpotsResultForRule != null)
            {
                placeSpotsResult.PlacedSpotResults.AddRange(firstFailedPlaceSpotsResultForRule.PlacedSpotResults);
                placeSpotsResult.UnplacedSpotResults.AddRange(firstFailedPlaceSpotsResultForRule.UnplacedSpotResults);
            }

            return placeSpotsResult;
        }

        private (SmoothScenario bestSmoothScenario, PlaceSpotsResult firstFailedPlaceSpotsResultForRule)
        FindBestSmoothScenario(
            bool useFirstSpotOnly,
            SmoothPassDefault smoothPass,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            ICollection<Guid> spotIdsUsed,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothScenarioCreator smoothScenarioCreator,
            SmoothScenarioExecutor smoothScenarioExecutor,
            SmoothPassDefaultIteration smoothPassIteration,
            IReadOnlyCollection<SmoothScenario> smoothScenarios,
            Dictionary<Guid, SpotPlacementResult> spotPlacementResultsByScenarioId,
            bool canSplitSpotsOverBreaks)
        {
            if (smoothScenarios.Count == 0)
            {
                return (null, null);
            }

            SmoothScenario bestSmoothScenario = null;
            PlaceSpotsResult firstFailedPlaceSpotsResultForRule = null;

            foreach (var smoothScenario in smoothScenarios.OrderBy(s => s.Sequence))
            {
                // Copy data (Spots, breaks etc) so that we can execute scenario
                // to it without affecting the actual data
                var scenarioSpotsForBreak = new List<Spot>();
                var scenarioProgSmoothBreaks = new List<SmoothBreak>();
                var scenarioSpotIdsUsed = new HashSet<Guid>();

                smoothScenarioCreator.CopyScenarioData(
                    spotsForBreak,
                    progSmoothBreaks,
                    spotIdsUsed,
                    scenarioSpotsForBreak,
                    scenarioProgSmoothBreaks,
                    scenarioSpotIdsUsed);

                var validateSpotsAndGetSpotPlacement = useFirstSpotOnly
                    ? new List<Spot>() { scenarioSpotsForBreak[0] }
                    : scenarioSpotsForBreak;

                SpotPlacementResult spotPlacementResult = ExecuteScenarioAndGetSpotPlacement(
                    smoothPass,
                    scheduleProgrammes,
                    smoothProg,
                    breaksBeingSmoothed,
                    spotInfos,
                    clashesByExternalRef,
                    productsByExternalRef,
                    smoothScenarioExecutor,
                    smoothPassIteration,
                    spotPlacementResultsByScenarioId,
                    smoothScenario,
                    scenarioProgSmoothBreaks,
                    scenarioSpotIdsUsed,
                    validateSpotsAndGetSpotPlacement,
                    canSplitSpotsOverBreaks);

                if (spotPlacementResult.BestBreakResult?.SmoothBreak != null)
                {
                    if (bestSmoothScenario is null
                        || (smoothScenario.Priority < bestSmoothScenario.Priority))
                    {
                        bestSmoothScenario = smoothScenario;
                    }
                }
                else if (smoothScenario.Actions.Count == 0 && firstFailedPlaceSpotsResultForRule is null)
                {
                    // No best break, for default scenario then record first
                    // failed attempt so that if we don't find a break then we
                    // return details of this failure.
                    firstFailedPlaceSpotsResultForRule = spotPlacementResult.PlaceSpotsResult;
                }
            }

            return (bestSmoothScenario, firstFailedPlaceSpotsResultForRule);
        }

        private SpotPlacementResult ExecuteScenarioAndGetSpotPlacement(
            SmoothPassDefault smoothPass,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            SmoothScenarioExecutor smoothScenarioExecutor,
            SmoothPassDefaultIteration smoothPassIteration,
            Dictionary<Guid, SpotPlacementResult> spotPlacementResultsByScenarioId,
            SmoothScenario smoothScenario,
            List<SmoothBreak> scenarioProgSmoothBreaks,
            HashSet<Guid> scenarioSpotIdsUsed,
            List<Spot> validateSpotsAndGetSpotPlacement,
            bool canSplitSpotsOverBreaks)
        {
            // Execute scenario against spots/breaks to put the data in the
            // correct state. Don't log spot actions because it's a test scenario.
            smoothScenarioExecutor.Execute(
                smoothPass,
                smoothPassIteration.Sequence,
                smoothScenario,
                scenarioProgSmoothBreaks,
                scenarioSpotIdsUsed,
                logSpotAction: false);

            var validateAddSpotsToBreaksResults = ValidateAddSpotsToBreaks(
                validateSpotsAndGetSpotPlacement,
                smoothProg.Prog,
                smoothProg.SalesArea,
                scenarioProgSmoothBreaks,
                breaksBeingSmoothed,
                scheduleProgrammes,
                spotInfos,
                clashesByExternalRef,
                smoothPassIteration,
                canSplitSpotsOverBreaks: canSplitSpotsOverBreaks);

            // Determine the break to add the spot to using the specified rules
            var spotPlacementResult = GetSpotPlacementUsingBestBreakFactors(
                smoothPass,
                validateSpotsAndGetSpotPlacement,
                spotInfos,
                clashesByExternalRef,
                productsByExternalRef,
                scenarioProgSmoothBreaks,
                smoothPassIteration,
                validateAddSpotsToBreaksResults);

            // Store results, maintain best scenario, has lowest priority. We
            // can't use the scenario that has the best break with the highest
            // score because it would mean comparing scores calculated using
            // different factors.
            spotPlacementResultsByScenarioId.Add(smoothScenario.Id, spotPlacementResult);

            return spotPlacementResult;
        }

        private IReadOnlyCollection<SmoothScenario> GetSmoothScenariosForSpotsValidForTheBreak(
            IReadOnlyCollection<Programme> scheduleProgrammes,
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothScenarioCreator smoothScenarioCreator,
            SmoothPassDefaultIteration smoothPassIteration,
            bool canSplitSpotsOverBreaks)
        {
            // Validate adding spots to each break in the current state. This is
            // necessary so that we can identify scenarios that allow us to
            // place the spots in the ideal position. E.g. if insufficient time
            // in ideal break then we can create a scenario that moves other
            // spots from the break.
            var validateAddSpotsToBreaksResults = ValidateAddSpotsToBreaks(
                spotsForBreak,
                smoothProg.Prog,
                smoothProg.SalesArea,
                progSmoothBreaks,
                breaksBeingSmoothed,
                scheduleProgrammes,
                spotInfos,
                clashesByExternalRef,
                smoothPassIteration,
                canSplitSpotsOverBreaks: canSplitSpotsOverBreaks);

            return smoothScenarioCreator.GetSmoothScenarios(
                spotsForBreak,
                progSmoothBreaks,
                validateAddSpotsToBreaksResults,
                smoothPassIteration.BreakPositionRules,
                smoothPassIteration.RespectSpotTime);
        }

        /// <summary>
        /// Evaluate whether a spot could be added to a break. If it cannot, a
        /// failure is recorded.
        /// </summary>
        private List<SmoothFailureMessagesForSpotsCollection> ValidateAddSpotsToBreaks(
            IReadOnlyCollection<Spot> spotsForBreak,
            Programme programme,
            SalesArea salesArea,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            SmoothPassDefaultIteration smoothPassIteration,
            bool canSplitSpotsOverBreaks)
        {
            var results = new List<SmoothFailureMessagesForSpotsCollection>();
            int smoothBreaksCount = progSmoothBreaks.Count;

            foreach (var smoothBreak in progSmoothBreaks)
            {
                ICanAddSpot canAddSpotService = new CanAddSpotService(smoothBreak);

                var validationResults = AddSpotsToBreakValidatorService
                    .ValidateAddSpots(
                        smoothBreak,
                        programme,
                        salesArea,
                        spotsForBreak,
                        spotInfos,
                        smoothBreaksCount,
                        smoothPassIteration.ProductClashRules,
                        smoothPassIteration.RespectCampaignClash,
                        smoothPassIteration.RespectSpotTime,
                        smoothPassIteration.RespectRestrictions,
                        smoothPassIteration.RespectClashExceptions,
                        smoothPassIteration.BreakPositionRules,
                        smoothPassIteration.RequestedPositionInBreakRules,
                        clashesByExternalRef,
                        canSplitSpotsOverBreaks,
                        _smoothResources,
                        breaksBeingSmoothed,
                        scheduleProgrammes,
                        _clashExposureCountService,
                        _sponsorshipRestrictionService,
                        canAddSpotService);

                results.Add(validationResults);
            }

            return results;
        }

        /// <summary>
        /// Gets the best break to add the spots to using best break factors
        /// (scoring mechanism).
        /// </summary>
        /// <param name="smoothPass"></param>
        /// <param name="spotsForBreak"></param>
        /// <param name="firstSpotOnly"></param>
        /// <param name="spotInfos"></param>
        /// <param name="progSmoothBreaks"></param>
        /// <param name="smoothPassIteration"></param>
        /// <returns></returns>
        private SpotPlacementResult GetSpotPlacementUsingBestBreakFactors(
            SmoothPassDefault smoothPass,
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothPassDefaultIteration smoothPassIteration,
            IReadOnlyList<SmoothFailureMessagesForSpotsCollection> validateAddSpotsToBreakResults
            )
        {
            var spotPlacementResult = new SpotPlacementResult()
            {
                PlaceSpotsResult = new PlaceSpotsResult()
            };

            var validSmoothBreaks = new List<SmoothBreak>();
            var programmeSmoothBreaksForIndexing = progSmoothBreaks.ToList();

            // Check if spots can be added to this break
            foreach (var smoothBreak in progSmoothBreaks)
            {
                int breakIndex = programmeSmoothBreaksForIndexing.IndexOf(smoothBreak);

                var validateAddSpotsResults = validateAddSpotsToBreakResults[breakIndex];

                foreach (var spot in spotsForBreak)
                {
                    // Log results of validating if spots could be added to break
                    var reasonDescriptions = new StringBuilder("Results: ");

                    var validateAddSpotsResultsForSpot = validateAddSpotsResults[spot.Uid];

                    for (int failureIndex = 0; failureIndex < validateAddSpotsResultsForSpot.Failures.Count; failureIndex++)
                    {
                        var failure = validateAddSpotsResultsForSpot.Failures[failureIndex];

                        _ = reasonDescriptions.Append(failureIndex == 0 ? String.Empty : "; ");
                        _ = reasonDescriptions.Append(failure.FailureMessage.ToString());

                        if (failure.Restriction != null)
                        {
                            _ = reasonDescriptions.AppendFormat(
                                " (RestrictionID={0})",
                                failure.Restriction.Id
                                );
                        }
                    }

                    if (validateAddSpotsResults[spot.Uid].Failures.Count == 0)
                    {
                        _ = reasonDescriptions.Append("None");
                    }

                    _smoothDiagnostics.LogSpotAction(
                        smoothPass,
                        smoothPassIteration.Sequence,
                        spot,
                        smoothBreak,
                        SmoothSpot.SmoothSpotActions.ValidateAddSpotToBreak,
                        reasonDescriptions.ToString());
                }

                // Store results in unplaced results until we've decided which
                // break to use.
                int countFailureMessages = 0;

                foreach (var spot in spotsForBreak)
                {
                    countFailureMessages += validateAddSpotsResults[spot.Uid].Failures.Count;

                    var placeSpotResult = new PlaceSpotResult(
                        spot.Uid,
                        smoothBreak.TheBreak.ExternalBreakRef,
                        validateAddSpotsResults[spot.Uid]);

                    spotPlacementResult.PlaceSpotsResult
                        .UnplacedSpotResults.Add(placeSpotResult);
                }

                // If break suitable then add to list
                if (countFailureMessages == 0)
                {
                    validSmoothBreaks.Add(smoothBreak);
                }
            }

            // Determine the best break
            spotPlacementResult.CountValidBreaks = validSmoothBreaks.Count;
            if (validSmoothBreaks.Count == 1)     // Single break
            {
                spotPlacementResult.BestBreakResult = new BestBreakResult()
                {
                    SmoothBreak = validSmoothBreaks[0],
                    Score = 1000       // Dummy, high score so that we can compare scores
                };

                spotPlacementResult.PlaceSpotsResult.SmoothBreak = validSmoothBreaks[0];

                _smoothDiagnostics.LogBestBreakFactorMessage(
                    smoothPass,
                    smoothPassIteration,
                    null,
                    validSmoothBreaks[0].TheBreak,
                    spotsForBreak,
                    null,
                    "Only break that is valid");
            }
            else if (validSmoothBreaks.Count > 1)
            {
                // Multiple breaks Get break factor groups to use when
                // evaluating best break
                IReadOnlyCollection<BestBreakFactorGroup> bestBreakFactorGroups =
                    _smoothConfiguration.GetBestBreakFactorGroupsData(
                        spotsForBreak,
                        progSmoothBreaks,
                        smoothPass,
                        smoothPassIteration);

                // Get best break
                var bestBreakEvaluator = new BestBreakEvaluator(
                    _smoothDiagnostics,
                    _smoothResources,
                    _clashExposureCountService);

                BestBreakFactorGroup usedBestBreakFactorGroup = null;

                spotPlacementResult.BestBreakResult = bestBreakEvaluator.GetBestBreak(
                    smoothPass,
                    smoothPassIteration,
                    bestBreakFactorGroups,
                    validSmoothBreaks,
                    spotsForBreak,
                    spotInfos,
                    clashesByExternalRef,
                    productsByExternalRef,
                    progSmoothBreaks,
                    out usedBestBreakFactorGroup);

                if (usedBestBreakFactorGroup != null)
                {
                    spotPlacementResult.PlaceSpotsResult.BestBreakFactorGroupName = usedBestBreakFactorGroup.Name;
                }

                if (spotPlacementResult.BestBreakResult is null)   // No best break, use first one
                {
                    spotPlacementResult.BestBreakResult = new BestBreakResult
                    {
                        SmoothBreak = validSmoothBreaks[0],
                        Score = 1      // Dummy score so that we can compare scores
                    };
                }

                spotPlacementResult.PlaceSpotsResult.SmoothBreak = spotPlacementResult.BestBreakResult.SmoothBreak;
            }

            if (spotPlacementResult.PlaceSpotsResult.SmoothBreak != null)
            {
                var placedSpotResults = spotPlacementResult.PlaceSpotsResult.UnplacedSpotResults
                    .Where(r => r.ExternalBreakRef == spotPlacementResult.PlaceSpotsResult.SmoothBreak.TheBreak.ExternalBreakRef)
                    .ToList();

                RecordPlacedSpots(spotPlacementResult, placedSpotResults);
            }

            return spotPlacementResult;
        }

        private static void RecordPlacedSpots(
            SpotPlacementResult spotPlacementResult,
            IReadOnlyCollection<PlaceSpotResult> placedSpotResults)
        {
            foreach (var result in placedSpotResults)
            {
                _ = spotPlacementResult.PlaceSpotsResult.UnplacedSpotResults.Remove(result);
            }

            spotPlacementResult.PlaceSpotsResult.PlacedSpotResults.AddRange(placedSpotResults);
        }
    }
}
