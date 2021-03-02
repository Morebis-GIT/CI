using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models
{
    public class TimeAndDow
    {
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string DaysOfWeek { get; set; }
    }
}
