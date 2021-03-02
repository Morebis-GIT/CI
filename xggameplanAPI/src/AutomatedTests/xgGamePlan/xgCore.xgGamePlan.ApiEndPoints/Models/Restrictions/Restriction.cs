using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Restrictions
{
    public class Restriction
    {

        public Guid Uid { get; set; }
        public string ExternalIdentifier { get; set; }
        public IEnumerable<string> SalesAreas { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        /// <summary>
        /// ”1111111” Days of the week that the restriction applies to Monday to Sunday 
        /// where 1 means applies and 0 means does not - this will always have 7 digits
        /// </summary>
        public string RestrictionDays { get; set; }
        public IncludeOrExcludeOrEither SchoolHolidayIndicator { get; set; }
        public IncludeOrExcludeOrEither PublicHolidayIndicator { get; set; }
        public IncludeOrExclude LiveProgrammeIndicator { get; set; }
        public RestrictionType RestrictionType { get; set; }
        public RestrictionBasis RestrictionBasis { get; set; }
        public string ExternalProgRef { get; set; }
        public string ProgrammeCategory { get; set; }
        public string ProgrammeClassification { get; set; }
        public IncludeOrExclude ProgrammeClassificationIndicator { get; set; }
        public int TimeToleranceMinsBefore { get; set; }
        public int TimeToleranceMinsAfter { get; set; }
        public int IndexType { get; set; }
        public int IndexThreshold { get; set; }
        public int ProductCode { get; set; }
        public string ClashCode { get; set; }
        public string ClearanceCode { get; set; }
        public string ClockNumber { get; set; }
    }
}
