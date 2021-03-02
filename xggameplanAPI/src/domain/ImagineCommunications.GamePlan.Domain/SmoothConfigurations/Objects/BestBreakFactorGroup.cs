using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Group of items for identifying best break. For each item then a score is calculated and the
    /// Evaluation property determines how we evaluate the item scores to return an overall score.
    /// </summary>
    public class BestBreakFactorGroup
    {
        /// <summary>
        /// <para>Factors</para>
        /// <para>
        /// NOTE: Please be careful about having multiple items. It the scores from two breaks are
        /// being compared for this group then the scores should come from the same item for both groups.
        /// </para>
        /// </summary>
        public List<BestBreakFactorGroupItem> Items { get; }

        /// <summary>
        /// How the scores for this group will be determined from the scores for each item
        /// </summary>
        public BestBreakFactorGroupEvaluation Evaluation { get; }

        /// <summary>
        /// Action if no single break has highest score
        /// </summary>
        public SameBreakGroupScoreActions SameBreakGroupScoreAction { get; }

        /// <summary>
        /// For SameBreakGroupScoreActions.SingleBreakFactorIfBestScoreIsNonZero then indicates the
        /// factor to calculate a score for. It should be a factor that guarantees a unique non-zero
        /// score for each break. E.g. Ascending break position, first break has high score down
        /// to last break that has lowest score.
        /// </summary>
        public BestBreakFactor SameBreakGroupScoreFactor { get; }

        /// <summary>
        /// Sequence for priority
        /// </summary>
        public int Sequence { get; }

        /// <summary>
        /// Name of group
        /// </summary>
        public string Name { get; }

        public BestBreakFactorGroup(
            int sequence,
            string name,
            BestBreakFactorGroupEvaluation evaluation,
            SameBreakGroupScoreActions sameBreakGroupScoreAction,
            BestBreakFactor sameBreakGroupScoreFactor,
            List<BestBreakFactorGroupItem> items)
        {
            Sequence = sequence;
            Name = name;
            Items = items;
            Evaluation = evaluation;
            SameBreakGroupScoreAction = sameBreakGroupScoreAction;
            SameBreakGroupScoreFactor = sameBreakGroupScoreFactor;
        }

        /// <summary>
        /// Calculates the score from the scores for each item
        /// </summary>
        /// <param name="scoreByItem"></param>
        /// <returns></returns>
        public decimal GetGroupScore(List<decimal> scoreByItem)
        {
            switch (Evaluation)
            {
                case BestBreakFactorGroupEvaluation.TotalScore: return scoreByItem.Sum();
                case BestBreakFactorGroupEvaluation.MaxScore: return scoreByItem.Max();
                case BestBreakFactorGroupEvaluation.MinScore: return scoreByItem.Min();
                case BestBreakFactorGroupEvaluation.AvgScore: return scoreByItem.Average();
                case BestBreakFactorGroupEvaluation.FirstNonZeroScore:
                    for (int index = 0; index < scoreByItem.Count; index++)
                    {
                        if (scoreByItem[index] > 0)
                        {
                            return scoreByItem[index];
                        }
                    }
                    break;

                case BestBreakFactorGroupEvaluation.LastNonZeroScore:
                    decimal lastNonZeroScore = 0;
                    for (int index = 0; index < scoreByItem.Count; index++)
                    {
                        if (scoreByItem[index] > 0)
                        {
                            lastNonZeroScore = scoreByItem[index];
                        }
                    }
                    return lastNonZeroScore;
            }
            return 0;
        }
    }
}
