using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class PassSalesAreaPriorityModel
    {
        public List<SalesAreaPriorityModel> SalesAreaPriorities = new List<SalesAreaPriorityModel>();

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public bool IsPeakTime { get; set; }
        public bool IsOffPeakTime { get; set; }
        public bool IsMidnightTime { get; set; }
        public bool AreDatesRetained { get; set; }
        public bool AreTimesRetained { get; set; }

        public String DaysOfWeek { get; set; } //0 or 1 for each day of week

        public object Clone()
        {
            var passSalesAreaPriority = (PassSalesAreaPriorityModel)MemberwiseClone();

            if (SalesAreaPriorities != null)
            {
                passSalesAreaPriority.SalesAreaPriorities = new List<SalesAreaPriorityModel>();
                SalesAreaPriorities.ForEach(s => passSalesAreaPriority.SalesAreaPriorities.Add((SalesAreaPriorityModel)s.Clone()));
            }

            return passSalesAreaPriority;
        }
    }
}
