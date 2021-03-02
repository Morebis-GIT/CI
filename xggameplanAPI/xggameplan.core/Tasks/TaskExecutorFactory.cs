using System;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;

namespace xggameplan.core.Tasks
{
    /// <summary>
    /// Factory for ITaskProcessor instances
    /// </summary>
    public class TaskExecutorFactory 
    {
        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly ITaskExecutorResolver _taskExecutorResolver;

        public TaskExecutorFactory(ITaskInstanceRepository taskInstanceRepository, ITaskExecutorResolver taskExecutorResolver)
        {
            _taskInstanceRepository = taskInstanceRepository;
            _taskExecutorResolver = taskExecutorResolver;
        }

        public TaskInstance GetTaskInstance(Guid taskInstanceId)
        {
            return _taskInstanceRepository.Get(taskInstanceId);
        }

        public ITaskExecutor GetTaskProcessor(TaskInstance taskInstance)
        {
            return _taskExecutorResolver.Resolve((taskInstance ?? throw new ArgumentNullException(nameof(taskInstance))).TaskId) ??
                   throw new ArgumentException($"Invalid TaskId {taskInstance.TaskId}");
        }
    }
}
