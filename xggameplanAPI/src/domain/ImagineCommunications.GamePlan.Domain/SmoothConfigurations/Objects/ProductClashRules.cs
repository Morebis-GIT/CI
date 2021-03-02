namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Rules for product clash exposures
    /// </summary>
    public enum ProductClashRules
    {
        /// <summary>
        /// No clash limit
        /// </summary>
        NoClashLimit = 0,
        /// <summary>
        /// No clashes allowed
        /// </summary>
        NoClashes = 1,
        /// <summary>
        /// Respect clash exposure limit
        /// </summary>
        LimitOnExposureCount = 2
    }
}
