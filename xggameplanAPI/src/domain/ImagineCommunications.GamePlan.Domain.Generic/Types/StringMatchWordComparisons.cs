namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    /// <summary>
    /// Rules for comparing target word with search word
    /// </summary>
    public enum StringMatchWordComparisons : byte
    {
        /// <summary>
        /// Target word and search word must match exact
        /// </summary>
        ExactWord = 0,
        /// <summary>
        /// Target word must contain search word
        /// </summary>
        ContainsWord = 1,
        /// <summary>
        /// Target word must start with search word
        /// </summary>
        StartsWithWord = 2,
        /// <summary>
        /// Target word must end with search word
        /// </summary>
        EndsWithWord = 3
    }
}
