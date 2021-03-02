using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances
{
    /// <summary>
    /// Task instance
    /// </summary>
    public class TaskInstance
    {
        /// <summary>
        /// Instance Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Task ID
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Tenant ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public TaskInstanceStatues Status { get; set; }

        /// <summary>
        /// Time created
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// Time completed
        /// </summary>
        public DateTime TimeCompleted { get; set; }

        /// <summary>
        /// Time last active, enables crashed tasks to be detected.
        /// </summary>
        public DateTime TimeLastActive { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public static int ActiveFrequencySeconds = 60;
    }
}
