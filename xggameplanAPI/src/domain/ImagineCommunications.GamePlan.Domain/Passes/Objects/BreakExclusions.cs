using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    public class BreakExclusion : ICloneable
    {
        public string SalesArea { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public List<DayOfWeek> SelectableDays { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
