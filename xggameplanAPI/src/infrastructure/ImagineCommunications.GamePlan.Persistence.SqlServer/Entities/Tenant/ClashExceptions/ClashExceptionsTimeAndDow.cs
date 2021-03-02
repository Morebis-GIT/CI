using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions
{
    public class ClashExceptionsTimeAndDow : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int ClashExceptionId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string DaysOfWeek { get; set; }
    }
}
