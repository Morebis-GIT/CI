using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;

namespace xggameplan.core.Tasks
{
    /// <summary>
    /// Interface for processing a task
    /// </summary>
    public interface ITaskExecutor
    {
        TaskResult Execute(TaskInstance taskSettings);
    }
}
