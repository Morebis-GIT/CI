using System;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;

namespace xggameplan.core.Tasks
{
    public class TaskResult
    {
        public TaskInstanceStatues Status { get; set; }

        public Exception Exception { get; set; }
    }
}
