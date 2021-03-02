using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
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
        private readonly IReadOnlyCollection<Programme> _allProgrammesForPeriodAndSalesArea;
        private readonly SmoothProgramme _smoothProgramme;

        private static readonly IReadOnlyList<SmoothFailureMessages> _sponsorshipRestrictionFailures =
            new SmoothFailureMessages[] {
                SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash,
                SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser
                };

        public SmoothPassBookedExecuter(
            ISmoothDiagnostics smoothDiagnostics,
            SmoothResources smoothResources,
            SmoothProgramme smoothProgramme,
            SponsorshipRestrictionService sponsorshipRestrictionService,
            IReadOnlyCollection<Programme> allProgrammesForPeriodAndSalesArea,
            Action<string> raiseInfo,
            Action<string, Exception> raiseException
            )
        {
            _smoothDiagnostics = smoothDiagnostics;
            _smoothResources = smoothResources;
            _sponsorshipRestrictionService = sponsorshipRestrictionService;
            _allProgrammesForPeriodAndSalesArea = allProgrammesForPeriodAndSalesArea;
            _smoothProgramme = smoothProgramme;

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
        /// <param name="breaksForThePeriodBeingSmoothed"></param>
        /// <param name="spotIdsUsed"></param>
        /// <returns></returns>
        public SmoothPassResult Execute(
            SmoothPassBooked smoothPass,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            ISet<Guid> spotIdsUsed
            )
        {
            var smoothPassResult = new SmoothPassResult(smoothPass.Sequence);
            var programme = _smoothProgramme.Programme;
            var salesArea = _smoothProgramme.SalesArea;

            // Check booked spots within each break
            foreach (var smoothBreak in _smoothProgramme.ProgrammeSmoothBreaks)
            {
                var breakSpotsToCheck = new List<SmoothSpot>(
                    smoothBreak.SmoothSpots.Where(s => !s.IsCurrent));

                Break theBreak = smoothBreak.TheBreak;

                bool AnySponsorshipRestrictions(Spot spotToCheckForRestrictions)
                {
                    IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result =
                        _sponsorshipRestrictionService.CheckSponsorshipRestrictions(
                            spotToCheckForRestrictions,
                            theBreak.ExternalBreakRef,
                            theBreak.ScheduledDate,
                            theBreak.Duration,
                            smoothBreak.SmoothSpots.Select(s => s.Spot).ToList()
                            );

                    return result
                        .Where(r => r.spotUid == spotToCheckForRestrictions.Uid)
                        .Select(r => r.failureMessage)
                        .Any(r => _sponsorshipRestrictionFailures.Contains(r));
                }

                foreach (var smoothSpot in breakSpotsToCheck)
                {
                    Spot spot = smoothSpot.Spot;

                    try
                    {
                        if (AnySponsorshipRestrictions(spot))
                        {
                            UnplaceRestrictedSpot(
                                smoothPass,
                                smoothSpot,
                                smoothBreak,
                                "Sponsorship restrictions found: spot is a competitor to the sponsor",
                                spotIdsUsed
                                );

                            continue;
                        }

                        var restrictionResults = _smoothResources.RestrictionChecker
                            .CheckRestrictions(
                                programme,
                                theBreak,
                                spot,
                                salesArea,
                                breaksForThePeriodBeingSmoothed,
                                _allProgrammesForPeriodAndSalesArea);

                        if (restrictionResults.Count == 0)
                        {
                            continue;
                        }

                        UnplaceRestrictedSpot(
                            smoothPass,
                            smoothSpot,
                            smoothBreak,
                            $"Restrictions {GetRestrictionResultsDescription(restrictionResults)}",
                            spotIdsUsed
                            );

                        smoothPassResult.BookedSpotIdsUnplacedDueToRestrictions.Add(spot.Uid);
                    }
                    catch (Exception exception)
                    {
                        RaiseException(
                            $"Error checking booked spot {spot.ExternalSpotRef} " +
                            $"on pass {Log(smoothPass.Sequence)}",
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

                Restriction restriction = restrictionResult.Restriction;

                _ = description.AppendFormat(
                        "(ID={0}; Type={1}; Basis={2}; Reason={3})",
                        restriction.Id.ToString(),
                        restriction.RestrictionType.ToString(),
                        restriction.RestrictionBasis.ToString(),
                        restrictionResult.Reason.ToString());
            }

            return description.ToString();
        }

        /// <summary>Unplaces the restricted spot.</summary>
        /// <param name="smoothPass">The smooth pass.</param>
        /// <param name="smoothSpot">The smooth spot.</param>
        /// <param name="smoothBreak">The smooth break.</param>
        /// <param name="spotActionMessage">The spot action message.</param>
        /// <param name="spotIdsUsed">The spot ids used.</param>
        private void UnplaceRestrictedSpot(
            SmoothPass smoothPass,
            SmoothSpot smoothSpot,
            SmoothBreak smoothBreak,
            string spotActionMessage,
            ISet<Guid> spotIdsUsed)
        {
            RaiseInfo(
                $"Unplacing booked spot {smoothSpot.Spot.ExternalSpotRef} " +
                $"from break {smoothBreak.TheBreak.ExternalBreakRef}: {spotActionMessage}"
                );

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
