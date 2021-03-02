using System;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries
{
    public class ClashExceptionSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// ClashException starting after or on this day(with time) will be returned.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// ClashException Ending before or on this day(with time) will be returned.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// If specified, results will be ordered by the given column.Not specified, result will be ordered by start date
        /// </summary>
        public ClashExceptionOrder OrderBy { get; set; }

        /// <summary>
        /// If specified, given order direction will be used to sort results. If not specified, Ascending ordering will be used.
        /// </summary>
        public OrderDirection OrderByDirection { get; set; }

    }
}
