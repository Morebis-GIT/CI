using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;

namespace xggameplan.Model
{
    public class UpdateRestrictionModel : IUpdateRestrictionCommand
    {
        /// <summary>
        /// Sales Area Numbers for the restriction. If it's not assigned the restriction applies to
        /// all sales areas.
        /// </summary>
        public List<string> SalesAreas { get; set; }

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
        /// Include / Exclude – if not present, will assume include
        /// </summary>
        public IncludeOrExclude? LiveProgrammeIndicator { get; set; }

        /// <summary>
        /// Type of Restriction;
        /// 1=Time; 2=Programme/Episode; 3=Programme Category; 4=Index;
        /// </summary>
        public RestrictionType? RestrictionType { get; set; }

        /// <summary>
        /// Basis of restriction – Clearance code, Product, Clash
        /// </summary>
        public RestrictionBasis? RestrictionBasis { get; set; }

        /// <summary>
        /// Programme that has triggered the restriction.
        /// This may or may not be populated based on the restriction type.
        /// </summary>
        public string ExternalProgRef { get; set; }

        /// <summary>
        /// Programme category that has triggered the restriction.
        /// </summary>
        public string ProgrammeCategory { get; set; }

        /// <summary>
        /// Programme Classification
        /// </summary>
        public string ProgrammeClassification { get; set; }

        /// <summary>
        /// Programme Classification Indicator
        /// </summary>
        public IncludeOrExclude? ProgrammeClassificationIndicator { get; set; }

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
        /// Index Type
        /// </summary>
        public int? IndexType { get; set; }

        /// <summary>
        /// Index Threshold
        /// </summary>
        public int? IndexThreshold { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public int? ProductCode { get; set; }

        /// <summary>
        /// Clash code
        /// </summary>
        public string ClashCode { get; set; }

        /// <summary>
        /// Clearance Code
        /// </summary>
        public string ClearanceCode { get; set; }

        /// <summary>
        /// Clock Number / Copy Code / Industry Code
        /// </summary>
        public string ClockNumber { get; set; }
    }
}
