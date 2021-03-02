using System;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.common.Services;

namespace xggameplan.core.Tasks
{
    public class TaskUtilities
    {
        /// <summary>
        /// Updates the TaskInstance status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="repositoryFactory"></param>
        /// <param name="status"></param>
        /// <param name="timeCompleted"></param>
        public static void UpdateTaskInstanceStatus(Guid id, IRepositoryFactory repositoryFactory, TaskInstanceStatues? status = null, DateTime? timeCompleted = null, DateTime? timeLastActive = null)
        {
            using (MachineLock.Create($"xggameplan.TaskExecutor.UpdateTaskInstanceStatus.{id}", TimeSpan.FromSeconds(60)))
            using (var scope = repositoryFactory.BeginRepositoryScope())
            {
                var taskInstanceRepository = scope.CreateRepository<ITaskInstanceRepository>();

                var taskInstance = taskInstanceRepository.Get(id);
                if (status != null)
                {
                    taskInstance.Status = status.Value;
                }

                if (timeCompleted != null)
                {
                    taskInstance.TimeCompleted = timeCompleted.Value;
                }

                if (timeLastActive != null)
                {
                    taskInstance.TimeLastActive = timeLastActive.Value;
                }

                taskInstanceRepository.Update(taskInstance);
                taskInstanceRepository.SaveChanges();
            }
        }
    }
}
