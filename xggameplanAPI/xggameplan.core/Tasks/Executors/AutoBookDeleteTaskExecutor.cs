using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AutoBooks.Abstractions;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task for deleting AutoBook
    /// </summary>
    internal class AutoBookDeleteTaskExecutor : TaskExecutorBase, ITaskExecutor
    {
        private readonly IAutoBooks _autoBooks;

        public AutoBookDeleteTaskExecutor(IAutoBooks autoBooks, IRepositoryFactory repositoryFactory)
        {
            _autoBooks = autoBooks;
            _repositoryFactory = repositoryFactory;
        }

        public TaskResult Execute(TaskInstance taskInstance)
        {
            TaskResult taskResult = new TaskResult();

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    StartActiveNotifier(taskInstance.Id);

                    TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.InProgress, null);

                    var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();
                    string autoBookId = taskInstance.Parameters["AutoBookId"].ToString();
                    var autoBook = autoBookRepository.Get(autoBookId);

                    _autoBooks.Delete(autoBook);

                    TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.CompletedSuccess, DateTime.UtcNow);
                    taskResult.Status = TaskInstanceStatues.CompletedSuccess;
                }
            }
            catch (System.Exception exception)
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
