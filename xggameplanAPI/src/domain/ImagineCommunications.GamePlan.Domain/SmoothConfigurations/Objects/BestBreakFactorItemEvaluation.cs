namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Rule for how the item evaluates the scores for each factor.
    ///
    /// The [Something]ButZeroIfAnyFactorScoreIsZero values are typically used where we have
    /// multiple factors and we want all to have a non-zero score otherwise we return a zero score.
    /// </summary>
    public enum BestBreakFactorItemEvaluation
    {
        /// <summary>
        /// Total score of all factors, default if single item
        /// </summary>
        TotalScore = 0,
        /// <summary>
        /// Total score of all factors but zero if any factor score is zero
        /// </summary>
        TotalScoreButZeroIfAnyFactorScoreIsZero = 1,
        /// <summary>
        /// Max score of all factors
        /// </summary>
        MaxScore = 2,
        /// <summary>
        /// Max score of all factors but zero if any factor score is zero
        /// </summary>
        MaxScoreButZeroIfAnyFactorScoreIsZero = 3,
        /// <summary>
        /// Min score of all factors
        /// </summary>
        MinScore = 4,
        /// <summary>
        ///  Min score of all factors but zero if any factor score is zero
        /// </summary>
        MinScoreButZeroIfAnyFactorScoreIsZero = 5,
        /// <summary>
        /// Average score of all factors
        /// </summary>
        AvgScore = 6,
        /// <summary>
        /// Average score of all factors but zero if any factor score is zero
        /// </summary>
        AvgScoreButZeroIfAnyFactorScoreIsZero = 7,
    }
}
