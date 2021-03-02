using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Legacy;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.Extensions;
using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    internal class SmoothDateRange
    {
        private Action<string> RaiseInfo { get; }
        private Action<string> RaiseWarning { get; }
        public Action<string, Exception> RaiseException { get; }

        private readonly Guid _runId;
        private readonly Guid _firstScenarioId;
        private readonly DateTime _processorDateTime;
        private readonly SalesArea _salesArea;
        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly ImmutableSmoothData _threadSafeCollections;
        private readonly IClashExposureCountService _clashExposureCountService;
        private readonly SaveSmoothChanges _saveSmoothChanges;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly SmoothFailuresFactory _smoothFailuresFactory;
        private readonly SmoothRecommendationsFactory _smoothRecommendationsFactory;
        private readonly IImmutableList<SmoothPass> _smoothPasses;

        public SmoothDateRange(
            Guid runId,
            Guid firstScenarioId,
            DateTime processorDateTime,
            SalesArea salesArea,
            ISmoothConfiguration smoothConfiguration,
            ISmoothDiagnostics smoothDiagnostics,
            ImmutableSmoothData threadSafeCollections,
            IClashExposureCountService clashExposureCountService,
            SaveSmoothChanges saveSmoothChanges,
            IRepositoryFactory repositoryFactory,
            Action<string> raiseInfo,
            Action<string> raiseWarning,
            Action<string, Exception> raiseException)
        {
            RaiseInfo = raiseInfo;
            RaiseWarning = raiseWarning;
            RaiseException = raiseException;
            _threadSafeCollections = threadSafeCollections;
            _clashExposureCountService = clashExposureCountService;
            _saveSmoothChanges = saveSmoothChanges;
            _repositoryFactory = repositoryFactory;
            _runId = runId;
            _firstScenarioId = firstScenarioId;
            _processorDateTime = processorDateTime;
            _salesArea = salesArea;
            _smoothConfiguration = smoothConfiguration;
            _smoothDiagnostics = smoothDiagnostics;
            _smoothFailuresFactory = new SmoothFailuresFactory(_smoothConfiguration);
            _smoothRecommendationsFactory = new SmoothRecommendationsFactory(_smoothConfiguration);

            _smoothPasses = _smoothConfiguration.SortedSmoothPasses;
        }

        public (SmoothBatchOutput smoothBatchOutput, List<Recommendation> recommendations, List<SmoothFailure> smoothFailures)
        Execute(DateTimeRange weekBeingSmoothed)
        {
            string salesAreaName = _salesArea.Name;

            string auditMessageForSalesAreaNameAndBatchStartEndDate =
                $"for sales area {salesAreaName} ({Log(weekBeingSmoothed)})";

            RaiseInfoAndDebug($"Smoothing batch {auditMessageForSalesAreaNameAndBatchStartEndDate}");

            var batchThreadOutput = new SmoothBatchOutput();

            foreach (var item in PrepareSmoothFailureMessageCollection(_threadSafeCollections.SmoothFailureMessages))
            {
                batchThreadOutput.SpotsByFailureMessage.Add(item);
            }

            foreach (var item in PrepareSmoothOutputWithSmoothPasses(_smoothPasses))
            {
                batchThreadOutput.OutputByPass.Add(item);
            }

            int countBreaksWithPreviousSpots = 0;
            int countBreaksWithNoPreviousSpots = 0;

            var programmes = new List<Programme>();
            var allSpots = new List<Spot>();
            var spots = new List<Spot>();

            IImmutableList<Break> breaksForTheWeekBeingSmoothed = ImmutableList<Break>.Empty;
            IImmutableList<RatingsPredictionSchedule> ratingsPredictionSchedules = ImmutableList<RatingsPredictionSchedule>.Empty;

            IReadOnlyDictionary<string, SpotPlacement> spotPlacementsByExternalRef;

            try
            {
#pragma warning disable HAA0101 // Array allocation for params parameter
                Parallel.Invoke(
                    () => programmes.AddRange(LoadProgrammesToSmooth(weekBeingSmoothed, salesAreaName)),
                    () =>
                    {
                        var results = LoadSpotsToSmooth(weekBeingSmoothed, salesAreaName);
                        allSpots.AddRange(results.allSpots);
                        spots.AddRange(results.unbookedSpots);
                    },
                    () => ratingsPredictionSchedules = LoadRatingsPredictionSchedules(weekBeingSmoothed, salesAreaName),
                    () => breaksForTheWeekBeingSmoothed = LoadBreaksToSmooth(weekBeingSmoothed, salesAreaName)
                    );
#pragma warning restore HAA0101 // Array allocation for params parameter

                void RaiseInfoForCounts(string message) =>
                    RaiseInfo($"Read {message} {auditMessageForSalesAreaNameAndBatchStartEndDate}");

                RaiseInfoForCounts($"{Log(programmes.Count)} programmes");
                RaiseInfoForCounts($"{Log(allSpots.Count)} client picked spots");
                RaiseInfoForCounts($"{Log(spots.Count)} unbooked spots");
                RaiseInfoForCounts($"{Log(ratingsPredictionSchedules.Count)} ratings prediction schedules");
                RaiseInfoForCounts($"{Log(breaksForTheWeekBeingSmoothed.Count)} breaks");

                spotPlacementsByExternalRef = LoadSmoothPreviousSpotPlacements(allSpots);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    $"Error loading smooth data {auditMessageForSalesAreaNameAndBatchStartEndDate})",
                    exception
                    );
            }

            VerifyModel.VerifySpotProductsReferences(
                salesAreaName,
                allSpots,
                _threadSafeCollections.ProductsByExternalRef,
                _threadSafeCollections.ClashesByExternalRef,
                RaiseWarning
                );

            IReadOnlyDictionary<Guid, SpotInfo> spotInfos = SpotInfo.Factory(
                allSpots,
                _threadSafeCollections.ProductsByExternalRef,
                _threadSafeCollections.ClashesByExternalRef
                );

            // Default all spots to no placement attempt, will reduce
            // the list as we attempt, generate Smooth failures at the
            // end for any where no attempt was made to place it (E.g.
            // No breaks or progs)
            SpotPlacementService.MarkSpotsAsNoPlacementAttempted(spots, spotInfos);

            Dictionary<string, Break> breaksByExternalRef =
                Break.IndexListByExternalId(breaksForTheWeekBeingSmoothed);

            var filterService = new SponsorshipRestrictionFilterService(
                    _threadSafeCollections.SponsorshipRestrictions);

            IReadOnlyList<SmoothSponsorshipTimeline> timelines = filterService
                .GetSponsorshipRestrictionTimeline(
                    weekBeingSmoothed,
                    salesAreaName);

            ISmoothSponsorshipTimelineManager timelineManager = new SmoothSponsorshipTimelineManager(timelines);
            timelineManager.SetupTimelineRunningTotals(breaksForTheWeekBeingSmoothed);

            var smoothFailures = new List<SmoothFailure>();
            var recommendations = new List<Recommendation>();
            var spotIdsUsedForBatch = new HashSet<Guid>();

            foreach (Programme oneProgramme in programmes.OrderBy(p => p.StartDateTime))
            {
                var programmeStartEnd = GetProgrammeStartEnd(oneProgramme);
                var programmeBreaks = GetProgrammeBreaks(programmeStartEnd, breaksForTheWeekBeingSmoothed);
                var programmeSpots = GetProgrammeSpots(programmeStartEnd, allSpots);

                // The running total and maximums allowed will add up
                // within the service as we can't access the variables
                // outside of the programme loop from inside the event
                // handlers (they're in a different scope).
                SponsorshipRestrictionService sponsorshipRestrictionService =
                    SponsorshipRestrictionServiceFactory(
                        spotInfos,
                        filterService,
                        timelineManager,
                        oneProgramme);

                var smoothOneProgramme = new SmoothOneProgramme(
                    _runId,
                    _salesArea,
                    _processorDateTime,
                    _smoothDiagnostics,
                    ratingsPredictionSchedules,
                    _threadSafeCollections,
                    _clashExposureCountService,
                    sponsorshipRestrictionService,
                    RaiseInfo,
                    RaiseException
                    );

                var smoothProgramme = smoothOneProgramme.StartSmoothingProgramme(
                    oneProgramme,
                    batchThreadOutput,
                    programmeBreaks,
                    programmeSpots,
                    spotInfos,
                    _smoothPasses,
                    breaksByExternalRef,
                    spotPlacementsByExternalRef,
                    breaksForTheWeekBeingSmoothed,
                    programmes,
                    spotIdsUsedForBatch);

                countBreaksWithPreviousSpots += smoothProgramme.BreaksWithPreviousSpots;
                countBreaksWithNoPreviousSpots += smoothProgramme.BreaksWithoutPreviousSpots;

                _saveSmoothChanges.SaveSmoothedSpots(smoothProgramme.SpotsToBatchSave);

                smoothFailures.AddRange(smoothProgramme.SmoothFailures);
                recommendations.AddRange(smoothProgramme.Recommendations);

                bool SpotNotSetDueToExternalCampaignRef(Spot s) =>
                    _smoothConfiguration.ExternalCampaignRefsToExclude.Contains(s.ExternalCampaignNumber)
                    && !s.IsBooked();

                batchThreadOutput.SpotsNotSetDueToExternalCampaignRef +=
                    programmeSpots.Count(SpotNotSetDueToExternalCampaignRef);

                // Copy the final values of the sponsorship restriction
                // calculations and running totals into the outer
                // variables so the next iteration can use them. This
                // seems to be the only way to do this at the minute,
                // until I think of something else.
                timelineManager = sponsorshipRestrictionService.TimelineManager;
            } // End of foreach programme loop

            string batchStartEndDateForLogging = Log(weekBeingSmoothed);

            var recommendationsForUnplacedSpots = _smoothRecommendationsFactory
                .CreateRecommendationsForUnplacedSpots(
                    allSpots,
                    _salesArea,
                    _processorDateTime
                    );

            RaiseInfo(
                $"Created {Log(recommendationsForUnplacedSpots.Count)} recommendations " +
                $"for unplaced spots {auditMessageForSalesAreaNameAndBatchStartEndDate}");

            recommendations.AddRange(recommendationsForUnplacedSpots);
            batchThreadOutput.Recommendations += recommendations.Count;

            _saveSmoothChanges.SaveSpotPlacements(
                _processorDateTime,
                allSpots,
                spotPlacementsByExternalRef,
                spotInfos,
                batchStartEndDateForLogging
                );

            _saveSmoothChanges.SaveSmoothFailures(
                _runId,
                salesAreaName,
                _smoothFailuresFactory,
                smoothFailures,
                spots,
                spotInfos,
                batchStartEndDateForLogging
                );

            UpdateSmoothOutputForSpotsByFailureMessage(
                batchThreadOutput.SpotsByFailureMessage,
                smoothFailures,
                batchStartEndDateForLogging
                );

            batchThreadOutput.Failures += smoothFailures.Count;

            var unusedSpotIdsForBatch = new HashSet<Guid>();
            AddUnusedSpotsToUnusedSpotsCollection(spotIdsUsedForBatch, unusedSpotIdsForBatch, allSpots);

            spotIdsUsedForBatch.CopyDistinctTo(batchThreadOutput.UsedSpotIds);
            unusedSpotIdsForBatch.CopyDistinctTo(batchThreadOutput.UnusedSpotIds);

            _saveSmoothChanges.SaveSmoothRecommendations(
                _firstScenarioId,
                auditMessageForSalesAreaNameAndBatchStartEndDate,
                recommendations
                );

            RaiseInfo(
                $"Smoothed batch {auditMessageForSalesAreaNameAndBatchStartEndDate}: " +
                $"Breaks with no prev spots={countBreaksWithNoPreviousSpots.ToString()}, " +
                $"Breaks with prev spots={countBreaksWithPreviousSpots.ToString()})"
                );

            return (batchThreadOutput, recommendations, smoothFailures);
        }

        private void RaiseInfoAndDebug(string message)
        {
            RaiseInfo(message);
            Debug.WriteLine(message);
        }

        private IDictionary<int, int> PrepareSmoothFailureMessageCollection(
            IImmutableList<SmoothFailureMessage> smoothFailureMessages)
        {
            var result = new Dictionary<int, int>();

            foreach (var message in smoothFailureMessages)
            {
                result.Add(message.Id, 0);
            }

            return result;
        }

        private static IDictionary<int, SmoothOutputForPass> PrepareSmoothOutputWithSmoothPasses(
            IReadOnlyCollection<SmoothPass> smoothPasses)
        {
            IDictionary<int, SmoothOutputForPass> outputByPass = new Dictionary<int, SmoothOutputForPass>();

            foreach (var smoothPass in smoothPasses)
            {
                if (outputByPass.ContainsKey(smoothPass.Sequence))
                {
                    continue;
                }

                outputByPass.Add(
                    smoothPass.Sequence,
                    new SmoothOutputForPass { PassSequence = smoothPass.Sequence }
                    );
            }

            return outputByPass;
        }

        private ImmutableList<Break> LoadBreaksToSmooth(
            DateTimeRange dateTimeRange,
            string salesAreaName)
        {
            return _threadSafeCollections.BreaksForAllSalesAreasForSmoothPeriod
                .Where(BreakIsInSalesAreaAndDateRange)
                .ToImmutableList();

            // Local function
            bool BreakIsInSalesAreaAndDateRange(Break aBreak) =>
                aBreak.SalesArea.Equals(salesAreaName, StringComparison.InvariantCultureIgnoreCase)
                && dateTimeRange.Contains(aBreak.ScheduledDate);
        }

        // Cannot put this collection into the immutable lookups as there's
        // currently no repository method to get all by date and a list of sales areas.
        private IReadOnlyCollection<Programme> LoadProgrammesToSmooth(
            DateTimeRange dateTimeRange,
            string salesAreaName)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = scope.CreateRepository<IProgrammeRepository>();

            return repo
                .Search(dateTimeRange.Start, dateTimeRange.End, salesAreaName)
                .ToList();
        }

        private (IReadOnlyCollection<Spot> allSpots, IReadOnlyCollection<Spot> unbookedSpots) LoadSpotsToSmooth(
            DateTimeRange dateTimeRange,
            string salesAreaName)
        {
            var (dateFrom, dateTo) = dateTimeRange;

            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repo = scope.CreateRepository<ISpotRepository>();

            var allClientPickedSpots = repo
                .Search(dateFrom, dateTo, salesAreaName)
                .Where(s => s.ClientPicked)
                .ToList();

            var unbookedSpots = allClientPickedSpots
                .Where(s => !s.IsBooked())
                .ToList();

            return (allClientPickedSpots, unbookedSpots);
        }

        private IReadOnlyDictionary<string, SpotPlacement> LoadSmoothPreviousSpotPlacements(
            IReadOnlyCollection<Spot> allSpots)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var spotPlacementRepository = scope.CreateRepository<ISpotPlacementRepository>();

            return GetSpotPlacementsByExternalRef(allSpots, spotPlacementRepository);
        }

        // Cannot put this collection into the immutable lookups as there's
        // currently no repository method to get all by date and a list of sales areas.
        private ImmutableList<RatingsPredictionSchedule> LoadRatingsPredictionSchedules(
            DateTimeRange dateTimeRange,
            string salesAreaName
            )
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var ratingsScheduleRepository = scope.CreateRepository<IRatingsScheduleRepository>();

            return ratingsScheduleRepository
                .GetSchedules(dateTimeRange.Start, dateTimeRange.End, salesAreaName)
                .ToImmutableList();
        }

        /// <summary>
        /// Returns previous SpotPlacement for each spot
        /// </summary>
        /// <param name="allSpots"></param>
        /// <param name="spotPlacementRepository"></param>
        /// <returns></returns>
        private static IReadOnlyDictionary<string, SpotPlacement> GetSpotPlacementsByExternalRef(
            IReadOnlyCollection<Spot> allSpots,
            ISpotPlacementRepository spotPlacementRepository
            )
        {
            var previousSpotPlacementsByExternalRef = new Dictionary<string, SpotPlacement>();

            IEnumerable<SpotPlacement> spotPlacements = spotPlacementRepository.GetByExternalSpotRefs(
                allSpots
                    .Select(s => s.ExternalSpotRef)
                    .Distinct()
                );

            foreach (var previousSpotPlacement in spotPlacements)
            {
                if (previousSpotPlacementsByExternalRef.ContainsKey(previousSpotPlacement.ExternalSpotRef))
                {
                    continue;
                }

                previousSpotPlacementsByExternalRef.Add(
                    previousSpotPlacement.ExternalSpotRef, previousSpotPlacement
                    );
            }

            return previousSpotPlacementsByExternalRef;
        }

        private static DateTimeRange GetProgrammeStartEnd(Programme oneProgramme)
        {
            DateTime programmeStart = oneProgramme.StartDateTime;
            DateTime programmeEnd = programmeStart.Add(oneProgramme.Duration.ToTimeSpan());

            return (programmeStart, programmeEnd);
        }

        private static IReadOnlyCollection<Spot> GetProgrammeSpots(
            DateTimeRange programmeStartEnd,
            IReadOnlyCollection<Spot> spots) =>
            ProgrammeSpecificDataSet.GetForPeriod<Spot>(programmeStartEnd, spots);

        private static IReadOnlyCollection<Break> GetProgrammeBreaks(
            DateTimeRange programmeStartEnd,
            IReadOnlyCollection<Break> breaks) =>
            ProgrammeSpecificDataSet.GetForPeriod<Break>(programmeStartEnd, breaks);

        private SponsorshipRestrictionService SponsorshipRestrictionServiceFactory(
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            SponsorshipRestrictionFilterService filterService,
            ISmoothSponsorshipTimelineManager timelineManager,
            Programme oneProgramme)
        {
            SponsorshipRestrictionService sponsorshipRestrictionService =
                CreateSponsorshipRestrictionService(
                    spotInfos,
                    filterService,
                    timelineManager,
                    oneProgramme,
                    RaiseException);

            // Subscribe to the service's events
            sponsorshipRestrictionService.RaiseAddedSpotToBreakEvent += (source, e) =>
            {
                if (!(source is SponsorshipRestrictionService service))
                {
                    return;
                }

                try
                {
                    SmoothSponsorshipTimeline timeline = e.IsSponsor
                        ? timelineManager.FindTimelineForSponsoredProduct(
                            e.BreakExternalReference,
                            e.ProductExternalReference)
                        : timelineManager.FindTimelineForCompetitor(
                            e.BreakExternalReference,
                            e.ProductAdvertiserIdentifier,
                            e.ProductClashCode);

                    AddSpotToRunningTotal(timeline.RunningTotals, e);

                    timeline.RestrictionLimits = SponsorshipLimitsCalculator
                        .CalculateRestrictionLimits(
                            timeline.RunningTotals,
                            e.CalculationType,
                            e.Applicability);
                }
                catch (Exception ex)
                {
                    RaiseException("Unable to find a sponsorship timeline after " +
                        $"adding {(e.IsSponsor ? "sponsored" : "competitor")} " +
                        $"spot with product {e.ProductExternalReference.ToString()} into " +
                        $"break {e.BreakExternalReference.ToString()}",
                        ex);
                }
            };

            // Subscribe to the service's events
            sponsorshipRestrictionService.RaiseRemovedSpotFromBreakEvent += (source, e) =>
            {
                if (!(source is SponsorshipRestrictionService service))
                {
                    return;
                }

                try
                {
                    SmoothSponsorshipTimeline timeline = e.IsSponsor
                        ? timelineManager.FindTimelineForSponsoredProduct(
                            e.BreakExternalReference,
                            e.ProductExternalReference)
                        : timelineManager.FindTimelineForCompetitor(
                            e.BreakExternalReference,
                            e.ProductAdvertiserIdentifier,
                            e.ProductClashCode);

                    RemoveSpotFromRunningTotal(timeline.RunningTotals, e);

                    timeline.RestrictionLimits = SponsorshipLimitsCalculator
                        .CalculateRestrictionLimits(
                            timeline.RunningTotals,
                            e.CalculationType,
                            e.Applicability);
                }
                catch (Exception ex)
                {
                    RaiseException("Unable to find a sponsorship timeline after " +
                        $"removing {(e.IsSponsor ? "sponsored" : "competitor")} " +
                        $"spot with product {e.ProductExternalReference.ToString()} into " +
                        $"break {e.BreakExternalReference.ToString()}",
                        ex);
                }
            };

            return sponsorshipRestrictionService;
        }

        /// <summary>
        /// Might be possible to move this to the service itself as a factory method.
        /// </summary>
        /// <param name="spotInfos"></param>
        /// <param name="filterService"></param>
        /// <param name="sponsorshipRunningTotals"></param>
        /// <param name="oneProgramme"></param>
        /// <returns></returns>
        private static SponsorshipRestrictionService CreateSponsorshipRestrictionService(
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            SponsorshipRestrictionFilterService filterService,
            ISmoothSponsorshipTimelineManager timelineManager,
            Programme oneProgramme,
            Action<string, Exception> raiseException)
        {
            var programmeSponsorshipRestrictions = GetProgrammeSponsorshipRestrictions(
                filterService,
                oneProgramme
                );

            return new SponsorshipRestrictionService(
                programmeSponsorshipRestrictions,
                timelineManager,
                spotInfos,
                raiseException);
        }

        /// <summary>
        /// Filters a list of sponsorship restrictions for a specific programme.
        /// </summary>
        /// <param name="allSponsorshipRestrictions"></param>
        /// <param name="oneProgramme"></param>
        /// <returns></returns>
        private static IImmutableList<SponsorshipRestrictionFilterResults>
        GetProgrammeSponsorshipRestrictions(
            SponsorshipRestrictionFilterService sponsorshipRestrictionFilterService,
            Programme oneProgramme
            ) => sponsorshipRestrictionFilterService.Filter(oneProgramme);

        private void RemoveSpotFromRunningTotal(
            SmoothSponsorshipRunningTotals sponsorshipRunningTotals,
            RemovedSpotFromBreakEventArgs e)
        {
            if (e.IsSponsor)
            {
                RemoveSponsoredSpot();
            }
            else
            {
                RemoveCompetitorSpot();
            }

            void RemoveSponsoredSpot()
            {
                if (e.RestrictionType.Type == SponsorshipRestrictionType.SpotDuration)
                {
                    sponsorshipRunningTotals
                        .RemoveSponsoredProductToSpotByDuration(
                        e.ProductExternalReference,
                        e.RestrictionType.Duration);
                }
                else
                {
                    sponsorshipRunningTotals
                        .RemoveSponsoredProductToSpotByCount(
                        e.ProductExternalReference,
                        1);
                }
            }

            void RemoveCompetitorSpot()
            {
                if (e.RestrictionType.Type == SponsorshipRestrictionType.SpotDuration)
                {
                    sponsorshipRunningTotals
                        .RemoveCompetitorToSpotByDuration(
                        e.ProductExternalReference,
                        e.RestrictionType.Duration);
                }
                else
                {
                    sponsorshipRunningTotals
                        .RemoveCompetitorToSpotByCount(
                        e.ProductExternalReference,
                        1);
                }
            }
        }

        private static void AddSpotToRunningTotal(
            SmoothSponsorshipRunningTotals sponsorshipLimitations,
            AddedSpotToBreakEventArgs e)
        {
            if (e.IsSponsor)
            {
                AddSponsoredSpot();
            }
            else
            {
                sponsorshipLimitations.AddCompetitorToAdvertiserIdentifier(
                    e.ProductExternalReference,
                    e.ProductAdvertiserIdentifier);
                sponsorshipLimitations.AddCompetitorToClashCode(
                    e.ProductExternalReference,
                    e.ProductClashCode);

                AddCompetitorSpot();
            }

            void AddCompetitorSpot()
            {
                if (e.RestrictionType.type == SponsorshipRestrictionType.SpotDuration)
                {
                    sponsorshipLimitations.AddCompetitorToSpotByDuration(
                    e.ProductExternalReference,
                    e.RestrictionType.duration);
                }
                else
                {
                    sponsorshipLimitations.AddCompetitorToSpotByCount(
                    e.ProductExternalReference,
                    1);
                }
            }

            void AddSponsoredSpot()
            {
                if (e.RestrictionType.type == SponsorshipRestrictionType.SpotDuration)
                {
                    sponsorshipLimitations.AddSponsoredProductToSpotByDuration(
                    e.ProductExternalReference,
                    e.RestrictionType.duration);
                }
                else
                {
                    sponsorshipLimitations.AddSponsoredProductToSpotByCount(
                    e.ProductExternalReference,
                    1);
                }
            }
        }

        private static void AddUnusedSpotsToUnusedSpotsCollection(
            IReadOnlyCollection<Guid> spotIdsUsed,
            HashSet<Guid> spotIdsNotUsed,
            IReadOnlyCollection<Spot> allSpots)
        {
            foreach (var spot in allSpots)
            {
                if (spotIdsUsed.Contains(spot.Uid) || spot.IsBooked())
                {
                    continue;
                }

                SpotPlacementService.FlagSpotAsNotUsed(spot, spotIdsNotUsed);
            }

            // Sanity check
            _ = spotIdsNotUsed.RemoveWhere(spotIdsUsed.Contains);
        }

        private static void UpdateSmoothOutputForSpotsByFailureMessage(
            IDictionary<int, int> spotsByfailureMessage,
            IReadOnlyCollection<SmoothFailure> smoothFailures,
            string batchStartEndDateForLogging)
        {
            if (spotsByfailureMessage.Count == 0)
            {
                return;
            }

            if (smoothFailures.Count == 0)
            {
                return;
            }

            var spotRefAndMessageIds = new HashSet<string>();

            foreach (SmoothFailure smoothFailure in smoothFailures)
            {
                if (smoothFailure?.MessageIds is null)
                {
                    continue;
                }

                try
                {
                    foreach (int messageId in smoothFailure.MessageIds)
                    {
                        string spotRefAndMessageId = $"{Log(messageId)}|{smoothFailure.ExternalSpotRef}";

                        if (spotRefAndMessageIds.Contains(spotRefAndMessageId) || !spotsByfailureMessage.ContainsKey(messageId))
                        {
                            continue;
                        }

                        spotsByfailureMessage[messageId]++;
                        _ = spotRefAndMessageIds.Add(spotRefAndMessageId);
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        $"Error counts spots by failure message for {batchStartEndDateForLogging}",
                        exception
                        );
                }
            }
        }
    }
}
