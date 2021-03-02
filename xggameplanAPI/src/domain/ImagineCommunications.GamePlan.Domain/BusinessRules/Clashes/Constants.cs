namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes
{
    /// <summary>
    /// Defines order of the clash
    /// </summary>
    public enum ClashOrder
    {
        /// <summary>
        /// Order by Clash code
        /// </summary>
        Externalref,

        /// <summary>
        /// Order by parent clash code
        /// </summary>
        ParentExternalidentifier,

        /// <summary>
        /// Order by clash description
        /// </summary>
        Description
    }
}
