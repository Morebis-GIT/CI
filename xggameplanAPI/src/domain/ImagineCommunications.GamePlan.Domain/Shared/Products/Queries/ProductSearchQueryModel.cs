using System;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Shared.Products.Queries
{
    public class ProductSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// Product starting after or on this day(with time) will be returned.
        /// </summary>

        public DateTime FromDateInclusive { get; set; }

        /// <summary>
        /// Product ending before or on this day(with time) will be returned.
        /// </summary>
        public DateTime ToDateInclusive { get; set; }

        /// <summary>
        /// Product have or start with this reference will be returned
        /// </summary>
        public string Externalidentifier { get; set; }

        /// <summary>
        /// Product have or start with this Name will be returned
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Product have or start with this Name or Externalidentifier will be returned
        /// </summary>
        public string NameOrRef { get; set; }
        /// <summary>
        /// Product have or start with this clash will be returned
        /// </summary>
        public string ClashCode { get; set; }

        /// <summary>
        /// If specified, results will be ordered by the given column. If not specified, schedule days will be ordered by local date.
        /// </summary>
        public ProductOrder? OrderBy { get; set; }

        /// <summary>
        /// If specified, given order direction will be used to sort results. If not specified, Ascending ordering will be used.
        /// </summary>
        public OrderDirection? OrderByDirection { get; set; }

        /// <summary>
        /// If specified, response includes actual TotalCount, otherwise 0
        /// </summary>
        public bool IncludeTotalCount { get; set; } 
    }
}
