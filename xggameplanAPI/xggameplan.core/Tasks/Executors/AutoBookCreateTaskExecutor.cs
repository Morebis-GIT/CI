using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AutoBooks.Abstractions;
using AutoBookDomainObject = ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects.AutoBook;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task for creating AutoBook
    /// </summary>
    internal class AutoBookCreateTaskExecutor : TaskExecutorBase, ITaskExecutor
    {
        private readonly IAutoBooks _autoBooks;

        public AutoBookCreateTaskExecutor(IAutoBooks autoBooks, IRepositoryFactory repositoryFactory)
        {
            _autoBooks = autoBooks;
            _repositoryFactory = repositoryFactory;
        }

        public TaskResult Execute(TaskInstance taskInstance)
        {
            var taskResult = new TaskResult();

            try
            {
                StartActiveNotifier(taskInstance.Id);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.InProgress, null);

                int instanceConfigurationId = Convert.ToInt16(taskInstance.Parameters["InstanceConfigurationId"]);
                var newAutoBook = new AutoBookDomainObject()
                {
                    TimeCreated = DateTime.UtcNow,
                    Locked = false,
                    Status = AutoBookStatuses.Provisioning,
                    InstanceConfigurationId = instanceConfigurationId
                };

                _autoBooks.Create(newAutoBook);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.CompletedSuccess, DateTime.UtcNow);
                taskResult.Status = TaskInstanceStatues.CompletedSuccess;
            }
            catch (Exception exception)
            {
                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.CompletedError, DateTime.UtcNow);
                taskResult.Status = TaskInstanceStatues.CompletedError;
                taskResult.Exception = exception;
            }
            finally
            {
                StopActiveNotifier();
            }
            return taskResult;
        }
    }
}
