namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// The level of product category to check for clashes.
    /// </summary>
    public enum ClashCodeLevel
    {
        /// <summary>
        /// <para>
        /// The highest level to check for product clashes.
        /// </para>
        /// <para>
        /// For example, if products X and Y are both alcoholic drinks a clash would occur.
        /// </para>
        /// </summary>
        Parent = 0,

        /// <summary>
        /// <para>
        /// The lowest level to check for product clashes.
        /// </para>
        /// <para>
        /// For example, if products X and Y are both alcoholic drinks but X is a beer and Y is a
        /// wine then a clash would not occur. Only if both X and Y were beer would there be a clash.
        /// </para>
        /// </summary>
        Child = 1
    }
}
