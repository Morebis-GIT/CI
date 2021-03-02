using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class PassSalesAreaPriority
    {
        public IEnumerable<SalesAreaPriorityModel> SalesAreaPriorities { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string DaysOfWeek { get; set; } //0 or 1 for each day of week
    }
}
