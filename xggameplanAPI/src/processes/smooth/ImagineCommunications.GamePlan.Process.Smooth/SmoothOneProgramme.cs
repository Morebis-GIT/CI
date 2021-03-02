using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth
{
    /// <summary>
    /// Create a new instance of this class for each programme to smooth.
    /// </summary>
    public class SmoothOneProgramme
    {
        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly SmoothPassBookedExecuter _smoothPassBookedExecuter;
        private readonly SmoothPassDefaultExecuter _smoothPassDefaultExecuter;
        private readonly SmoothPassUnplacedExecuter _smoothPassUnplacedExecuter;
        private readonly ImmutableSmoothData _threadSafeCollections;
        private readonly SponsorshipRestrictionService _sponsorshipRestrictionService;
        private readonly SmoothFailuresFactory _smoothFailuresFactory;
        private readonly SmoothRecommendationsFactory _smoothRecommendationsFactory;
        private readonly DateTime _processorDateTime;
        private readonly SmoothProgramme _smoothProgramme;
        private readonly IReadOnlyCollection<Spot> _programmeSpots;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;
        private readonly Guid _runId;

        private readonly SmoothResources _smoothResources = new SmoothResources();

        private Action<string> RaiseInfo { get; }
        private Action<string, Exception> RaiseException { get; }

        private Action<string> RaiseDebug { get; } = (msg) => Debug.WriteLine(msg);

        /// <summary>
        /// Create a new instance of this class for each programme to smooth.
        /// </summary>
        public SmoothOneProgramme(
            Programme programme,
            IEnumerable<Break> programmeBreaks,
            IReadOnlyCollection<Spot> programmeSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            Guid runId,
            SalesArea salesArea,
            DateTime processorDateTime,
            ISmoothDiagnostics smoothDiagnostics,
            IImmutableList<RatingsPredictionSchedule> ratingsPredictionSchedules,
            ImmutableSmoothData threadSafeCollections,
            IClashExposureCountService clashExposureCountService,
            SponsorshipRestrictionService sponsorshipRestrictionService,
            IReadOnlyCollection<Product> products,
            IReadOnlyCollection<Clash> clashes,
            IReadOnlyCollection<Programme> allProgrammesForPeriodAndSalesArea,
            Action<string> raiseInfo,
            Action<string, Exception> raiseException)
        {
            RaiseInfo = raiseInfo;
            RaiseException = raiseException;

            if (programme is null)
            {
                var guruMeditation = new ArgumentNullException(nameof(programme));
                RaiseException("The programme to Smooth is null", guruMeditation);

                throw guruMeditation;
            }

            _smoothProgramme = new SmoothProgramme(salesArea, programme);
            _smoothProgramme.InitialiseSmoothBreaks(programmeBreaks);

            _processorDateTime = processorDateTime;
            _programmeSpots = programmeSpots;
            _spotInfos = spotInfos;
            _runId = runId;
            _threadSafeCollections = threadSafeCollections;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;
            _smoothConfiguration = threadSafeCollections.SmoothConfigurationReader;
            _smoothDiagnostics = smoothDiagnostics;

            _smoothPassBookedExecuter = new SmoothPassBookedExecuter(
                _smoothDiagnostics,
                _smoothResources,
                _smoothProgramme,
                sponsorshipRestrictionService,
                allProgrammesForPeriodAndSalesArea,
                RaiseInfo,
                RaiseException);

            _smoothPassDefaultExecuter = new SmoothPassDefaultExecuter(
                _smoothDiagnostics,
                _smoothResources,
                _smoothProgramme,
                sponsorshipRestrictionService,
                allProgrammesForPeriodAndSalesArea,
                _smoothConfiguration,
                clashExposureCountService,
                _threadSafeCollections.ClashesByExternalRef,
                _threadSafeCollections.ProductsByExternalRef,
                RaiseException);

            _smoothPassUnplacedExecuter = new SmoothPassUnplacedExecuter(
                _smoothDiagnostics,
                _smoothResources,
                _smoothProgramme,
                sponsorshipRestrictionService,
                allProgrammesForPeriodAndSalesArea,
                _smoothConfiguration,
                clashExposureCountService,
                _threadSafeCollections.ClashesByExternalRef,
                RaiseException);

            if (_smoothConfiguration.ClashExceptionCheckEnabled)
            {
                _smoothResources.ClashExceptionChecker = new ClashExceptionChecker(
                    threadSafeCollections.ClashExceptions,
                    products,
                    clashes
                    );
            }

            if (_smoothConfiguration.RestrictionCheckEnabled)
            {
                _smoothResources.RestrictionChecker = new RestrictionChecker(
                    threadSafeCollections.Restrictions,
                    products,
                    clashes,
                    threadSafeCollections.IndexTypes,
                    threadSafeCollections.Universes,
                    ratingsPredictionSchedules);
            }

            _smoothFailuresFactory = new SmoothFailuresFactory(_smoothConfiguration);
            _smoothRecommendationsFactory = new SmoothRecommendationsFactory(_smoothConfiguration);
        }

        /// <summary>
        /// Smooth one programme.
        /// </summary>
        public SmoothProgramme Execute(
            SmoothBatchOutput smoothBatchOutput,
            IReadOnlyCollection<SmoothPass> smoothPasses,
            IReadOnlyDictionary<string, Break> breaksByExternalRef,
            IReadOnlyDictionary<string, SpotPlacement> previousSpotPlacementsByExternalRef,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            ISet<Guid> spotIdsUsed)
        {
            RaiseDebug(
                $"Smoothing programme ID [{Log(_smoothProgramme.Programme.Id)}] " +
                $"in sales area {_smoothProgramme.SalesAreaName}" +
                Ellipsis
                );

            int countSpotsUnplacedBefore = _programmeSpots.Count(s => !s.IsBooked());

            (
                _smoothProgramme.BreaksWithPreviousSpots,
                _smoothProgramme.BreaksWithoutPreviousSpots
            ) = InitialiseSmoothBreaksForThisProgramme(
                    smoothBatchOutput,
                    spotIdsUsed);

            // For the breaks in this programme, perform multiple passes from
            // highest to lowest. If no programme breaks then don't execute any
            // passes otherwise we'll generate 'Unplaced spot attempt'
            // (TypeID=1) Smooth failures which is wrong. We'll generate 'No
            // placing attempt' Smooth failures at the end.
            var progSpotsNotUsed = new List<Spot>();
            var smoothPassResults = new List<SmoothPassResult>();

            // We only attempt to place spots if there are breaks
            if (_smoothProgramme.ProgrammeSmoothBreaks.Count > 0)
            {
                ExecuteSmoothPasses(
                    smoothPasses,
                    breaksForThePeriodBeingSmoothed,
                    smoothBatchOutput,
                    progSpotsNotUsed,
                    smoothPassResults,
                    spotIdsUsed);
            }

            // For all spots that we attempted to place above (if we make at
            // least one pass then we 'attempt' to place every spot) remove them
            // from the list so that our final list contains only those spots
            // that we didn't make any attempt to place (E.g. No breaks or programmes).
            if (smoothPassResults.Count > 0)
            {
                SpotPlacementService.MarkSpotsAsPlacementWasAttempted(_programmeSpots, _spotInfos);
            }

            _smoothProgramme.SmoothFailures.AddRange(
                _smoothFailuresFactory.CreateSmoothFailuresForUnplacedSpots(
                    _runId,
                    _smoothProgramme.SalesAreaName,
                    smoothPasses,
                    smoothPassResults,
                    progSpotsNotUsed,
                    breaksByExternalRef,
                    previousSpotPlacementsByExternalRef,
                    _threadSafeCollections.CampaignsByExternalRef,
                    _threadSafeCollections.ClashesByExternalRef,
                    _threadSafeCollections.ProductsByExternalRef
                    )
                );

            _smoothProgramme.SmoothFailures.AddRange(
                _smoothFailuresFactory.CreateSmoothFailuresForPlacedSpots(
                    _runId,
                    _smoothProgramme.SalesAreaName,
                    _smoothProgramme.ProgrammeSmoothBreaks,
                    breaksByExternalRef,
                    previousSpotPlacementsByExternalRef,
                    _threadSafeCollections.CampaignsByExternalRef,
                    _threadSafeCollections.ClashesByExternalRef,
                    _threadSafeCollections.ProductsByExternalRef
                    )
                );

            IReadOnlyCollection<Spot> spotsToBatchSave = RenumberSpotPositionInBreak(
                smoothBatchOutput,
                spotIdsUsed);

            _smoothProgramme.SpotsToBatchSave.AddRange(spotsToBatchSave);

            var recommendationsForPlacedSpots = _smoothRecommendationsFactory.CreateRecommendationsForPlacedSpots(
                _smoothProgramme.ProgrammeSmoothBreaks,
                _smoothProgramme.Programme,
                _smoothProgramme.SalesArea,
                _processorDateTime
                );

            AddRecommendationsForPlacedSpotsToSmoothProgramme(
                recommendationsForPlacedSpots);

            int countSpotsUnplacedAfter = _programmeSpots.Count(s => !s.IsBooked());

            _smoothDiagnostics.LogPlacedSmoothSpots(
                _smoothProgramme.Programme,
                _smoothProgramme.ProgrammeSmoothBreaks);

            _smoothDiagnostics.LogProgramme(
                _smoothProgramme.Programme.Id,
                _smoothProgramme.ProgrammeSmoothBreaks,
                countSpotsUnplacedBefore,
                countSpotsUnplacedAfter
                );

            _smoothDiagnostics.Flush();

            return _smoothProgramme;
        }

        private void ExecuteSmoothPasses(
            IEnumerable<SmoothPass> smoothPasses,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            SmoothBatchOutput smoothBatchOutput,
            List<Spot> progSpotsNotUsed,
            List<SmoothPassResult> smoothPassResults,
            ISet<Guid> spotIdsUsed)
        {
            foreach (var smoothPass in smoothPasses)
            {
                switch (smoothPass)
                {
                    case SmoothPassBooked specificSmoothPass:
                        ExecuteBookedPass(
                            specificSmoothPass,
                            breaksForThePeriodBeingSmoothed,
                            spotIdsUsed,
                            smoothBatchOutput);

                        break;

                    case SmoothPassDefault specificSmoothPass:
                        ExecuteDefaultPass(
                            specificSmoothPass,
                            breaksForThePeriodBeingSmoothed,
                            spotIdsUsed,
                            smoothPassResults);

                        break;

                    case SmoothPassUnplaced specificSmoothPass:
                        ExecuteUnplacedPass(
                            specificSmoothPass,
                            breaksForThePeriodBeingSmoothed,
                            spotIdsUsed,
                            smoothBatchOutput,
                            progSpotsNotUsed,
                            smoothPassResults);

                        break;
                }
            }
        }

        /// <summary>
        /// A pass for Booked spots. Don't store results, no spots placed.
        /// </summary>
        private void ExecuteBookedPass(
            SmoothPassBooked specificSmoothPass,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            ISet<Guid> spotIdsUsed,
            SmoothBatchOutput smoothBatchOutput)
        {
            SmoothPassResult result = _smoothPassBookedExecuter.Execute(
                specificSmoothPass,
                breaksForThePeriodBeingSmoothed,
                spotIdsUsed);

            smoothBatchOutput.BookedSpotsUnplacedDueToRestrictions +=
                result.BookedSpotIdsUnplacedDueToRestrictions.Count;
        }

        private void ExecuteDefaultPass(
            SmoothPassDefault specificSmoothPass,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            ISet<Guid> spotIdsUsed,
            List<SmoothPassResult> smoothPassResults)
        {
            SmoothPassResult result = _smoothPassDefaultExecuter.Execute(
                specificSmoothPass,
                breaksForThePeriodBeingSmoothed,
                spotIdsUsed,
                _programmeSpots,
                _spotInfos);

            smoothPassResults.Add(result);
        }

        /// <summary>
        /// Try and place unplaced spots, spots where there
        /// wasn't sufficient time in any break.
        /// </summary>
        private void ExecuteUnplacedPass(
            SmoothPassUnplaced specificSmoothPass,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            ISet<Guid> spotIdsUsed,
            SmoothBatchOutput smoothBatchOutput,
            List<Spot> progSpotsNotUsed,
            List<SmoothPassResult> smoothPassResults)
        {
            progSpotsNotUsed.Clear();

            var unbookedSpots = GetUnbookedSpots(_programmeSpots, spotIdsUsed);
            if (!unbookedSpots.Any())
            {
                return;
            }

            progSpotsNotUsed.AddRange(unbookedSpots);

            SmoothPassResult result = _smoothPassUnplacedExecuter.Execute(
                specificSmoothPass,
                breaksForThePeriodBeingSmoothed,
                spotIdsUsed,
                progSpotsNotUsed,
                _spotInfos);

            smoothPassResults.Add(result);

            if (result.CountPlacedSpots == 0)
            {
                return;
            }

            smoothBatchOutput.SpotsSetAfterMovingOtherSpots += result.CountPlacedSpots;

            // Update list of unplaced spots so that we can calculate
            // break avail adjustment. We placed previously unplaced
            // spots after moving other spots about.
            unbookedSpots = GetUnbookedSpots(_programmeSpots, spotIdsUsed);

            progSpotsNotUsed.Clear();
            progSpotsNotUsed.AddRange(unbookedSpots);

            // Local function
            // Note: passing in the programme spots avoids a capture of the
            // parent method's "this" object.
            static IEnumerable<Spot> GetUnbookedSpots(
                IReadOnlyCollection<Spot> programmeSpots,
                ISet<Guid> spotIdsUsed
                ) => programmeSpots.Where(s => !s.IsBooked() && !spotIdsUsed.Contains(s.Uid));
        }

        private (int countBreaksWithPreviousSpots, int countBreaksWithNoPreviousSpots)
        InitialiseSmoothBreaksForThisProgramme(
            SmoothBatchOutput smoothBatchOutput,
            ISet<Guid> spotIdsUsed)
        {
            int breakCount = _smoothProgramme.ProgrammeSmoothBreaks.Count;
            if (breakCount > 0)
            {
                smoothBatchOutput.Breaks += breakCount;
            }

            int countBreaksWithPreviousSpots = 0;
            int countBreaksWithNoPreviousSpots = 0;

            foreach (var smoothBreak in _smoothProgramme.ProgrammeSmoothBreaks)
            {
                IReadOnlyCollection<Spot> bookedSpotsForBreak =
                    GetSpotsPreviouslyBookedIntoBreakWithExternalReference(
                        _programmeSpots,
                        smoothBreak.TheBreak.ExternalBreakRef);

                if (bookedSpotsForBreak.Count == 0)
                {
                    countBreaksWithNoPreviousSpots++;
                    continue;
                }

                countBreaksWithPreviousSpots++;

                _ = SpotPlacementService.AddBookedSpotsToBreak(
                    smoothBreak,
                    bookedSpotsForBreak,
                    _spotInfos,
                    spotIdsUsed,
                    _sponsorshipRestrictionService
                    );

                foreach (var spot in bookedSpotsForBreak)
                {
                    smoothBreak.RemainingAvailability += spot.SpotLength;
                }

                LogSpotActionForBookedSpots(smoothBreak, bookedSpotsForBreak);
            }

            return (countBreaksWithPreviousSpots, countBreaksWithNoPreviousSpots);
        }

        private static IReadOnlyCollection<Spot> GetSpotsPreviouslyBookedIntoBreakWithExternalReference(
            IReadOnlyCollection<Spot> spots,
            BreakExternalReference value
            )
        {
            var breakExRef = value.ToString();

            return spots.Where(s =>
                !String.IsNullOrEmpty(s.ExternalBreakNo) &&
                s.ExternalBreakNo.Equals(breakExRef, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        private void LogSpotActionForBookedSpots(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> bookedSpotsForBreak)
        {
            foreach (var bookedSpot in bookedSpotsForBreak)
            {
                _smoothDiagnostics.LogSpotAction(
                    null,
                    0,
                    bookedSpot,
                    smoothBreak,
                    SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                    "Booked spot"
                    );
            }
        }

        /// <summary>Renumbers the spot position in break.</summary>
        /// <param name="smoothOutput">The smooth output.</param>
        /// <param name="spotIdsUsed">The spot ids used.</param>
        /// <remarks>Are the programme breaks the same as the breaks in the smooth programme?</remarks>
        private IReadOnlyCollection<Spot> RenumberSpotPositionInBreak(
            SmoothBatchOutput smoothOutput,
            ISet<Guid> spotIdsUsed)
        {
            var spotsToBatchSave = new List<Spot>();

            foreach (SmoothBreak smoothBreak in _smoothProgramme.ProgrammeSmoothBreaks)
            {
                smoothBreak.RenumberActualPositionOfSpotInBreak();

                foreach (SmoothSpot smoothSpot in smoothBreak.SmoothSpots)
                {
                    UpdateStatisticsForPass(smoothOutput, smoothSpot);
                    SpotPlacementService.FlagSpotAsUsed(smoothSpot.Spot, spotIdsUsed);

                    if (!smoothSpot.IsCurrent)
                    {
                        continue;
                    }

                    spotsToBatchSave.Add(smoothSpot.Spot);
                    smoothOutput.SpotsSet++;
                }
            }

            return spotsToBatchSave;

            // --------------- Local functions
            static void UpdateStatisticsForPass(SmoothBatchOutput output, SmoothSpot spot)
            {
                if (output.OutputByPass.ContainsKey(spot.SmoothPassSequence))
                {
                    output.OutputByPass[spot.SmoothPassSequence].CountSpotsSet++;
                }
            }
        }

        private void AddRecommendationsForPlacedSpotsToSmoothProgramme(
            IReadOnlyCollection<Recommendation> recommendationsForPlacedSpots)
        {
            if (recommendationsForPlacedSpots.Count == 0)
            {
                return;
            }

            RaiseInfo(
                $"Created {Log(recommendationsForPlacedSpots.Count)} recommendations for "
                + $"placed spots in sales area {_smoothProgramme.SalesAreaName} "
                + $"for programme {Log(_smoothProgramme.Programme.Id)}"
                );

            _smoothProgramme.Recommendations.AddRange(recommendationsForPlacedSpots);
        }
    }
}
