namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    /// <summary>
    /// Order of matching each search word
    /// </summary>
    public enum StringMatchWordOrders : byte
    {
        /// <summary>
        /// Target string can contain search words in any order. E.g. If target string="word1 word2 word3" then searching for
        /// "word2 word1" would succeed.
        /// </summary>
        AnyOrder = 0,
        /// <summary>
        /// Target string must contain search words in same order. E.g. If target string="word1 word2 word3" then searching for
        /// "word2 word1" would fail.
        /// </summary>
        ExactOrder = 1
    }
}
