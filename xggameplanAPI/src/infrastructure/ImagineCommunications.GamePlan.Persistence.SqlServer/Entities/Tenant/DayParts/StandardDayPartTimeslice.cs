using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts
{
   public class StandardDayPartTimeslice : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int DayPartId { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
