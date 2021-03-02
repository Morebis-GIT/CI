namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Rule for how the group evaluates the scores for all items
    /// </summary>
    public enum BestBreakFactorGroupEvaluation
    {
        /// <summary>
        /// Total score of all group items, default if single item
        /// </summary>
        TotalScore = 0,
        /// <summary>
        /// Max score of all group items
        /// </summary>
        MaxScore = 1,
        /// <summary>
        /// Min score of all group items
        /// </summary>
        MinScore = 2,
        /// <summary>
        /// Average score of all group items
        /// </summary>
        AvgScore = 3,
        /// <summary>
        /// First score that is non-zero (Typically if we have a list of items in priority order)
        /// </summary>
        FirstNonZeroScore = 4,
        /// <summary>
        /// Last score that is non-zero
        /// </summary>
        LastNonZeroScore = 5
    }
}
