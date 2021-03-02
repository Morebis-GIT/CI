using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class BreakExclusionModel
    {
        public string SalesArea { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public IEnumerable<DayOfWeek> SelectableDays { get; set; }
    }
}
