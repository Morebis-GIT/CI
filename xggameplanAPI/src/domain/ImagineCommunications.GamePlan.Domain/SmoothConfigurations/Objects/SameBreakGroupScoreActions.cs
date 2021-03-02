namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Action to take if no single break has highest score for group
    /// </summary>
    public enum SameBreakGroupScoreActions
    {
        /// <summary>
        /// Check next group
        /// </summary>
        CheckNextGroup = 0,
        /// <summary>
        /// Calculate score for single break factor if there are multiple breaks with the same best
        /// score. It is assumed that the particular break factor will identify one break.
        /// </summary>
        UseSingleBreakFactorIfBestScoreIsNonZero = 1
    }
}
