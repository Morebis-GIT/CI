using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunLandmarkScheduleSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int RunTypeId { get; set; }

        public bool SendToLandmarkAutomatically { get; set; }

        public string QueueName { get; set; }

        public SortedSet<DayOfWeek> DaysOfWeek { get; set; } = new SortedSet<DayOfWeek>();

        public TimeSpan? ScheduledTime { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }

        public RunType RunType { get; set; }
    }
}
