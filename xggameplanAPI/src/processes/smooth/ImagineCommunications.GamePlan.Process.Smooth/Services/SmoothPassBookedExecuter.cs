using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Booked spots Smooth pass executer.
    /// </summary>
    internal class SmoothPassBookedExecuter : SmoothPassExecuter
    {
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly SmoothResources _smoothResources;
        private readonly SponsorshipRestrictionService _sponsorshipRestrictionService;

        public SmoothPassBookedExecuter(
            ISmoothDiagnostics smoothDiagnostics,
            SmoothResources smoothResources,
            SponsorshipRestrictionService sponsorshipRestrictionService,
            Action<string> raiseInfo,
            Action<string, Exception> raiseException
            )
        {
            _smoothDiagnostics = smoothDiagnostics;
            _smoothResources = smoothResources;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;

            RaiseInfo = raiseInfo;
            RaiseException = raiseException;
        }

        private Action<string> RaiseInfo { get; }
        private Action<string, Exception> RaiseException { get; }

        /// <summary>
        /// Executes a Smooth pass. Checks booked spots, unplaces any spots with
        /// a restriction
        /// </summary>
        /// <param name="smoothPass"></param>
        /// <param name="smoothProg"></param>
        /// <param name="progSmoothBreaks"></param>
        /// <param name="breaksBeingSmoothed"></param>
        /// <param name="scheduleProgrammes"></param>
        /// <param name="spotIdsUsed"></param>
        /// <returns></returns>
        public SmoothPassResult Execute(
            SmoothPassBooked smoothPass,
            SmoothProgramme smoothProg,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            ISet<Guid> spotIdsUsed
            )
        {
            var smoothPassResult = new SmoothPassResult(smoothPass.Sequence);

            // Check booked spots within each break
            foreach (var smoothBreak in progSmoothBreaks)
            {
                var breakSpotsToCheck = new List<SmoothSpot>(
                    smoothBreak.SmoothSpots.Where(s => !s.IsCurrent));

                BreakExternalReference breakExternalReference = smoothBreak.TheBreak.ExternalBreakRef;

                foreach (var smoothSpot in breakSpotsToCheck)
                {
                    bool AnySponsorshipRestrictions()
                    {
                        IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result =
                            _sponsorshipRestrictionService
                            .CheckSponsorshipRestrictions(
                                smoothSpot.Spot,
                                breakExternalReference,
                                smoothBreak.TheBreak.ScheduledDate,
                                smoothBreak.TheBreak.Duration,
                                smoothBreak.SmoothSpots.Select(s => s.Spot).ToList()
                                );

                        return result
                            .Where(r => r.spotUid == smoothSpot.Spot.Uid)
                            .Select(r => r.failureMessage)
                            .Any(r => SponsorshipRestrictionFailures().Contains(r));

                        static IReadOnlyList<SmoothFailureMessages> SponsorshipRestrictionFailures() =>
                            new SmoothFailureMessages[] {
                                SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash,
                                SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser
                                };
                    }

                    try
                    {
                        if (AnySponsorshipRestrictions())
                        {
                            // Spot has sponsorship restriction(s), unplace it
                            UnplaceSpot(
                                smoothPass,
                                smoothSpot,
                                smoothBreak,
                                "Sponsorship restrictions found: spot is a competitor to the sponsor",
                                spotIdsUsed
                                );

                            continue;
                        }

                        var restrictionResults = _smoothResources.RestrictionChecker.CheckRestrictions(
                            smoothProg.Prog,
                            smoothBreak.TheBreak,
                            smoothSpot.Spot,
                            smoothProg.SalesArea,
                            breaksBeingSmoothed,
                            scheduleProgrammes);

                        if (restrictionResults.Count == 0)
                        {
                            continue;
                        }

                        // Spot has restriction(s), unplace it
                        UnplaceSpot(
                            smoothPass,
                            smoothSpot,
                            smoothBreak,
                            $"Restrictions {GetRestrictionResultsDescription(restrictionResults)}",
                            spotIdsUsed
                            );

                        smoothPassResult.BookedSpotIdsUnplacedDueToRestrictions.Add(smoothSpot.Spot.Uid);
                    }
                    catch (Exception exception)
                    {
                        RaiseException(
                            $"Error checking booked spot {smoothSpot.Spot.ExternalSpotRef} on pass {Log(smoothPass.Sequence)}",
                            exception
                            );
                    }
                }
            }

            return smoothPassResult;
        }

        /// <summary>
        /// Returns restriction results description
        /// </summary>
        /// <param name="restrictionResults"></param>
        /// <returns></returns>
        private static string GetRestrictionResultsDescription(
            IEnumerable<CheckRestrictionResult> restrictionResults
            )
        {
            var description = new StringBuilder();

            foreach (var restrictionResult in restrictionResults)
            {
                _ = description.Append(description.Length == 0 ? String.Empty : "; ");
                _ = description.AppendFormat(
                        "(ID={0}; Type={1}; Basis={2}; Reason={3})",
                        restrictionResult.Restriction.Id.ToString(),
                        restrictionResult.Restriction.RestrictionType.ToString(),
                        restrictionResult.Restriction.RestrictionBasis.ToString(),
                        restrictionResult.Reason.ToString());
            }

            return description.ToString();
        }

        private void UnplaceSpot(
            SmoothPass smoothPass,
            SmoothSpot smoothSpot,
            SmoothBreak smoothBreak,
            string spotActionMessage,
            ISet<Guid> spotIdsUsed)
        {
            RaiseInfo($"Unplacing booked spot {smoothSpot.Spot.ExternalSpotRef} from break {smoothBreak.TheBreak.ExternalBreakRef}: {spotActionMessage}");

            SpotPlacementService.RemoveSpotFromBreak(
                smoothBreak,
                smoothSpot.Spot,
                spotIdsUsed,
                _sponsorshipRestrictionService
                );

            smoothSpot.IsCurrent = true;

            _smoothDiagnostics.LogSpotAction(
                smoothPass,
                0,
                smoothSpot.Spot,
                smoothBreak,
                SmoothSpot.SmoothSpotActions.RemoveSpotFromBreak,
                spotActionMessage);
        }
    }
}
