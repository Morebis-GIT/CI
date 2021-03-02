using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Clashes
{
    public class ClashDifferenceTimeAndDow
    {
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public List<string> DaysOfWeek { get; set; }
    }
}
