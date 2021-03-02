using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi
{
    public class AutoBook : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string AutoBookId { get; set; }

        public string Api { get; set; }

        public AutoBookStatuses Status { get; set; }

        public AutoBookTask Task { get; set; }

        public bool Locked { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastRunStarted { get; set; }

        public DateTime LastRunCompleted { get; set; }

        public int InstanceConfigurationId { get; set; }
    }

    public enum AutoBookStatuses : short
    {
        Idle = 0,
        Provisioning = 1,
        In_Progress = 2,
        Task_Completed = 3,
        Task_Error = 4,
        Fatal_Error = 5
    }
}
