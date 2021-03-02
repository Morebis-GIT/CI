using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Filter criteria for audit events
    /// </summary>
    public class AuditEventFilter
    {
        /// <summary>
        /// Tenant to filter (null=Any)
        /// </summary>
        public int? TenantID = null;

        /// <summary>
        /// User to filter (null=Any)
        /// </summary>
        public int? UserID = null;

        /// <summary>
        /// Source to filter (null=Any)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Minimum time created
        /// </summary>
        public DateTime? MinTimeCreated { get; set; }

        /// <summary>
        /// Maximum time created
        /// </summary>
        public DateTime? MaxTimeCreated { get; set; }

        /// <summary>
        /// Event types to filter
        /// </summary>
        public List<int> EventTypeIds = new List<int>();

        /// <summary>
        /// Whether to return values
        /// </summary>
        public bool IncludeValues { get; set; }

        /// <summary>
        /// Whether to include if all or any filters are met
        /// </summary>
        public bool AllFiltersRequired { get; set; }

        /// <summary>
        /// Filters on values
        /// </summary>
        public List<AuditEventValueFilter> ValueFilters = new List<AuditEventValueFilter>();
    }
}
