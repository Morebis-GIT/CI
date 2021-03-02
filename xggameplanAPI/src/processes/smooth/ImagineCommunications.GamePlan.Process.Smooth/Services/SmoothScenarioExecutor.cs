using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Executes Smooth scenarios
    /// </summary>
    internal class SmoothScenarioExecutor
    {
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;
        private readonly SponsorshipRestrictionService _sponsorshipRestrictionService;

        public SmoothScenarioExecutor(
            ISmoothDiagnostics smoothDiagnostics,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            SponsorshipRestrictionService sponsorshipRestrictionService)
        {
            _smoothDiagnostics = smoothDiagnostics;
            _spotInfos = spotInfos;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;
        }

        /// <summary>
        /// Executes the scenario
        /// </summary>
        public void Execute(
            SmoothPass smoothPass,
            int smoothPassIteration,
            SmoothScenario smoothScenario,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            ICollection<Guid> spotIdsUsed,
            bool logSpotAction)
        {
            foreach (var action in smoothScenario.Actions.OrderBy(a => a.Sequence))
            {
                switch (action)
                {
                    case SmoothActionMoveSpotToUnplaced unplaceAction:
                        ExecuteMoveToUnplacedAction(
                            smoothPass,
                            smoothPassIteration,
                            unplaceAction,
                            progSmoothBreaks,
                            spotIdsUsed,
                            logSpotAction);

                        break;

                    case SmoothActionMoveSpotToBreak moveToBreakAction:
                        ExecuteMoveSpotToBreakAction(
                            smoothPass,
                            smoothPassIteration,
                            moveToBreakAction,
                            progSmoothBreaks,
                            spotIdsUsed,
                            logSpotAction);

                        break;
                }
            }
        }

        /// <summary>
        /// Execute the action to move a spot
        /// </summary>
        private void ExecuteMoveSpotToBreakAction(
            SmoothPass smoothPass,
            int smoothPassIteration,
            SmoothActionMoveSpotToBreak smoothAction,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            ICollection<Guid> spotIdsUsed,
            bool logSpotAction)
        {
            var externalSpotRefs = smoothAction.ExternalSpotRefs.ToList();
            var externalBreakRefs = smoothAction.ExternalBreakRefs.ToList();

            for (int index = 0; index < externalSpotRefs.Count; index++)
            {
                MoveSpotToBreak(
                    smoothPass,
                    smoothPassIteration,
                    externalSpotRefs[index],
                    externalBreakRefs[index],
                    progSmoothBreaks,
                    spotIdsUsed,
                    logSpotAction);
            }
        }

        /// <summary>
        /// Moves the spot to the break
        /// </summary>
        private void MoveSpotToBreak(
            SmoothPass smoothPass,
            int smoothPassIteration,
            string externalSpotRef,
            string externalBreakRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            ICollection<Guid> spotIdsUsed,
            bool logSpotAction)
        {
            // Find the break that spot is placed in
            var srcSmoothBreak = progSmoothBreaks.FirstOrDefault(sb =>
                sb.SmoothSpots.Any(s => s.Spot.ExternalSpotRef == externalSpotRef)
                );

            if (srcSmoothBreak is null)
            {
                return;
            }

            SmoothSpot smoothSpot = srcSmoothBreak.SmoothSpots
                .First(s => s.Spot.ExternalSpotRef == externalSpotRef);

            SpotPlacementService.RemoveSpotFromBreak(
                srcSmoothBreak,
                smoothSpot.Spot,
                spotIdsUsed,
                _sponsorshipRestrictionService
                );

            if (logSpotAction)
            {
                _smoothDiagnostics.LogSpotAction(
                    smoothPass,
                    smoothPassIteration,
                    smoothSpot.Spot,
                    srcSmoothBreak,
                    SmoothSpot.SmoothSpotActions.RemoveSpotFromBreak,
                    "Scenario");
            }

            // Find the break that spot should be placed in.
            var dstSmoothBreak = progSmoothBreaks.FirstOrDefault(sb =>
                sb.TheBreak.ExternalBreakRef == externalBreakRef);

            if (dstSmoothBreak is null)
            {
                return;
            }

            _ = SpotPlacementService.AddSpotsToBreak(
                  dstSmoothBreak,
                  smoothSpot.SmoothPassSequence,
                  smoothPassIterationSequence: 0,
                  new List<Spot>() { smoothSpot.Spot },
                  SpotPositionRules.Exact,
                  canMoveSpotToOtherBreak: true,
                  spotIdsUsed,
                  bestBreakFactorGroupName: null,
                  _spotInfos,
                  _sponsorshipRestrictionService);

            if (logSpotAction)
            {
                _smoothDiagnostics.LogSpotAction(
                    smoothPass,
                    smoothPassIteration,
                    smoothSpot.Spot,
                    dstSmoothBreak,
                    SmoothSpot.SmoothSpotActions.PlaceSpotInBreak,
                    "Scenario");
            }
        }

        /// <summary>
        /// Executes action to move spot(s) to unplaced list
        /// </summary>
        private void ExecuteMoveToUnplacedAction(
            SmoothPass smoothPass,
            int smoothPassIteration,
            SmoothActionMoveSpotToUnplaced smoothAction,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            ICollection<Guid> spotIdsUsed,
            bool logSpotAction)
        {
            foreach (var externalSpotRef in smoothAction.ExternalSpotRefs)
            {
                MoveSpotToUnplacedList(
                    smoothPass,
                    smoothPassIteration,
                    externalSpotRef,
                    progSmoothBreaks,
                    spotIdsUsed,
                    logSpotAction);
            }
        }

        /// <summary>
        /// Moves the spot to the unplaced spots list
        /// </summary>
        private void MoveSpotToUnplacedList(
            SmoothPass smoothPass,
            int smoothPassIteration,
            string externalSpotRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            ICollection<Guid> spotIdsUsed,
            bool logSpotAction)
        {
            // Find the break that spot is placed in
            var smoothBreak = progSmoothBreaks.FirstOrDefault(sb =>
                sb.SmoothSpots.Any(s => s.Spot.ExternalSpotRef == externalSpotRef)
                );

            if (smoothBreak is null)
            {
                return;
            }

            // Spot was placed in break
            var smoothSpot = smoothBreak.SmoothSpots.FirstOrDefault(s =>
                s.Spot.ExternalSpotRef == externalSpotRef);

            SpotPlacementService.RemoveSpotFromBreak(
                smoothBreak,
                smoothSpot.Spot,
                spotIdsUsed,
                _sponsorshipRestrictionService
                );

            if (logSpotAction)
            {
                _smoothDiagnostics.LogSpotAction(
                    smoothPass,
                    smoothPassIteration,
                    smoothSpot.Spot,
                    smoothBreak,
                    SmoothSpot.SmoothSpotActions.RemoveSpotFromBreak,
                    "Scenario");
            }
        }
    }
}
