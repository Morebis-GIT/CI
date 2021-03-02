using System;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;

namespace xggameplan.Model
{
    public class UpdateRestrictionInternalModel : IUpdateRestrictionCommand
    {
        /// <summary>
        /// Start date of restriction
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of restriction
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Start time of restriction
        /// </summary>
        public TimeSpan? StartTime { get; set; }

        /// <summary>
        /// End time of restriction
        /// </summary>
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// ”1111111” Days of the week that the restriction applies to Monday to Sunday
        /// where 1 means applies and 0 means does not - this will always have 7 digits
        /// </summary>
        public string RestrictionDays { get; set; }

        /// <summary>
        /// Include / Exclude / Either – if not provided defaulted to Include
        /// </summary>
        public IncludeOrExcludeOrEither SchoolHolidayIndicator { get; set; }

        /// <summary>
        /// Include / Exclude / Either – if not provided defaulted to Include
        /// </summary>
        public IncludeOrExcludeOrEither PublicHolidayIndicator { get; set; }

        /// <summary>
        /// Time tolerance before programme restriction – number in minutes.
        /// Defaulted to 0 mins if not included.
        /// </summary>
        public int TimeToleranceMinsBefore { get; set; }

        /// <summary>
        /// Time tolerance after programme restriction – number in minutes.
        /// Defaulted to 0 mins if not included.
        /// </summary>
        public int TimeToleranceMinsAfter { get; set; }

        /// <summary>
        /// Index Threshold
        /// </summary>
        public int? IndexThreshold { get; set; }
    }
}
