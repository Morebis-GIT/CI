using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassBreakExclusion : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public Guid SalesAreaId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public SortedSet<DayOfWeek> SelectableDays { get; set; } = new SortedSet<DayOfWeek>();
        public SalesArea SalesArea { get; set; }
    }
}
