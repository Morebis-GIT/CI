using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;
using xggameplan.core.Extensions;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Creates Smooth scenarios
    /// </summary>
    internal class SmoothScenarioCreator
    {
        private readonly SmoothResources _smoothResources;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;

        public SmoothScenarioCreator(
            SmoothResources smoothResources,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            _smoothResources = smoothResources;
            _spotInfos = spotInfos;
        }

        /// <summary>
        /// <para>Returns list of scenarios to try.</para>
        /// <para>
        /// We may need to improve the way that we work out the best scenario
        /// using the ImpactScore property.
        /// </para>
        /// </summary>
        /// <param name="spotsForBreak"></param>
        /// <param name="progSmoothBreaks"></param>
        /// <returns></returns>
        public IReadOnlyCollection<SmoothScenario> GetSmoothScenarios(
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<SmoothFailureMessagesForSpotsCollection> validateAddSpotsToBreakResults,
            SpotPositionRules breakPositionRules,
            bool respectSpotTime)
        {
            var smoothScenarios = new List<SmoothScenario>();

            smoothScenarios.AddRange(
                GetSmoothScenariosForSponsoredSpots(
                    spotsForBreak,
                    progSmoothBreaks,
                    validateAddSpotsToBreakResults,
                    lastSmoothScenarioSequence: 0,
                    breakPositionRules,
                    respectSpotTime
                    )
                );

            // Add scenario with no actions. Set it has the highest impact so
            // that we ideally pick one of the scenarios from above.
            smoothScenarios.Add(
                new SmoothScenario
                {
                    Id = Guid.NewGuid(),
                    Sequence = smoothScenarios.Count + 1,
                    Priority = Int32.MaxValue
                });

            return smoothScenarios
                .OrderBy(ss => ss.Sequence)
                .ToList();
        }

        /// <summary>
        /// Returns whether spot can be removed from break
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        private bool CanMoveSpotFromBreak(Spot spot)
        {
            return BreakUtilities.IsBreakRefNotSetOrUnused(_spotInfos[spot.Uid].ExternalBreakRefAtRunStart) ||                          // Unbooked spots can always be moved
                   (!BreakUtilities.IsBreakRefNotSetOrUnused(_spotInfos[spot.Uid].ExternalBreakRefAtRunStart) && spot.Preemptable);     // Booked spots can only be moved if preemptable
        }

        /// <summary>
        /// <para>
        /// Get SmoothScenario instances for sponsored spots. We try and return scenarios.
        /// </para>
        /// <para>
        /// Unfortunately we have some business rules regarding sponsor spots
        /// coded here. We should try and make it more data driven later on. The
        /// business rules are that the first sponsor spot is placed in break
        /// #1, the next in break #2.
        /// </para>
        /// </summary>
        private IReadOnlyCollection<SmoothScenario> GetSmoothScenariosForSponsoredSpots(
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<SmoothFailureMessagesForSpotsCollection> validateAddSpotsToBreakResults,
            int lastSmoothScenarioSequence,
            SpotPositionRules breakPositionRules,
            bool respectSpotTime)
        {
            if (progSmoothBreaks.Count == 0)
            {
                return new List<SmoothScenario>();
            }

            var sponsoredSpots = spotsForBreak
                .Where(s => s.Sponsored);

            if (!sponsoredSpots.Any())
            {
                return new List<SmoothScenario>();
            }

            var sponsoredSpot = sponsoredSpots.First();

            var validBreaksForSpotTime = progSmoothBreaks
                .Where(sb =>
                {
                    ICanAddSpot canAddSpotService = new CanAddSpotService(sb);
                    return canAddSpotService.CanAddSpotWithTime(sponsoredSpot.StartDateTime, sponsoredSpot.EndDateTime);
                })
                .OrderBy(sb => sb.TheBreak.ScheduledDate);

            if (!validBreaksForSpotTime.Any())
            {
                return new List<SmoothScenario>();
            }

            bool isRestrictedSpotTime = validBreaksForSpotTime.First().Position != 1 || validBreaksForSpotTime.Last().Position != progSmoothBreaks.Last().Position;

            var smoothScenarios = new List<SmoothScenario>();

            var listOfSpotValidations = validateAddSpotsToBreakResults.ToList();
            var listOfProgrammeSmoothBreaks = progSmoothBreaks.ToList();

            // Check each break, see if there are sponsored spots that we could move
            foreach (var progSmoothBreak in progSmoothBreaks)
            {
                // Lists of groups of spots moved out of break, used for
                // preventing duplicate scenarios which is inefficient
                var externalSpotRefsMovedOutOfBreakByGroup = new HashSet<string>();

                // Get results for adding spot to break in its current state
                var validateAddSpotsToBreakResult = listOfSpotValidations[listOfProgrammeSmoothBreaks.IndexOf(progSmoothBreak)];

                // Check if any failures. Ignore if T1_BreakPosition which
                // happens if break requested and this break is not allowed If
                // break position not allowed then don't move any spots. This
                // would happen if a break request was specified and this isn't
                // the requested break
                if (validateAddSpotsToBreakResult[sponsoredSpot.Uid].Failures.Count > 0 &&    // Failures prevent spot being added
                    !validateAddSpotsToBreakResult[sponsoredSpot.Uid].Failures.Any(f => f.FailureMessage == SmoothFailureMessages.T1_BreakPosition))
                {
                    var spotGroupsToMove = new List<Spot[]>();

                    // Only consider moving spots from this break if any of the
                    // following conditions are true:
                    // - No break request and first or last break (as per
                    // business rules).
                    // - Requested break and this is requested break.
                    // - Spot has restricted times and break is within it.
                    bool addScenariosToFixFailures = false;

                    if (!HasBreakRequest(sponsoredSpot) &&
                        (progSmoothBreak.Position == listOfProgrammeSmoothBreaks[0].Position || progSmoothBreak.Position == progSmoothBreaks.Last().Position)
                        )
                    {
                        addScenariosToFixFailures = true;
                    }
                    else if (HasBreakRequest(sponsoredSpot))
                    {
                        ICanAddSpot canAddSpotService = new CanAddSpotService(progSmoothBreak);

                        if (canAddSpotService.CanAddSpotWithBreakRequest(sponsoredSpot.BreakRequest, progSmoothBreaks.Count, breakPositionRules))
                        {
                            addScenariosToFixFailures = true;
                        }
                        else if (IsBreakWithinSpotTimeRestriction(respectSpotTime, isRestrictedSpotTime, validBreaksForSpotTime, progSmoothBreak))
                        {
                            addScenariosToFixFailures = true;
                        }
                    }
                    else if (IsBreakWithinSpotTimeRestriction(respectSpotTime, isRestrictedSpotTime, validBreaksForSpotTime, progSmoothBreak))
                    {
                        addScenariosToFixFailures = true;
                    }

                    if (addScenariosToFixFailures)
                    {
                        // See if we can find spots to move out of the break,
                        // check failures
                        foreach (var failure in validateAddSpotsToBreakResult[sponsoredSpot.Uid].Failures)
                        {
                            switch (failure.FailureMessage)
                            {
                                // Move spots for campaign clash
                                case SmoothFailureMessages.T1_CampaignClash:
                                    List<Spot> spotsToSeeIfCanBeMoved = _smoothResources.CampaignClashChecker.GetCampaignClashesForNewSpots(
                                        new List<Spot>() { sponsoredSpot },
                                        progSmoothBreak.SmoothSpots.Select(s => s.Spot).ToList()
                                        );

                                    var spotsForCampaignClashFailure = GetSpotsThatCanBeMovedForSpots(
                                        spotsForBreak,
                                        spotsToSeeIfCanBeMoved);

                                    foreach (var spot in spotsForCampaignClashFailure)
                                    {
                                        spotGroupsToMove.Add(
                                            new Spot[] { spot }
                                        );
                                    }

                                    break;

                                // Move spots to free up break availability
                                case SmoothFailureMessages.T1_InsufficentRemainingDuration:
                                    Duration minSpotLength = sponsoredSpot.SpotLength - progSmoothBreak.RemainingAvailability;

                                    // Sanity check
                                    if (minSpotLength > Duration.Zero)
                                    {
                                        // Get all single spots that
                                        IReadOnlyCollection<Spot> spotsWithEnoughLength = progSmoothBreak.SmoothSpots
                                            .Where(s => s.Spot.SpotLength >= minSpotLength)
                                            .Select(s => s.Spot)
                                            .ToList();

                                        var spotsForInsufficientRemainingDurationFailure = GetSpotsThatCanBeMovedForSpots(
                                            spotsForBreak,
                                            spotsWithEnoughLength);

                                        // Single spots can be moved
                                        if (spotsForInsufficientRemainingDurationFailure.Count > 0)
                                        {
                                            // Limit number to try
                                            foreach (var spot in spotsForInsufficientRemainingDurationFailure.Take(2))
                                            {
                                                spotGroupsToMove.Add(new Spot[] { spot });
                                            }
                                        }
                                        else
                                        {
                                            // No single spots can be moved to
                                            // make room, try spot combinations
                                            // Get all short spots that can be moved
                                            IReadOnlyCollection<Spot> spots = progSmoothBreak.SmoothSpots
                                                .Select(s => s.Spot)
                                                .ToList();

                                            var spotsToMoveFromBreak = GetSpotsThatCanBeMovedForSpots(
                                                spotsForBreak,
                                                spots)
                                            .ToList();

                                            // Add spot combinations (pairs)
                                            // that would leave sufficient break availability
                                            if (spotsToMoveFromBreak.Count <= 1)
                                            {
                                                break;
                                            }

                                            // Get all combinations of 2 spots
                                            // from the lowest priority N spots
                                            // (because we ideally want to move
                                            // the lowest priority spots),
                                            var spotCombinations = Combinations
                                                .GetCombinations(2, spotsToMoveFromBreak.Take(3).ToArray())
                                                .OrderByDescending(sc => sc.Average(s => s.Preemptlevel));

                                            // Check each combination
                                            int countSpotCombinationsUsed = 0;

                                            // Spots leave sufficient availability
                                            foreach (var spotCombination in spotCombinations
                                                .Where(sc => SpotUtilities.GetTotalSpotLength(sc) >= minSpotLength.ToTimeSpan())
                                                )
                                            {
                                                foreach (var _ in spotsForInsufficientRemainingDurationFailure)
                                                {
                                                    spotGroupsToMove.Add(spotCombination);
                                                }

                                                countSpotCombinationsUsed++;

                                                // Limit number of combinations
                                                if (countSpotCombinationsUsed >= 2)
                                                {
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    break;

                                case SmoothFailureMessages.T1_ProductClash:
                                    // Move spots for product clash
                                    IReadOnlyCollection<Spot> spotsThatClash = _smoothResources.ProductClashChecker
                                        .GetProductClashesForSingleSpot(
                                            sponsoredSpot,
                                            progSmoothBreak.SmoothSpots.Select(s => s.Spot).ToList(),
                                            _spotInfos,
                                            ClashCodeLevel.Parent
                                            );

                                    var spotsForProductClashFailure = GetSpotsThatCanBeMovedForSpots(
                                        spotsForBreak,
                                        spotsThatClash);

                                    foreach (var spot in spotsForProductClashFailure)
                                    {
                                        spotGroupsToMove.Add(new Spot[] { spot });
                                    }

                                    break;

                                case SmoothFailureMessages.T1_RequestedPositionInBreak:

                                    var spotPositioning = new SpotPositioning();
                                    int breakSeqForPositionInBreakRequest = String.IsNullOrEmpty(sponsoredSpot.RequestedPositioninBreak)
                                        ? 0
                                        : spotPositioning.GetBreakSeqFromRequestedPositionInBreak(sponsoredSpot.RequestedPositioninBreak);

                                    // Move spots with same requested position
                                    // in break
                                    var spotsForRequestedPositionInBreakFailure = GetSpotsThatCanBeMovedForSpots(
                                        spotsForBreak,
                                        progSmoothBreak.SmoothSpots
                                            .Where(s => s.BreakSequence == breakSeqForPositionInBreakRequest)
                                            .Select(s => s.Spot)
                                            .ToList()
                                            );

                                    foreach (var spot in spotsForRequestedPositionInBreakFailure)
                                    {
                                        spotGroupsToMove.Add(new Spot[] { spot });
                                    }

                                    break;
                            }
                        }
                    }

                    // Add scenario to move group of spots, avoid duplicating
                    // scenarios for same spot
                    foreach (var spotGroupToMove in spotGroupsToMove)
                    {
                        string externalSpotRefsForGroup = SpotUtilities.GetListOfSpotExternalReferences(",", spotGroupToMove.ToList());

                        if (!externalSpotRefsMovedOutOfBreakByGroup.Contains(externalSpotRefsForGroup))
                        {
                            const int PriorityBase2 = 1_000_000;

                            var smoothScenario = new SmoothScenario()
                            {
                                Id = Guid.NewGuid(),
                                Sequence = lastSmoothScenarioSequence + 1,
                                Priority = PriorityBase2 + (100_000 - spotGroupToMove[0].Preemptlevel)
                            };

                            smoothScenario.Actions.Add(
                                new SmoothActionMoveSpotToUnplaced(
                                    sequence: 1,
                                    spotGroupToMove.Select(s => s.ExternalSpotRef)
                                    )
                                );

                            smoothScenarios.Add(smoothScenario);

                            lastSmoothScenarioSequence = smoothScenarios.Last().Sequence;

                            _ = externalSpotRefsMovedOutOfBreakByGroup.Add(externalSpotRefsForGroup);
                        }
                    }
                }
            }

            return smoothScenarios;

            // Local functions
            static bool IsBreakWithinSpotTimeRestriction(
                bool respectSpotTime,
                bool isRestrictedSpotTime,
                IOrderedEnumerable<SmoothBreak> validBreaksForSpotTime,
                SmoothBreak progSmoothBreak)
            {
                return isRestrictedSpotTime
                    && respectSpotTime
                    && progSmoothBreak.Position >= validBreaksForSpotTime.First().Position
                    && progSmoothBreak.Position <= validBreaksForSpotTime.LastOrDefault()?.Position;
            }

            static bool HasBreakRequest(Spot spot) =>
                !String.IsNullOrWhiteSpace(spot.BreakRequest);
        }

        /// <summary>
        /// Returns spots that can be moved for the spots, orders lowest
        /// priority first
        /// </summary>
        private IReadOnlyCollection<Spot> GetSpotsThatCanBeMovedForSpots(
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyCollection<Spot> spotsToSeeIfCanBeMoved)
        {
            Spot spot = spotsForBreak.First();

            return spotsToSeeIfCanBeMoved
                .Where(s => s.Preemptlevel > spot.Preemptlevel &&               // Move lower priority only
                    !s.IsMultipartSpot &&                                       // Don't move multipart spots
                    !SpotUtilities.IsSameSpotSponsor(s, spot, _spotInfos) &&    // Ignore same sponsor
                    CanMoveSpotFromBreak(s)
                    )
                .OrderByDescending(s => s.Preemptlevel)
                .ToList();
        }

        /// <summary>
        /// Copies the scenario data so that it can be applied to copies of the data.
        /// </summary>
        public void CopyScenarioData(
            IReadOnlyCollection<Spot> spotsForBreak,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            ICollection<Guid> spotIdsUsed,
            List<Spot> spotsForBreakCopy,
            List<SmoothBreak> progSmoothBreaksCopy,
            ICollection<Guid> spotIdsUsedCopy)
        {
            spotsForBreakCopy.Clear();
            progSmoothBreaksCopy.Clear();
            spotIdsUsedCopy.Clear();

            Copy(spotsForBreak, spotsForBreakCopy);
            Copy(progSmoothBreaks, progSmoothBreaksCopy);

            spotIdsUsed.CopyDistinctTo(spotIdsUsedCopy);

            // Local function
            static void Copy<T>(IReadOnlyCollection<T> source, List<T> destination)
                where T : class, ICloneable
            {
                destination.AddRange(
                    from item in source
                    select (T)item.Clone()
                    );
            }
        }
    }
}
