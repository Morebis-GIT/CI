using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AuditEvents;
using xggameplan.RunManagement;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task for starting next scenario on AutoBook, typically happens when AutoBook becomes idle
    /// </summary>
    public class StartNextScenarioTaskExecutor : TaskExecutorBase, ITaskExecutor
    {
        private readonly IRunManager _runManager;

        public StartNextScenarioTaskExecutor(IRunManager runManager, IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository)
        {
            _runManager = runManager;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public TaskResult Execute(TaskInstance taskInstance)
        {
            var taskResult = new TaskResult();
            string autoBookId = String.Empty;

            try
            {
                StartActiveNotifier(taskInstance.Id);

                using var scope = _repositoryFactory.BeginRepositoryScope();
                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.InProgress, null);

                // Read parameters
                autoBookId = taskInstance.Parameters["AutoBookId"].ToString();
                var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();
                var autoBook = autoBookRepository.Get(autoBookId);
                if (autoBook == null)
                {
                    throw new Exception($"AutoBook {autoBookId} does not exist");
                }
                else
                {
                    _ = _runManager.NextScheduledScenarioStartRun(autoBook);
                }

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.CompletedSuccess, DateTime.UtcNow);
                taskResult.Status = TaskInstanceStatues.CompletedSuccess;
            }
            catch (Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, String.Format("Error executing task for starting next scenario (TaskInstanceID={0}, AutoBookID={1})", taskInstance.Id, autoBookId), exception));

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
