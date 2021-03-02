using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules
{
    public class ScheduleBreakEfficiency : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScheduleBreakId { get; set; }
        public string Demographic { get; set; }
        public double Efficiency { get; set; }
    }
}
