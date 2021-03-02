namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    /// <summary>
    /// How many search words that must match
    /// </summary>
    public enum StringMatchHowManyWordsToMatch : byte
    {
        /// <summary>
        /// Any word must match
        /// </summary>
        AnyWord = 0,
        /// <summary>
        /// All words must match
        /// </summary>
        AllWords = 1
    }
}
