using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Legacy;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class SmoothOneProgramme
    {
        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly SmoothPassBookedExecuter _smoothPassBookedExecuter;
        private readonly SmoothPassDefaultExecuter _smoothPassDefaultExecuter;
        private readonly SmoothPassUnplacedExecuter _smoothPassUnplacedExecuter;
        private readonly ImmutableSmoothData _threadSafeCollections;
        private readonly SponsorshipRestrictionService _sponsorshipRestrictionService;
        private readonly SmoothResources _smoothResources = new SmoothResources();
        private readonly SmoothFailuresFactory _smoothFailuresFactory;
        private readonly SmoothRecommendationsFactory _smoothRecommendationsFactory;

        private readonly DateTime _processorDateTime;
        private readonly Guid _runId;
        private readonly SalesArea _salesArea;

        private Action<string> RaiseInfo { get; }
        private Action<string, Exception> RaiseException { get; }

        private Action<string> RaiseDebug { get; } = (msg) => Debug.WriteLine(msg);

        public SmoothOneProgramme(
            Guid runId,
            SalesArea salesArea,
            DateTime processorDateTime,
            ISmoothDiagnostics smoothDiagnostics,
            IImmutableList<RatingsPredictionSchedule> ratingsPredictionSchedules,
            ImmutableSmoothData threadSafeCollections,
            IClashExposureCountService clashExposureCountService,
            SponsorshipRestrictionService sponsorshipRestrictionService,
            Action<string> raiseInfo,
            Action<string, Exception> raiseException
            )
        {
            RaiseInfo = raiseInfo;
            RaiseException = raiseException;

            _processorDateTime = processorDateTime;
            _runId = runId;
            _salesArea = salesArea;

            _threadSafeCollections = threadSafeCollections;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;
            _smoothConfiguration = threadSafeCollections.SmoothConfigurationReader;
            _smoothDiagnostics = smoothDiagnostics;

            _smoothPassBookedExecuter = new SmoothPassBookedExecuter(
                _smoothDiagnostics,
                _smoothResources,
                sponsorshipRestrictionService,
                RaiseInfo,
                RaiseException);

            _smoothPassDefaultExecuter = new SmoothPassDefaultExecuter(
                _smoothDiagnostics,
                _smoothResources,
                _smoothConfiguration,
                clashExposureCountService,
                sponsorshipRestrictionService,
                RaiseException);

            _smoothPassUnplacedExecuter = new SmoothPassUnplacedExecuter(
                _smoothDiagnostics,
                _smoothResources,
                _smoothConfiguration,
                clashExposureCountService,
                sponsorshipRestrictionService,
                RaiseException);

            InitialiseBeforeSmoothProgrammes(
                threadSafeCollections.ClashExceptions,
                threadSafeCollections.Products,
                threadSafeCollections.Clashes,
                threadSafeCollections.Restrictions,
                threadSafeCollections.IndexTypes,
                threadSafeCollections.Universes,
                ratingsPredictionSchedules
                );

            _smoothFailuresFactory = new SmoothFailuresFactory(_smoothConfiguration);
            _smoothRecommendationsFactory = new SmoothRecommendationsFactory(_smoothConfiguration);
        }

        /// <summary>
        /// Initializes before programmes are Smoothed.
        /// </summary>
        private void InitialiseBeforeSmoothProgrammes(
            IImmutableList<ClashException> clashExceptions,
            IImmutableList<Product> products,
            IImmutableList<Clash> clashes,
            IImmutableList<Restriction> restrictions,
            IImmutableList<IndexType> indexTypes,
            IImmutableList<Universe> universes,
            IImmutableList<RatingsPredictionSchedule> ratingsPredictionSchedules
            )
        {
            _smoothResources.CampaignClashChecker = new CampaignClashChecker(true);
            _smoothResources.ProductClashChecker = new ProductClashChecker(true);

            _smoothResources.ClashExceptionChecker = new ClashExceptionChecker(
                clashExceptions,
                products,
                clashes,
                _smoothConfiguration.ClashExceptionCheckEnabled
                );

            if (_smoothConfiguration.RestrictionCheckEnabled)
            {
                _smoothResources.RestrictionChecker = new RestrictionChecker(
                    restrictions,
                    products,
                    clashes,
                    indexTypes,
                    universes,
                    ratingsPredictionSchedules);
            }
            else
            {
                _smoothResources.RestrictionChecker = new NullRestrictionChecker();
            }
        }

        /// <summary>
        /// Smooth one programme.
        /// </summary>
        public SmoothProgramme StartSmoothingProgramme(
            Programme programme,
            SmoothBatchOutput smoothBatchOutput,
            IReadOnlyCollection<Break> programmeBreaks,
            IReadOnlyCollection<Spot> programmeSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyCollection<SmoothPass> smoothPasses,
            IReadOnlyDictionary<string, Break> breaksByExternalRef,
            IReadOnlyDictionary<string, SpotPlacement> previousSpotPlacementsByExternalRef,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            ISet<Guid> spotIdsUsed)
        {
            if (programme is null)
            {
                var guruMeditation = new ArgumentNullException(nameof(programme));
                RaiseException("The programme to Smooth is null", guruMeditation);

                throw guruMeditation;
            }

            var smoothProgramme = new SmoothProgramme(programme)
            {
                ProgSmoothBreaks = new List<SmoothBreak>(),
                Recommendations = new List<Recommendation>(),
                SalesArea = _salesArea,
                SmoothFailures = new List<SmoothFailure>()
            };

            RaiseDebug(
                $"Smoothing programme ID [{Log(smoothProgramme.Prog.Id)}] "
                + $"in sales area {programme.SalesArea}{Ellipsis}"
                );

            int countSpotsUnplacedBefore = programmeSpots.Count(s => !s.IsBooked());

            (
                smoothProgramme.BreaksWithPreviousSpots,
                smoothProgramme.BreaksWithoutPreviousSpots
            ) =
                InitialiseSmoothBreaksForThisProgramme(
                    smoothBatchOutput,
                    smoothProgramme,
                    programmeBreaks,
                    programmeSpots,
                    spotInfos,
                    spotIdsUsed);

            // For the breaks in this programme, perform multiple passes from
            // highest to lowest. If no programme breaks then don't execute any
            // passes otherwise we'll generate 'Unplaced spot attempt'
            // (TypeID=1) Smooth failures which is wrong. We'll generate 'No
            // placing attempt' Smooth failures at the end.
            var progSpotsNotUsed = new List<Spot>();
            var smoothPassResults = new List<SmoothPassResult>();

            // We only attempt to place spots if there are breaks
            if (smoothProgramme.ProgSmoothBreaks.Count > 0)
            {
                ExecuteSmoothPasses(
                    smoothPasses,
                    programmeSpots,
                    spotInfos,
                    breaksBeingSmoothed,
                    scheduleProgrammes,
                    smoothBatchOutput,
                    smoothProgramme,
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
                SpotPlacementService.MarkSpotsAsPlacementWasAttempted(programmeSpots, spotInfos);
            }

            // Add Smooth failures for unplaced spots
            smoothProgramme.SmoothFailures.AddRange(
                _smoothFailuresFactory.CreateSmoothFailuresForUnplacedSpots(
                    _runId,
                    _salesArea.Name,
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

            // Add Smooth failures for placed spots (E.g. Spot moved between breaks)
            smoothProgramme.SmoothFailures.AddRange(
                _smoothFailuresFactory.CreateSmoothFailuresForPlacedSpots(
                    _runId,
                    _salesArea.Name,
                    smoothProgramme.ProgSmoothBreaks,
                    breaksByExternalRef,
                    previousSpotPlacementsByExternalRef,
                    _threadSafeCollections.CampaignsByExternalRef,
                    _threadSafeCollections.ClashesByExternalRef,
                    _threadSafeCollections.ProductsByExternalRef
                    )
                );

            IReadOnlyCollection<Spot> spotsToBatchSave = RenumberSpotPositionInBreak(
                smoothBatchOutput,
                programmeBreaks,
                spotIdsUsed,
                smoothProgramme);

            smoothProgramme.SpotsToBatchSave.AddRange(spotsToBatchSave);

            var recommendationsForPlacedSpots = _smoothRecommendationsFactory.CreateRecommendationsForPlacedSpots(
                smoothProgramme.ProgSmoothBreaks,
                programme,
                _salesArea,
                _processorDateTime
                );

            AddRecommendationsForPlacedSpotsToSmoothProgramme(
                smoothProgramme,
                recommendationsForPlacedSpots);

            int countSpotsUnplacedAfter = programmeSpots.Count(s => !s.IsBooked());

            LogPlacedSpotDiagnostics(programme, smoothProgramme);
            LogProgrammeDiagnostics(smoothProgramme, countSpotsUnplacedBefore, countSpotsUnplacedAfter);

            _smoothDiagnostics.Flush();

            return smoothProgramme;
        }

        private void ExecuteSmoothPasses(
            IReadOnlyCollection<SmoothPass> smoothPasses,
            IReadOnlyCollection<Spot> programmeSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            SmoothBatchOutput smoothBatchOutput,
            SmoothProgramme smoothProgramme,
            List<Spot> progSpotsNotUsed,
            List<SmoothPassResult> smoothPassResults,
            ISet<Guid> spotIdsUsed)
        {
            foreach (var smoothPass in smoothPasses)
            {
                switch (smoothPass)
                {
                    case SmoothPassBooked specificSmoothPass:
                        // A pass for Booked spots. Don't store results, no
                        // spots placed
                        SmoothPassResult bookedSpotsSmoothPassResult = _smoothPassBookedExecuter.Execute(
                            specificSmoothPass,
                            smoothProgramme,
                            smoothProgramme.ProgSmoothBreaks,
                            breaksBeingSmoothed,
                            scheduleProgrammes,
                            spotIdsUsed);

                        smoothBatchOutput.BookedSpotsUnplacedDueToRestrictions +=
                            bookedSpotsSmoothPassResult.BookedSpotIdsUnplacedDueToRestrictions.Count;

                        break;

                    case SmoothPassDefault specificSmoothPass:
                        SmoothPassResult defaultSmoothPassResult = _smoothPassDefaultExecuter.Execute(
                            specificSmoothPass,
                            smoothProgramme,
                            programmeSpots,
                            spotInfos,
                            _threadSafeCollections.ClashesByExternalRef,
                            _threadSafeCollections.ProductsByExternalRef,
                            breaksBeingSmoothed,
                            scheduleProgrammes,
                            spotIdsUsed);

                        smoothPassResults.Add(defaultSmoothPassResult);
                        break;

                    case SmoothPassUnplaced specificSmoothPass:
                        // Try and place unplaced spots, spots where there
                        // wasn't sufficient time in any break.
                        progSpotsNotUsed.Clear();

                        var unbookedSpots = GetUnbookedSpots(programmeSpots, spotIdsUsed);
                        if (unbookedSpots.Count == 0)
                        {
                            break;
                        }

                        progSpotsNotUsed.AddRange(unbookedSpots);

                        SmoothPassResult unplacedSpotsPassResult = _smoothPassUnplacedExecuter.Execute(
                            specificSmoothPass,
                            smoothProgramme,
                            smoothProgramme.ProgSmoothBreaks,
                            progSpotsNotUsed,
                            spotInfos,
                            _threadSafeCollections.ClashesByExternalRef,
                            breaksBeingSmoothed,
                            scheduleProgrammes,
                            spotIdsUsed);

                        smoothPassResults.Add(unplacedSpotsPassResult);
                        smoothBatchOutput.SpotsSetAfterMovingOtherSpots += unplacedSpotsPassResult.CountPlacedSpots;

                        if (unplacedSpotsPassResult.CountPlacedSpots == 0)
                        {
                            break;
                        }

                        // Update list of unplaced spots so that we can calculate
                        // break avail adjustment. We placed previously unplaced
                        // spots after moving other spots about.
                        unbookedSpots = GetUnbookedSpots(programmeSpots, spotIdsUsed);

                        progSpotsNotUsed.Clear();
                        progSpotsNotUsed.AddRange(unbookedSpots);

                        break;
                }
            }

            // Local function
            static List<Spot> GetUnbookedSpots(
                IReadOnlyCollection<Spot> programmeSpots,
                ISet<Guid> spotIdsUsed)
            {
                var progSpotsNotUsed = new List<Spot>();

                foreach (Spot spot in programmeSpots.Where(s => !s.IsBooked()))
                {
                    if (spotIdsUsed.Contains(spot.Uid))
                    {
                        continue;
                    }

                    progSpotsNotUsed.Add(spot);
                }

                return progSpotsNotUsed;
            }
        }

        private (int countBreaksWithPreviousSpots, int countBreaksWithNoPreviousSpots)
        InitialiseSmoothBreaksForThisProgramme(
            SmoothBatchOutput smoothBatchOutput,
            SmoothProgramme smoothProgramme,
            IReadOnlyCollection<Break> programmeBreaks,
            IReadOnlyCollection<Spot> programmeSpots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ISet<Guid> spotIdsUsed)
        {
            int breakPosition = 0;
            int countBreaksWithPreviousSpots = 0;
            int countBreaksWithNoPreviousSpots = 0;

            foreach (var oneBreak in programmeBreaks)
            {
                breakPosition++;
                var smoothBreak = new SmoothBreak(oneBreak, breakPosition);
                smoothProgramme.ProgSmoothBreaks.Add(smoothBreak);

                smoothBatchOutput.Breaks++;

                IReadOnlyCollection<Spot> bookedSpotsForBreak =
                    GetSpotsPreviouslyBookedIntoBreakWithExternalReference(
                        programmeSpots,
                        oneBreak.ExternalBreakRef);

                if (bookedSpotsForBreak.Count == 0)
                {
                    countBreaksWithNoPreviousSpots++;
                    continue;
                }

                countBreaksWithPreviousSpots++;

                _ = SpotPlacementService.AddBookedSpotsToBreak(
                    smoothBreak,
                    bookedSpotsForBreak,
                    spotInfos,
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
            )
             .ToList();
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

        private static IReadOnlyCollection<Spot> RenumberSpotPositionInBreak(
            SmoothBatchOutput smoothOutput,
            IReadOnlyCollection<Break> programmeBreaks,
            ISet<Guid> spotIdsUsed,
            SmoothProgramme smoothProgramme)
        {
            var spotsToBatchSave = new List<Spot>();

            foreach (Break theBreak in programmeBreaks)
            {
                SmoothBreak smoothBreak = smoothProgramme
                    .ProgSmoothBreaks
                    .Find(b => b.TheBreak == theBreak);

                smoothBreak.RenumberActualPositionInBreak();

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
            void UpdateStatisticsForPass(SmoothBatchOutput output, SmoothSpot spot)
            {
                if (output.OutputByPass.ContainsKey(spot.SmoothPassSequence))
                {
                    output.OutputByPass[spot.SmoothPassSequence].CountSpotsSet++;
                }
            }
        }

        private void LogPlacedSpotDiagnostics(
            Programme prog,
            SmoothProgramme smoothProgramme) =>
            _smoothDiagnostics.LogPlacedSmoothSpots(prog, smoothProgramme.ProgSmoothBreaks);

        private void LogProgrammeDiagnostics(
            SmoothProgramme smoothProgramme,
            int countSpotsUnplacedBefore,
            int countSpotsUnplacedAfter)
        {
            if (!_smoothDiagnostics.LogDetail)
            {
                return;
            }

            _smoothDiagnostics.LogProgramme(
                smoothProgramme.Prog.Id,
                smoothProgramme.ProgSmoothBreaks,
                countSpotsUnplacedBefore,
                countSpotsUnplacedAfter
                );
        }

        private void AddRecommendationsForPlacedSpotsToSmoothProgramme(
            SmoothProgramme smoothProgramme,
            IReadOnlyCollection<Recommendation> recommendationsForPlacedSpots)
        {
            if (recommendationsForPlacedSpots.Count == 0)
            {
                return;
            }

            RaiseInfo(
                $"Created {Log(recommendationsForPlacedSpots.Count)} recommendations for "
                + $"placed spots in sales area {_salesArea.Name} "
                + $"for programme {Log(smoothProgramme.Prog.Id)}"
                );

            smoothProgramme.Recommendations.AddRange(recommendationsForPlacedSpots);
        }
    }
}
