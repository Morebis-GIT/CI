using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassBreakExclusion : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public string SalesArea { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public SortedSet<DayOfWeek> SelectableDays { get; set; } = new SortedSet<DayOfWeek>();
    }
}
