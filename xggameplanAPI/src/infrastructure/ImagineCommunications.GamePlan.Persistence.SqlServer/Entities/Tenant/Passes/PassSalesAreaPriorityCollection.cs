using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassSalesAreaPriorityCollection
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public bool IsPeakTime { get; set; }
        public bool IsOffPeakTime { get; set; }
        public bool IsMidnightTime { get; set; }
        public bool AreDatesRetained { get; set; }
        public bool AreTimesRetained { get; set; }

        public string DaysOfWeek { get; set; }
        public List<PassSalesAreaPriority> SalesAreaPriorities { get; set; }
    }
}
