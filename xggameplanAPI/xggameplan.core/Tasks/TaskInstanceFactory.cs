using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;

namespace xggameplan.core.Tasks
{
    /// <summary>
    /// Factory for TaskInstance instances
    /// </summary>
    public class TaskInstanceFactory
    {
        public static TaskInstance CreateStartRunTaskInstance(int tenantId, Guid runId)
        {
            TaskInstance taskInstance = new TaskInstance()
            {
                Id = Guid.NewGuid(),
                TaskId = TaskIds.StartRun,
                TimeCreated = DateTime.UtcNow,
                TenantId = tenantId
            };
            taskInstance.Parameters.Add("RunId", runId.ToString());
            return taskInstance;
        }

        public static TaskInstance CreateStartNextScenarioTaskInstance(int tenantId, string autoBookId)
        {
            TaskInstance taskInstance = new TaskInstance()
            {
                Id = Guid.NewGuid(),
                TaskId = TaskIds.StartNextScenario,
                TimeCreated = DateTime.UtcNow,
                TenantId = tenantId
            };
            taskInstance.Parameters.Add("AutoBookId", autoBookId);
            return taskInstance;
        }

        public static TaskInstance CreateScenarioCompletedTaskInstance(int tenantId, Guid runId, Guid scenarioId, string autoBookId, AutoBookStatuses autoBookStatus)
        {
            TaskInstance taskInstance = new TaskInstance()
            {
                Id = Guid.NewGuid(),
                TaskId = TaskIds.ScenarioCompleted,
                TimeCreated = DateTime.UtcNow,
                TenantId = tenantId
            };
            taskInstance.Parameters.Add("RunId", runId.ToString());
            taskInstance.Parameters.Add("ScenarioId", scenarioId.ToString());
            taskInstance.Parameters.Add("AutoBookId", autoBookId);
            taskInstance.Parameters.Add("AutoBookStatus", autoBookStatus.ToString());
            return taskInstance;
        }

        public static TaskInstance CreateTestTaskInstance(int tenantId, int seconds)
        {
            TaskInstance taskInstance = new TaskInstance()
            {
                Id = Guid.NewGuid(),
                TaskId = TaskIds.Test,
                TimeCreated = DateTime.UtcNow,
                TenantId = tenantId
            };            
            taskInstance.Parameters.Add("Seconds", seconds.ToString());
            return taskInstance;
        }

    }
}
