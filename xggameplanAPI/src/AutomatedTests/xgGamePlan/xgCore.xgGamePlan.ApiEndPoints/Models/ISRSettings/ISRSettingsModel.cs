using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ISRSettings
{
    public class ISRSettingsModel
    {
        public string SalesArea { get; set; }
        public double DefaultEfficiencyThreshold { get; set; }
        public string BreakType { get; set; }
        public IEnumerable<DayOfWeek> SelectableDays { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool ExcludePublicHolidays { get; set; } 
        public bool ExcludeSchoolHolidays { get; set; }	
    }
}
