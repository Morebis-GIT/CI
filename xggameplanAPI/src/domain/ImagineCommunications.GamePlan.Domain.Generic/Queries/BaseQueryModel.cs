namespace ImagineCommunications.GamePlan.Domain.Generic.Queries
{
    public abstract class BaseQueryModel
    {
        /// <summary>
        /// If specified, the number of results will be limited to the given number. Useful for server-side paging.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// If specified, the given number of results will be skipped. Useful for server-side paging.
        /// </summary>
        public int? Skip { get; set; }
    }
}
