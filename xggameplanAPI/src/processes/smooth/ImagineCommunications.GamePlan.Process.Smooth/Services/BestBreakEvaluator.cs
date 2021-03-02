using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// <para>
    /// Evaluates the best break to add the spot(s) to based on a list of factor groups.
    /// </para>
    /// <para>
    /// We process each group in priority order and, for each break, we
    /// calculate a score. If a single break has the best score then we return
    /// that break.
    /// </para>
    /// </summary>
    internal class BestBreakEvaluator
    {
        private readonly BestBreakFactorService _bestBreakFactorService;
        private readonly BestBreakFactorForGroupService _bestBreakFactorForGroupService;
        private readonly ISmoothDiagnostics _smoothDiagnostics;
        private readonly SmoothResources _smoothResources;

        public BestBreakEvaluator(
            ISmoothDiagnostics smoothDiagnostics,
            SmoothResources smoothResources,
            IClashExposureCountService clashExposureCountService)
        {
            _smoothDiagnostics = smoothDiagnostics;
            _smoothResources = smoothResources;

            _bestBreakFactorService = new BestBreakFactorService(clashExposureCountService);
            _bestBreakFactorForGroupService = new BestBreakFactorForGroupService(_bestBreakFactorService);
        }

        private void LogBestBreakFactorScore(
            SmoothPass smoothPass,
            SmoothPassDefaultIteration smoothPassIteration,
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            decimal breakScore,
            BestBreakFactorGroup bestBreakFactorGroup,
            string scoreDebug,
            string type)
        {
            var spotsDuration = Duration.Zero;
            foreach (var spot in spots)
            {
                spotsDuration = spotsDuration.Plus(spot.SpotLength);
            }

            string spotList = SpotUtilities.GetSpotDetailsListString(spots);

            var message = new StringBuilder(128);
            _ = message
                .AppendFormat("Break={0}; ", smoothBreak.TheBreak.ExternalBreakRef)
                .AppendFormat("BreakDur={0} sec(s);", ((int)smoothBreak.TheBreak.Duration.ToTimeSpan().TotalSeconds).ToString())
                .AppendFormat("Avail={0}s;", ((int)smoothBreak.RemainingAvailability.ToTimeSpan().TotalSeconds).ToString())
                .AppendFormat("SpotsDetails={0}; ", spotList)
                .AppendFormat("SpotsLength={0}s; ", ((int)spotsDuration.ToTimeSpan().TotalSeconds).ToString())
                .AppendFormat("ScoreDebug={0}; ", scoreDebug)
                .AppendFormat("Type={0}", type)
                ;

            _smoothDiagnostics.LogBestBreakFactorMessage(
                smoothPass,
                smoothPassIteration,
                bestBreakFactorGroup,
                smoothBreak.TheBreak,
                spots,
                breakScore,
                message.ToString());
        }

        /// <summary>
        /// Returns best break to add spot(s) to.
        /// </summary>
        public BestBreakResult GetBestBreak(
            SmoothPass smoothPass,
            SmoothPassDefaultIteration smoothPassIteration,
            IReadOnlyCollection<BestBreakFactorGroup> bestBreakFactorGroups,
            IReadOnlyCollection<SmoothBreak> validSmoothBreaks,
            IReadOnlyCollection<Spot> spotsToPlace,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            out BestBreakFactorGroup usedBestBreakFactorGroup)
        {
            var bestBreakResult = new BestBreakResult();
            usedBestBreakFactorGroup = null;

            string breakDetailsListString = BreakUtilities.GetListOfBreakExternalReferences(
                "; ",
                validSmoothBreaks.Select(sb => sb.TheBreak).ToList());

            // Get spot details list as string
            string spotDetailsListString = SpotUtilities.GetSpotDetailsListString(spotsToPlace);

            // Check each group in priority order
            foreach (var bestBreakFactorGroup in bestBreakFactorGroups.OrderBy(g => g.Sequence))
            {
                _smoothDiagnostics.LogBestBreakFactorMessage(
                    smoothPass,
                    smoothPassIteration,
                    bestBreakFactorGroup,
                    null,
                    spotsToPlace,
                    null,
                    $"Checking best break factor group (Breaks={breakDetailsListString}; Spots={spotDetailsListString})"
                    );

                // Set all breaks to check
                var smoothBreaksRemaining = new List<SmoothBreak>();
                smoothBreaksRemaining.AddRange(validSmoothBreaks);

                // Generate unique random score for each break so that we can
                // pick a random break if necessary.
                IReadOnlyCollection<double> randomScoreByBreak = GetRandomScoreByBreak(
                    progSmoothBreaks,
                    uniqueScore: true);

                // Calculate scores for each break, determine best score
                var scoreByBreak = new Dictionary<SmoothBreak, decimal>();
                decimal bestBreakScore = -1;

                foreach (var smoothBreak in smoothBreaksRemaining)
                {
                    var (breakScore, scoreDebug) = _bestBreakFactorForGroupService
                        .GetBestBreakFactorScoreForGroup(
                            smoothBreak,
                            spotsToPlace,
                            spotInfos,
                            clashesByExternalRef,
                            productsByExternalRef,
                            progSmoothBreaks,
                            randomScoreByBreak,
                            bestBreakFactorGroup,
                            _smoothResources
                            );

                    LogBestBreakFactorScore(
                        smoothPass,
                        smoothPassIteration,
                        smoothBreak,
                        spotsToPlace,
                        breakScore,
                        bestBreakFactorGroup,
                        "Main",
                        scoreDebug
                        );

                    scoreByBreak.Add(smoothBreak, breakScore);

                    if (breakScore > bestBreakScore || bestBreakScore == -1)
                    {
                        bestBreakScore = breakScore;
                    }
                }

                if (bestBreakScore > 0)   // At least one break has a non-zero score
                {
                    // Remove all breaks with score less than the best score
                    var smoothBreaksToRemove = new List<SmoothBreak>();
                    foreach (var smoothBreak in smoothBreaksRemaining)
                    {
                        if (scoreByBreak[smoothBreak] < bestBreakScore)
                        {
                            smoothBreaksToRemove.Add(smoothBreak);
                        }
                    }

                    while (smoothBreaksToRemove.Count > 0)
                    {
                        _ = smoothBreaksRemaining.Remove(smoothBreaksToRemove[0]);
                        smoothBreaksToRemove.RemoveAt(0);
                    }

                    // Single break with best score, return it
                    if (smoothBreaksRemaining.Count == 1)
                    {
                        _smoothDiagnostics.LogBestBreakFactorMessage(
                            smoothPass,
                            smoothPassIteration,
                            bestBreakFactorGroup,
                            smoothBreaksRemaining[0].TheBreak,
                            spotsToPlace,
                            bestBreakScore,
                            $"Using single break which has best score (Spots={spotDetailsListString})"
                            );

                        usedBestBreakFactorGroup = bestBreakFactorGroup;
                        bestBreakResult.Score = bestBreakScore;
                        bestBreakResult.SmoothBreak = smoothBreaksRemaining[0];

                        return bestBreakResult;
                    }

                    // Multiple breaks with best score, determine what we do
                    if (smoothBreaksRemaining.Count > 1)
                    {
                        switch (bestBreakFactorGroup.SameBreakGroupScoreAction)
                        {
                            // Check next group, our default and so no action
                            // required here
                            case SameBreakGroupScoreActions.CheckNextGroup:
                                _smoothDiagnostics.LogBestBreakFactorMessage(
                                    smoothPass,
                                    smoothPassIteration,
                                    bestBreakFactorGroup,
                                    null,
                                    spotsToPlace,
                                    null,
                                    "CheckNextGroup: Need to check next group"
                                    );
                                break;

                            // Check single factor to identify single break
                            case SameBreakGroupScoreActions.UseSingleBreakFactorIfBestScoreIsNonZero:
                                // Calculate score for each remaining break
                                _smoothDiagnostics.LogBestBreakFactorMessage(
                                    smoothPass,
                                    smoothPassIteration,
                                    bestBreakFactorGroup,
                                    null,
                                    spotsToPlace,
                                    null,
                                    $"UseSingleBreakFactorIfBestScoreIsNonZero: Finding break for single break factor score for group (BreaksRemaining={smoothBreaksRemaining.Count.ToString()})"
                                    );

                                var scoreByBreak2 = new Dictionary<SmoothBreak, decimal>();
                                decimal bestBreakScore2 = -1;

                                foreach (var smoothBreak in smoothBreaksRemaining)
                                {
                                    decimal breakScore2 = _bestBreakFactorService
                                        .GetBestBreakFactorScoreForFactor(
                                            smoothBreak,
                                            spotsToPlace,
                                            spotInfos,
                                            clashesByExternalRef,
                                            productsByExternalRef,
                                            progSmoothBreaks,
                                            randomScoreByBreak,
                                            bestBreakFactorGroup.SameBreakGroupScoreFactor,
                                            _smoothResources
                                        );

                                    var scoreDebug = $"UseSingleBreakFactorIfBestScoreIsNonZero: {bestBreakFactorGroup.SameBreakGroupScoreFactor.Factor.ToString()}={breakScore2.ToString("0.000000000000000000000000000000")}";

                                    LogBestBreakFactorScore(
                                        smoothPass,
                                        smoothPassIteration,
                                        smoothBreak,
                                        spotsToPlace,
                                        breakScore2,
                                        bestBreakFactorGroup,
                                        "",
                                        scoreDebug
                                        );

                                    scoreByBreak2.Add(smoothBreak, breakScore2);
                                    if (breakScore2 > bestBreakScore2 || bestBreakScore2 == -1)
                                    {
                                        bestBreakScore2 = breakScore2;
                                    }
                                }

                                // Remove all but the break with the best score
                                var smoothBreaksToRemove2 = new List<SmoothBreak>();
                                foreach (var smoothBreak in smoothBreaksRemaining)
                                {
                                    if (scoreByBreak2[smoothBreak] < bestBreakScore2)
                                    {
                                        smoothBreaksToRemove2.Add(smoothBreak);
                                    }
                                }

                                while (smoothBreaksToRemove2.Count > 0)
                                {
                                    _ = smoothBreaksRemaining.Remove(smoothBreaksToRemove2[0]);
                                    smoothBreaksToRemove2.RemoveAt(0);
                                }

                                // Return first break Sanity check, if no break
                                // then check next group
                                if (smoothBreaksRemaining.Count > 0)
                                {
                                    _smoothDiagnostics.LogBestBreakFactorMessage(
                                        smoothPass,
                                        smoothPassIteration,
                                        bestBreakFactorGroup,
                                        smoothBreaksRemaining[0].TheBreak,
                                        spotsToPlace,
                                        null,
                                        string.Format("UseSingleBreakFactorIfBestScoreIsNonZero: Found break for single break factor score for group (Spots={0}; BreaksRemaining={1})", spotDetailsListString, smoothBreaksRemaining.Count)
                                        );

                                    usedBestBreakFactorGroup = bestBreakFactorGroup;
                                    bestBreakResult.Score = bestBreakScore2;
                                    bestBreakResult.SmoothBreak = smoothBreaksRemaining[0];
                                    return bestBreakResult;
                                }
                                else
                                {
                                    _smoothDiagnostics.LogBestBreakFactorMessage(
                                        smoothPass,
                                        smoothPassIteration,
                                        bestBreakFactorGroup,
                                        null,
                                        spotsToPlace,
                                        null,
                                        string.Format("UseSingleBreakFactorIfBestScoreIsNonZero: WARNING: Not found break for single break factor score for group (Spots={0}; BreaksRemaining={1})", spotDetailsListString, smoothBreaksRemaining.Count)
                                        );
                                }

                                break;
                        }
                    }
                }
                else    // No break has a non-zero score, check next group
                {
                    _smoothDiagnostics.LogBestBreakFactorMessage(
                        smoothPass,
                        smoothPassIteration,
                        bestBreakFactorGroup,
                        null,
                        spotsToPlace,
                        null,
                        "No breaks with highest score, checking next group"
                        );
                }
            }

            _smoothDiagnostics.LogBestBreakFactorMessage(
                smoothPass,
                smoothPassIteration,
                null,
                null,
                spotsToPlace,
                null,
                "No best break for spots"
                );

            // All groups checked, no best break, shouldn't really happen,
            // possibly bad configuration
            return bestBreakResult;
        }

        /// <summary>
        /// Generates random score for each break
        /// </summary>
        /// <param name="progSmoothBreaks"></param>
        /// <param name="uniqueScore"></param>
        /// <returns></returns>
        private static IReadOnlyCollection<double> GetRandomScoreByBreak(
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            bool uniqueScore)
        {
            var date = DateTime.UtcNow;
            var random = new Random(date.Millisecond);
            var scores = new List<double>();

            foreach (var _ in progSmoothBreaks)
            {
                double score = 0;
                while (score == 0)
                {
                    score = random.Next(1, 1000);
                    if (uniqueScore && scores.Contains(score))
                    {
                        score = 0;
                    }
                }

                scores.Add(score);
            }

            return scores;
        }
    }
}
