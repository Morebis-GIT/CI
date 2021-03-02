using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class BreakExclusionModel
    {
        public string SalesArea { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public List<DayOfWeek> SelectableDays { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
