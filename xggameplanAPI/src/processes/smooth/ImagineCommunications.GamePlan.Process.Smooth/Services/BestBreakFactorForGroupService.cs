using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class BestBreakFactorForGroupService
    {
        private readonly BestBreakFactorService _bestBreakFactorService;

        public BestBreakFactorForGroupService(
            BestBreakFactorService bestBreakFactorService
            ) => _bestBreakFactorService = bestBreakFactorService;

        /// <summary>
        /// <para>
        /// Calculates the score for adding spots to the break for the group
        /// based on the individual item scores.
        /// </para>
        /// <para>
        /// The score doesn't need to take into account the filtering criteria
        /// that we use in order to filter which break that the spot can be
        /// added to. This would include the criteria checked in
        /// SmoothBreak.ValidateAddSpots or the Exact/Near/Anywhere criteria.
        /// All breaks would have the same score for that particular attribute
        /// and so there's no point in calculating it.
        /// </para>
        /// </summary>
        public (decimal factorScore, string scoreDebug) GetBestBreakFactorScoreForGroup(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<double> randomScoreByBreak,
            BestBreakFactorGroup bestBreakFactorGroup,
            SmoothResources smoothResources)
        {
            var scoreByItem = new List<decimal>();
            var scoreDebug = new StringBuilder();

            foreach (var item in bestBreakFactorGroup.Items)
            {
                var itemScore = GetBestBreakFactorScoreForGroupItem(
                    smoothBreak,
                    spots,
                    spotInfos,
                    clashesByExternalRef,
                    productsByExternalRef,
                    progSmoothBreaks,
                    randomScoreByBreak,
                    item,
                    smoothResources,
                    scoreDebug
                    );

                scoreByItem.Add(itemScore);
            }

            return (
                bestBreakFactorGroup.GetGroupScore(scoreByItem),
                scoreDebug.ToString()
                );
        }

        /// <summary>
        /// Calculates the score for adding spots to the break for the group item
        /// </summary>
        private decimal GetBestBreakFactorScoreForGroupItem(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<double> randomScoreByBreak,
            BestBreakFactorGroupItem item,
            SmoothResources smoothResources,
            StringBuilder scoreDebug)
        {
            if (item.FilterFactors?.Count > 0)
            {
                int countZeroFilterFactorScores = 0;
                int countNonZeroFilterFactorScores = 0;

                foreach (BestBreakFactor bestBreakFactorItem in item.FilterFactors.OrderBy(f => f.Priority))
                {
                    decimal filterFactorScore = _bestBreakFactorService
                        .GetBestBreakFactorScoreForFactor(
                            smoothBreak,
                            spots,
                            spotInfos,
                            clashesByExternalRef,
                            productsByExternalRef,
                            progSmoothBreaks,
                            randomScoreByBreak,
                            bestBreakFactorItem,
                            smoothResources
                        );

                    if (scoreDebug.Length > 0)
                    {
                        _ = scoreDebug.Append("; ");
                    }

                    _ = scoreDebug
                        .Append(bestBreakFactorItem.Factor.ToString())
                        .Append('=')
                        .Append(filterFactorScore.ToString("0.000000000000000000000000000000"));

                    if (filterFactorScore == 0)
                    {
                        countZeroFilterFactorScores++;
                    }
                    else
                    {
                        countNonZeroFilterFactorScores++;
                    }
                }

                if (item.AllFilterFactorsMustBeNonZero && countZeroFilterFactorScores > 0)
                {
                    return 0;
                }

                if (!item.AllFilterFactorsMustBeNonZero && countNonZeroFilterFactorScores == 0)
                {
                    return 0;
                }
            }

            decimal[] scoreByPriority = new decimal[BestBreakFactorGroupItem.MaxBreakFactorPriority];
            foreach (BestBreakFactor defaultFactor in item.DefaultFactors.OrderBy(f => f.Priority))
            {
                decimal factorScore = _bestBreakFactorService
                    .GetBestBreakFactorScoreForFactor(
                        smoothBreak,
                        spots,
                        spotInfos,
                        clashesByExternalRef,
                        productsByExternalRef,
                        progSmoothBreaks,
                        randomScoreByBreak,
                        defaultFactor,
                        smoothResources
                    );

                if (scoreDebug.Length > 0)
                {
                    _ = scoreDebug.Append("; ");
                }

                _ = scoreDebug
                    .Append(defaultFactor.Factor.ToString())
                    .Append('=')
                    .Append(factorScore.ToString("0.000000000000000000000000000000"));

                scoreByPriority[defaultFactor.Priority - 1] += factorScore;
            }

            var prioritiesAdded = new List<int>();
            var scoreByFactor = new List<decimal>();

            foreach (BestBreakFactor defaultFactor in item.DefaultFactors)
            {
                if (prioritiesAdded.Contains(defaultFactor.Priority))
                {
                    continue;
                }

                prioritiesAdded.Add(defaultFactor.Priority);
                scoreByFactor.Add(scoreByPriority[defaultFactor.Priority - 1]);
            }

            if (!item.UseZeroScoresInEvaluation)
            {
                _ = scoreByFactor.RemoveAll(s => s == 0);
            }

            decimal itemScore = item.Evaluation switch
            {
                BestBreakFactorItemEvaluation.AvgScore => scoreByFactor.Average(),
                BestBreakFactorItemEvaluation.AvgScoreButZeroIfAnyFactorScoreIsZero => scoreByFactor.Contains(0) ? 0 : scoreByFactor.Average(),
                BestBreakFactorItemEvaluation.MaxScore => scoreByFactor.Max(),
                BestBreakFactorItemEvaluation.MaxScoreButZeroIfAnyFactorScoreIsZero => scoreByFactor.Contains(0) ? 0 : scoreByFactor.Max(),
                BestBreakFactorItemEvaluation.MinScore => scoreByFactor.Min(),
                BestBreakFactorItemEvaluation.MinScoreButZeroIfAnyFactorScoreIsZero => scoreByFactor.Contains(0) ? 0 : scoreByFactor.Min(),
                BestBreakFactorItemEvaluation.TotalScore => scoreByFactor.Sum(),
                BestBreakFactorItemEvaluation.TotalScoreButZeroIfAnyFactorScoreIsZero => scoreByFactor.Contains(0) ? 0 : scoreByFactor.Sum(),
                _ => 0
            };

            bool isScoreNonZero = itemScore > 0;
            int countZeroScores = scoreByFactor.Count(s => s == 0);
            int countNonZeroScores = scoreByFactor.Count - countZeroScores;

            _ = scoreDebug
                .Append("; ItemScore=")
                .Append(itemScore.ToString("0.000000000000000000000000000000"))
                .Append("; IsScoreNonZero=")
                .Append(isScoreNonZero)
                .Append("; ZeroScores=")
                .Append(countZeroScores)
                .Append("; NonZeroScores=")
                .Append(countNonZeroScores)
                .Append("; Eval=")
                .Append(item.Evaluation.ToString());

            return itemScore;
        }
    }
}
