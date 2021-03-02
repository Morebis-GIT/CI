using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Item for identifying best break, contains a number of factors
    /// </summary>
    public class BestBreakFactorGroupItem
    {
        /// <summary>
        /// Max number of break factor priorities supported. This is limited by
        /// the size of the number that can store the total score for all factors.
        /// </summary>
        public static int MaxBreakFactorPriority = 6;

        /// <summary>
        /// Default factors
        /// </summary>
        public List<BestBreakFactor> DefaultFactors { get; }

        /// <summary>
        /// Filter factors
        /// </summary>
        public List<BestBreakFactor> FilterFactors { get; }

        public bool AllFilterFactorsMustBeNonZero { get; }

        /// <summary>
        /// Whether to consider zero scores in evaluation. E.g. If using average
        /// score then it controls whether the average includes the zero scores.
        /// </summary>
        public bool UseZeroScoresInEvaluation { get; }

        /// <summary>
        /// How all factors will be evaluated to calculate the score for this item
        /// </summary>
        public BestBreakFactorItemEvaluation Evaluation { get; }

        public BestBreakFactorGroupItem(
            BestBreakFactorItemEvaluation evaluation,
            bool useZeroScoresInEvaluation,
            bool allFilterFactorsMustBeNonZero,
            List<BestBreakFactor> filterFactors,
            List<BestBreakFactor> defaultFactors)
        {
            AllFilterFactorsMustBeNonZero = allFilterFactorsMustBeNonZero;
            FilterFactors = filterFactors;
            DefaultFactors = defaultFactors;
            UseZeroScoresInEvaluation = useZeroScoresInEvaluation;
            Evaluation = evaluation;
        }
    }
}
