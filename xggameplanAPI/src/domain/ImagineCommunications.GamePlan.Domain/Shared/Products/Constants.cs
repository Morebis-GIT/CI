namespace ImagineCommunications.GamePlan.Domain.Shared.Products
{
    public enum ProductOrder
    {
        /// <summary>
        /// Ordering by date of the product effictive start date
        /// </summary>
        StartDate,

        /// <summary>
        /// Ordering by date of the product effictive end date
        /// </summary>
        EndDate,

        /// <summary>
        /// order by product external identifier
        /// </summary>
        Externalidentifier,

        /// <summary>
        /// order by product name
        /// </summary>
        Name,

        /// <summary>
        /// order by clash code
        /// </summary>
        ClashCode
    }
}
