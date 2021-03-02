using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class ISRSettingsModel
    {
        public string SalesArea { get; set; }
        public double DefaultEfficiencyThreshold { get; set; }
        //public List<string> BreakTypes { get; set; }    // Should it be a single break type?
        public string BreakType { get; set; }
        public List<DayOfWeek> SelectableDays { get; set; }     // TODO: Make Bitwise?: MTWTFSS
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool ExcludePublicHolidays { get; set; } // Dates from SalesArea.PublicHolidays
        public bool ExcludeSchoolHolidays { get; set; }	// Dates from SalesArea.SchoolHolidays

        public List<ISRDemographicSettingsModel> DemographicsSettings = new List<ISRDemographicSettingsModel>();
    }
}
