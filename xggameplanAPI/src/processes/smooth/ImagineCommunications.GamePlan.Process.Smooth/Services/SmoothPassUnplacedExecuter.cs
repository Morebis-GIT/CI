using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Smooth pass executer for unplaced spots. For each unplaced spot then it
    /// attempts to move other spots. Booked spots cannot be moved. The intent
    /// is to try and only move low priority spots if possible. When moving
    /// spots around then we make our best attempt but we can't always guarantee
    /// that it will be possible for various reasons (E.g. Some spots can't be
    /// moved. We only move another spot once rather than multiple times.)
    /// </summary>
    internal class SmoothPassUnplacedExecuter : SmoothPassExecuter
    {
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly SmoothResources _smoothResources;
        private readonly SmoothProgramme _smoothProgramme;
        private readonly IClashExposureCountService _clashExposureCountService;
        private readonly SponsorshipRestrictionService _sponsorshipRestrictionService;
        private readonly IReadOnlyCollection<Programme> _allProgrammesForPeriodAndSalesArea;
        private readonly IImmutableDictionary<string, Clash> _clashesByExternalRef;

        private Action<string, Exception> RaiseException { get; }

        public SmoothPassUnplacedExecuter(
            ISmoothDiagnostics smoothDiagnostics,
            SmoothResources smoothResources,
            SmoothProgramme smoothProgramme,
            SponsorshipRestrictionService sponsorshipRestrictionService,
            IReadOnlyCollection<Programme> allProgrammesForPeriodAndSalesArea,
            ISmoothConfiguration smoothConfiguration,
            IClashExposureCountService clashExposureCountService,
            IImmutableDictionary<string, Clash> clashesByExternalRef,
            Action<string, Exception> raiseException
            )
        {
            _smoothDiagnostics = smoothDiagnostics;
            _smoothConfiguration = smoothConfiguration;
            _smoothResources = smoothResources;
            _smoothProgramme = smoothProgramme;
            _clashExposureCountService = clashExposureCountService;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;
            _allProgrammesForPeriodAndSalesArea = allProgrammesForPeriodAndSalesArea;
            _clashesByExternalRef = clashesByExternalRef;
            RaiseException = raiseException;
        }

        /// <summary>
        /// Attempt to place unplaced spots by moving other spots. These are
        /// spots that couldn't be placed during the main pass, should only be
        /// spots that couldn't be placed due to no break with sufficient
        /// remaining time.
        /// </summary>
        public SmoothPassResult Execute(
            SmoothPassUnplaced smoothPass,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            ISet<Guid> spotIdsUsed,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            var smoothPassResult = new SmoothPassResult(smoothPass.Sequence);

            if (MaximumOfTwoBreaksWithRemainingTime(_smoothProgramme.ProgrammeSmoothBreaks))
            {
                return smoothPassResult;
            }

            // Get all spots for pass, order by priority for processing
            var spotFilter = new SpotFilter()
            {
                Sponsored = null,
                Preemptable = null,
                MinPreemptLevel = null,     // Any
                MaxPreemptLevel = null,     // Any
                HasBreakRequest = null,     // Any
                BreakRequests = null,
                HasPositionInBreakRequest = null,       // Any
                PositionInBreakRequestsToExclude = null,        // Any
                HasMultipartSpots = null,
                MultipartSpots = null,      // Any
                HasProductClashCode = null,
                ProductClashCodesToExclude = null,
                ExternalCampaignRefsToExclude = _smoothConfiguration.ExternalCampaignRefsToExclude,
                HasSpotEndTime = null,
                MinSpotLength = null,
                MaxSpotLength = null,
                SpotIdsToExclude = spotIdsUsed
            };

            // Order the spots by priority
            var spotsToPlace = GetSpots(spotFilter, spots, spotInfos);
            var spotsOrdered = _smoothConfiguration.SortSpotsToPlace(
                spotsToPlace,
                (_smoothProgramme.Programme.StartDateTime, _smoothProgramme.Programme.Duration)
            );

            // Try and place spots
            foreach (var spot in spotsOrdered)
            {
                try
                {
                    PlaceSpotsResult placeSpotsResult = PlaceSpot(
                        smoothPass,
                        spot,
                        spotInfos,
                        _smoothProgramme.ProgrammeSmoothBreaks,
                        breaksBeingSmoothed,
                        spotIdsUsed);

                    smoothPassResult.PlaceSpotsResultList.Add(placeSpotsResult);
                }
                catch (Exception exception)
                {
                    RaiseException($"Error trying to place unplaced spot {spot.ExternalSpotRef}", exception);
                }
            }

            return smoothPassResult;

            static bool MaximumOfTwoBreaksWithRemainingTime(List<SmoothBreak> smoothBreaks) =>
                smoothBreaks.Count(sb => sb.RemainingAvailability > Duration.Zero) < 2;
        }

        /// <summary>
        /// Attempts to place unplaced spot by moving other spots.
        /// </summary>
        private PlaceSpotsResult PlaceSpot(
            SmoothPassUnplaced smoothPass,
            Spot spot,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            ISet<Guid> spotIdsUsed)
        {
            var placeSpotsResult = new PlaceSpotsResult();
            bool spotPlaced = false;

            // Don't currently handle multipart spots here because we shouldn't split them.
            if (!spot.IsMultipartSpot)
            {
                spotPlaced = PlaceSingleSpot(
                    smoothPass,
                    spot,
                    spotInfos,
                    progSmoothBreaks,
                    breaksBeingSmoothed,
                    spotIdsUsed,
                    placeSpotsResult,
                    spotPlaced);
            }

            if (!spotPlaced)
            {
                var placeSpotResult = new PlaceSpotResult(spot.Uid);

                placeSpotResult.ValidateAddSpotsResultForSpot.Failures.Add(
                    new SmoothFailureAndReasonForFailure(SmoothFailureMessages.T5_CantPlaceUnplacedSpot)
                    );

                placeSpotsResult.UnplacedSpotResults.Add(placeSpotResult);
            }

            return placeSpotsResult;
        }

        /// <summary>Places a single, non-multipart spot.</summary>
        /// <param name="smoothPass">The smooth pass.</param>
        /// <param name="spot">The spot.</param>
        /// <param name="spotInfos">The spot infos.</param>
        /// <param name="progSmoothBreaks">The prog smooth breaks.</param>
        /// <param name="breaksBeingSmoothed">The breaks being smoothed.</param>
        /// <param name="spotIdsUsed">The spot ids used.</param>
        /// <param name="placeSpotsResult">The place spots result.</param>
        /// <param name="spotPlaced">if set to <c>true</c> the spot was placed.</param>
        /// <returns></returns>
        private bool PlaceSingleSpot(
            SmoothPassUnplaced smoothPass,
            Spot spot,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            ISet<Guid> spotIdsUsed,
            PlaceSpotsResult placeSpotsResult,
            bool spotPlaced)
        {
            // Check each iteration, try and find a break
            var smoothPassIterations = _smoothConfiguration.GetSmoothPassUnplacedIterations(spot, smoothPass);

            foreach (SmoothPassUnplacedIteration smoothPassIteration in smoothPassIterations.OrderBy(i => i.Sequence))
            {
                // Get all breaks where spot could be moved to if sufficient
                // time is remaining
                var smoothBreaksForElibility = progSmoothBreaks
                    .Where(smoothBreak =>
                    {
                        var respectingSpotTime = smoothPassIteration.RespectSpotTime;
                        bool isEligible = IsSpotEligible(respectingSpotTime, smoothBreak, spot);

                        var spotToCheck = new List<Spot> { spot };
                        var canAddSpotService = CanAddSpotService.Factory(smoothBreak);

                        return isEligible
                            && canAddSpotService.CanAddSpotsWithCampaignClashRule(
                                spotToCheck,
                                smoothPassIteration.RespectCampaignClash,
                                _smoothResources.CampaignClashChecker
                            )
                            && canAddSpotService.CanAddSpotsWithProductClashRule(
                                spotToCheck,
                                spotInfos,
                                _clashesByExternalRef,
                                smoothPassIteration.ProductClashRule,
                                smoothPassIteration.RespectClashExceptions,
                                _smoothResources.ProductClashChecker,
                                _smoothResources.ClashExceptionChecker,
                                _clashExposureCountService
                            )
                            && canAddSpotService.CanAddSpotsWithRestrictionRule(
                                _smoothProgramme,
                                spotToCheck,
                                smoothPassIteration.RespectRestrictions,
                                _smoothResources.RestrictionChecker,
                                breaksBeingSmoothed,
                                _allProgrammesForPeriodAndSalesArea
                            );
                    })
                    .OrderByDescending(sb => sb.RemainingAvailability.BclCompatibleTicks);

                // See if we can just add the spot to a break with
                // sufficient duration. This can typically happen if we've
                // moved spots around on a previous call to this method and
                // freed up time.
                if (!spotPlaced && smoothBreaksForElibility.Any())
                {
                    foreach (var smoothBreak in smoothBreaksForElibility)
                    {
                        if (AnySponsorshipRestrictions(spot, smoothBreak))
                        {
                            _smoothDiagnostics.LogSpotAction(
                                smoothPass,
                                smoothPassIteration.Sequence,
                                spot,
                                smoothBreak,
                                SmoothSpot.SmoothSpotActions.ValidateAddSpotToBreak,
                                "Spot not placed as a sponsorship restriction was found.");

                            continue;
                        }

                        if (smoothBreak.RemainingAvailability >= spot.SpotLength)
                        {
                            var smoothSpotsAddedToBreak = SpotPlacementService.AddSpotsToBreak(
                                smoothBreak,
                                smoothPass.Sequence,
                                smoothPassIteration.Sequence,
                                new List<Spot>() { spot },
                                SpotPositionRules.Anywhere,
                                true,
                                spotIdsUsed,
                                null,
                                spotInfos,
                                _sponsorshipRestrictionService);

                            foreach (var smoothSpot in smoothSpotsAddedToBreak)
                            {
                                _smoothDiagnostics.LogSpotAction(
                                    smoothPass,
                                    smoothPassIteration.Sequence,
                                    smoothSpot.Spot,
                                    smoothBreak,
                                    SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                                    "");
                            }

                            var placeSpotResult = new PlaceSpotResult(
                                spot.Uid,
                                smoothBreak.TheBreak.ExternalBreakRef);

                            placeSpotsResult.PlacedSpotResults.Add(placeSpotResult);
                            spotPlaced = true;

                            break;
                        }
                    }
                }

                // Try moving spots in order to make room
                if (!spotPlaced && smoothBreaksForElibility.Any())
                {
                    foreach (var smoothBreakForMoveFrom in smoothBreaksForElibility)
                    {
                        if (AnySponsorshipRestrictions(spot, smoothBreakForMoveFrom))
                        {
                            continue;
                        }

                        // Get all spots to consider for move from this break
                        var spotsToConsiderForMove = _smoothConfiguration.GetSpotsThatCanBeMoved(spot, smoothBreakForMoveFrom);
                        if (spotsToConsiderForMove.Count > 0)
                        {
                            // Calculate spot move details for these spots,
                            // which spots to move, which break to move to
                            SpotMoveDetails spotMoveDetails = GetSpotMoveDetails(
                                smoothPassIteration,
                                spot,
                                smoothBreakForMoveFrom,
                                spotsToConsiderForMove,
                                progSmoothBreaks);

                            List<SmoothSpot> smoothSpotsToMove = spotMoveDetails.SmoothSpotsToMove;

                            if (smoothSpotsToMove.Count > 0)
                            {
                                // Found spots to move. Move spots to other breaks.
                                for (int spotIndex = 0; spotIndex < smoothSpotsToMove.Count; spotIndex++)
                                {
                                    MoveSpot(
                                        smoothPass,
                                        smoothSpotsToMove[spotIndex],
                                        smoothBreakForMoveFrom,
                                        spotMoveDetails.SmoothBreaksToMoveTo[spotIndex],
                                        spotInfos,
                                        spotIdsUsed);
                                }

                                // Place the spot
                                var smoothSpotsAddedToBreak = SpotPlacementService.AddSpotsToBreak(
                                    smoothBreakForMoveFrom,
                                    smoothPass.Sequence,
                                    smoothPassIteration.Sequence,
                                    new List<Spot> { spot },
                                    SpotPositionRules.Anywhere,
                                    canMoveSpotToOtherBreak: true,
                                    spotIdsUsed,
                                    bestBreakFactorGroupName: null,
                                    spotInfos,
                                    _sponsorshipRestrictionService);

                                string externalSpotReferences = SpotUtilities.GetListOfSpotExternalReferences(
                                    ",",
                                    smoothSpotsToMove.ConvertAll(s => s.Spot));

                                // Log spot action for diagnostic
                                smoothSpotsAddedToBreak.ForEach(smoothSpot =>
                                    _smoothDiagnostics.LogSpotAction(
                                        smoothPass,
                                        smoothPassIteration.Sequence,
                                        smoothSpot.Spot,
                                        smoothBreakForMoveFrom,
                                        SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                                        $"Moved {externalSpotReferences}"
                                        )
                                    );

                                var placeSpotResult = new PlaceSpotResult(
                                    spot.Uid,
                                    smoothBreakForMoveFrom.TheBreak.ExternalBreakRef
                                    );

                                placeSpotsResult.PlacedSpotResults.Add(placeSpotResult);
                                spotPlaced = true;

                                break;
                            }
                        }
                    }
                }

                if (spotPlaced)
                {
                    break;
                }
            }

            return spotPlaced;
        }

        private bool AnySponsorshipRestrictions(Spot spot, SmoothBreak smoothBreak)
        {
            IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result =
                _sponsorshipRestrictionService
                .CheckSponsorshipRestrictions(
                    spot,
                    smoothBreak.TheBreak.ExternalBreakRef,
                    smoothBreak.TheBreak.ScheduledDate,
                    smoothBreak.TheBreak.Duration,
                    smoothBreak.SmoothSpots.Select(s => s.Spot).ToList()
                    );

            return result
                .Where(r => r.spotUid == spot.Uid)
                .Select(r => r.failureMessage)
                .Any(r => SponsorshipRestrictionFailures().Contains(r));

            static IReadOnlyList<SmoothFailureMessages> SponsorshipRestrictionFailures() =>
                new SmoothFailureMessages[] {
                    SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash,
                    SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser
                    };
        }

        /// <summary>
        /// Moves spot between breaks
        /// </summary>
        /// <param name="smoothSpot"></param>
        /// <param name="smoothBreakForMoveFrom"></param>
        /// <param name="breakMoveForMoveTo"></param>
        /// <param name="spotIdsUsed"></param>
        private void MoveSpot(
            SmoothPass smoothPass,
            SmoothSpot smoothSpot,
            SmoothBreak smoothBreakForMoveFrom,
            SmoothBreak breakMoveForMoveTo,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ISet<Guid> spotIdsUsed)
        {
            if (IsMovingToSameBreak(smoothBreakForMoveFrom, breakMoveForMoveTo))
            {
                return;
            }

            SpotPlacementService.RemoveSpotFromBreak(
                smoothBreakForMoveFrom,
                smoothSpot.Spot,
                spotIdsUsed,
                _sponsorshipRestrictionService
                );

            _smoothDiagnostics.LogSpotAction(
                smoothPass,
                smoothPassIteration: 0,
                smoothSpot.Spot,
                smoothBreakForMoveFrom,
                SmoothSpot.SmoothSpotActions.RemoveSpotFromBreak,
                $"Moving to {breakMoveForMoveTo.TheBreak.ExternalBreakRef}");

            // Move spot to new break
            List<SmoothSpot> smoothSpotsMoved = null;

            if (smoothSpot.IsCurrent)
            {
                smoothSpotsMoved = SpotPlacementService.AddSpotsToBreak(
                    breakMoveForMoveTo,
                    smoothSpot.SmoothPassSequence,
                    smoothPassIterationSequence: 0,
                    new List<Spot>() { smoothSpot.Spot },
                    SpotPositionRules.Anywhere,
                    canMoveSpotToOtherBreak: true,
                    spotIdsUsed,
                    bestBreakFactorGroupName: null,
                    spotInfos,
                    _sponsorshipRestrictionService);
            }
            else
            {
                // Booked spot (this shouldn't happen as we don't generally move
                // booked spots)
                smoothSpotsMoved = SpotPlacementService.AddBookedSpotsToBreak(
                    breakMoveForMoveTo,
                    new List<Spot>() { smoothSpot.Spot },
                    spotInfos,
                    spotIdsUsed,
                    _sponsorshipRestrictionService);
            }

            // Log spot actions for diagnostic
            smoothSpotsMoved.ForEach(smoothSpotMoved => _smoothDiagnostics.LogSpotAction(
                smoothPass,
                smoothPassIteration: 0,
                smoothSpotMoved.Spot,
                breakMoveForMoveTo,
                SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                $"Moved from {smoothBreakForMoveFrom.TheBreak.ExternalBreakRef}"));

            // Record original break that spot was added to for debug
            smoothSpotsMoved.ForEach(ss => ss.BreakPositionMovedFrom = smoothBreakForMoveFrom.Position);

            //-----------------
            // Local functions

            // If the source and destination breaks are the same there's no need
            // to move.
            bool IsMovingToSameBreak(SmoothBreak source, SmoothBreak destination) =>
                source == destination;
        }

        /// <summary>
        /// Attempts to define spots that can be moved to make way for the new
        /// spot, returns nothing if not possible.
        /// </summary>
        /// <returns></returns>
        private static SpotMoveDetails GetSpotMoveDetails(
            SmoothPassUnplacedIteration smoothPassIteration,
            Spot spot,
            SmoothBreak smoothBreakForMoveFrom,
            IReadOnlyCollection<SmoothSpot> spotsToConsiderForMove,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks)
        {
            var spotMove = new SpotMoveDetails();

            // Determine how much time will be freed by moving all spots to
            // other breaks
            var allMovedSpotsSpotLength = new Duration();

            foreach (var s in spotsToConsiderForMove)
            {
                allMovedSpotsSpotLength = allMovedSpotsSpotLength.Plus(s.Spot.SpotLength);
            }

            var freedSpotsSpotLength = new Duration();

            // Moving all spots to other breaks (if possible) will free up
            // sufficient time to add spot
            if (smoothBreakForMoveFrom.RemainingAvailability.Plus(allMovedSpotsSpotLength) >= spot.SpotLength)
            {
                // Store copy of remaining durations which we'll use and adjust
                // when trying to see which spots can be moved
                var remainingDurationByBreak = new Dictionary<SmoothBreak, Duration>();

                foreach (var sb in progSmoothBreaks)
                {
                    remainingDurationByBreak.Add(sb, sb.RemainingAvailability);
                }

                // For the spots that we're allowed to move then see if we can
                // actually move them.
                foreach (var spotToConsiderForMove in spotsToConsiderForMove)
                {
                    // Get breaks that spot can be moved to if the break had
                    // sufficient time remaining
                    var smoothBreaksForMoveTo = progSmoothBreaks
                        .Where(sb =>
                        {
                            var respectingSpotTime = smoothPassIteration.RespectSpotTime;
                            bool isEligible = IsSpotEligible(respectingSpotTime, sb, spotToConsiderForMove.Spot);

                            return isEligible && sb != smoothBreakForMoveFrom;
                        })
                        .ToList();

                    SmoothBreak smoothBreakToUse = null;

                    // Check if any break has exact time remaining to accept
                    // spot for move. Filling breaks is our ideal result,
                    // reduces the likelihood of ending up with breaks with
                    // durations that are too small to fill.
                    if (smoothBreakToUse is null)
                    {
                        foreach (var smoothBreakForMoveTo in smoothBreaksForMoveTo)
                        {
                            if (remainingDurationByBreak[smoothBreakForMoveTo] == spotToConsiderForMove.Spot.SpotLength)
                            {
                                smoothBreakToUse = smoothBreakForMoveTo;
                                break;
                            }
                        }
                    }

                    // If no break with exact time remaining then just check if
                    // any break has sufficent time remaining to accept spot
                    if (smoothBreakToUse is null)
                    {
                        foreach (var smoothBreakForMoveTo in smoothBreaksForMoveTo)
                        {
                            if (remainingDurationByBreak[smoothBreakForMoveTo] >= spotToConsiderForMove.Spot.SpotLength)
                            {
                                smoothBreakToUse = smoothBreakForMoveTo;
                                break;
                            }
                        }
                    }

                    // If we found a break to move the spot to then add to list
                    if (smoothBreakToUse != null)
                    {
                        spotMove.SmoothSpotsToMove.Add(spotToConsiderForMove);
                        spotMove.SmoothBreaksToMoveTo.Add(smoothBreakToUse);

                        freedSpotsSpotLength = freedSpotsSpotLength.Plus(spotToConsiderForMove.Spot.SpotLength);

                        // Reduce the remaining break duration to include the
                        // spot that we'll move to this break
                        remainingDurationByBreak[smoothBreakToUse] = remainingDurationByBreak[smoothBreakToUse].Minus(spotToConsiderForMove.Spot.SpotLength);
                    }

                    // Sufficient time freed up with spots in spotsThatCanBeMoved
                    if (smoothBreakForMoveFrom.RemainingAvailability.Plus(freedSpotsSpotLength) >= spot.SpotLength)
                    {
                        break;
                    }
                }
            }

            // Sanity check
            if (smoothBreakForMoveFrom.RemainingAvailability.Plus(freedSpotsSpotLength) < spot.SpotLength)
            {
                spotMove.SmoothSpotsToMove.Clear();
                spotMove.SmoothBreaksToMoveTo.Clear();
            }

            return spotMove;
        }

        private static bool IsSpotEligible(
            bool respectingSpotTime,
            SmoothBreak smoothBreak,
            Spot spot)
        {
            var canAddSpotService = CanAddSpotService.Factory(smoothBreak);

            if (respectingSpotTime)
            {
                return canAddSpotService.IsSpotEligibleWhenRespectingSpotTime(spot);
            }
            else
            {
                return canAddSpotService.IsSpotEligibleWhenIgnoringSpotTime(spot);
            }
        }
    }
}
