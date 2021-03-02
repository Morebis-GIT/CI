using System;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects
{
    public class ClashDifference
    {
        /// <summary>
        /// A sales area specific difference is defined.If it's not assigned the difference applies to
        /// all sales areas.
        /// </summary>
        public string SalesArea { get; set; }

        /// <summary>
        /// The difference applies only from this date (inclusive).
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The difference applies only until this date (inclusive).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The difference applies only on these days and between the given times on those days.
        /// </summary>
        public TimeAndDowAPI TimeAndDow { get; set; }

        /// <summary>
        /// The clash expose count to use during peak times if the filter criteria matches
        /// </summary>
        public int? PeakExposureCount { get; set; }

        /// <summary>
        /// The clash expose count to use during offpeak times if the filter criteria matches
        /// </summary>
        public int? OffPeakExposureCount { get; set; }
    }
}
