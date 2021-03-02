using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.Queries
{
    /// <summary>
    /// Generic result of the search API request operation with paging.
    /// </summary>
    public class SearchResultModel<TItem>
    {
        /// <summary>
        /// Found items which match the search criteria.
        /// </summary>
        public List<TItem> Items { get; set; }

        /// <summary>
        /// Total count of the objects before applying paging.
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Generic result of the DB search operation with paging.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class PagedQueryResult<TItem>
    {

        public PagedQueryResult(int totalCount, IList<TItem> items)
        {
            TotalCount = totalCount;
            Items = items;
        }

        /// <summary>
        /// Total count of the items in the database matching filter criteria, but before applying paging.
        /// </summary>
        public int TotalCount { get; }

        public IList<TItem> Items { get; }
    }
}
