namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes
{
    /// <summary>
    /// Defines order of the Programme.
    /// </summary>
    public enum ProgrammeOrder
    {
        /// <summary>
        /// Ordering by local date of the Programme scheduled date (default).
        /// </summary>
        LocalDate,

        /// <summary>
        /// Ordering by salesArea name.
        /// </summary>
        SalesArea,
    }
}
