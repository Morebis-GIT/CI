using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AuditEvents;
using xggameplan.core.RunManagement;
using xggameplan.RunManagement;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task for scenario completed
    /// </summary>
    internal class ScenarioCompletedTaskExecutor : TaskExecutorBase, ITaskExecutor
    {
        private readonly RunInstanceCreator _runInstanceCreator;

        public ScenarioCompletedTaskExecutor(RunInstanceCreator runInstanceCreator, IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository)
        {
            _runInstanceCreator = runInstanceCreator;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public TaskResult Execute(TaskInstance taskInstance)
        {
            var taskResult = new TaskResult();
            Guid runId = Guid.Empty;
            Guid scenarioId = Guid.Empty;
            string autoBookId = String.Empty;

            try
            {
                StartActiveNotifier(taskInstance.Id);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.InProgress, null);

                // Read parameters
                runId = new Guid(taskInstance.Parameters["RunId"]);
                scenarioId = new Guid(taskInstance.Parameters["ScenarioId"]);
                autoBookId = taskInstance.Parameters["AutoBookId"];
                AutoBookStatuses autoBookStatus = (AutoBookStatuses)Enum.Parse(typeof(AutoBookStatuses), taskInstance.Parameters["AutoBookStatus"], true);

                using var scope = _repositoryFactory.BeginRepositoryScope();
                // Get AutoBook instance
                var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();
                var autoBook = autoBookRepository.Get(autoBookId);

                RunInstance runInstance = _runInstanceCreator.Create(runId, scenarioId);

                // Check AutoBook status
                switch (autoBookStatus)
                {
                    case AutoBookStatuses.Task_Completed:
                        runInstance.HandleCompletedSuccess();
                        break;

                    case AutoBookStatuses.Task_Error:
                        runInstance.HandleCompletedTaskError();
                        break;

                    case AutoBookStatuses.Fatal_Error:
                        runInstance.HandleCompletedFatalError(autoBook);
                        break;
                }

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory,
                    TaskInstanceStatues.CompletedSuccess, DateTime.UtcNow);
                taskResult.Status = TaskInstanceStatues.CompletedSuccess;
            }
            catch (Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error executing task for scenario completed (TaskInstanceID={0}, RunID={1}, ScenarioID={2}, AutoBookID={3})", taskInstance.Id, runId, scenarioId, autoBookId), exception));

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
