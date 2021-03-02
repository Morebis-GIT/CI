using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries
{
    public class ProgrammeSearchQueryModel : BaseQueryModel
    {
        /// <summary>
        /// Programme starting after or on this day(with time) will be returned.
        /// </summary>

        public DateTime FromDateInclusive { get; set; }

        /// <summary>
        /// Programme ending before or on this day(with time) will be returned.
        /// </summary>

        public DateTime ToDateInclusive { get; set; }

        /// <summary>
        /// programme scheduled for this salesArea will be returned
        /// </summary>
        public List<string> SalesArea { get; set; }

        /// <summary>
        ///  programmes have or start with this Name or external reference will be returned
        /// </summary>
        public string NameOrRef { get; set; }

        /// <summary>
        /// If specified, results will be ordered by the given column. If not specified, schedule days will be ordered by local date.
        /// </summary>
        public ProgrammeOrder? OrderBy { get; set; }

        /// <summary>
        /// If specified, given order direction will be used to sort results. If not specified, Ascending ordering will be used.
        /// </summary>
        public OrderDirection? OrderByDirection { get; set; }
    }
}
