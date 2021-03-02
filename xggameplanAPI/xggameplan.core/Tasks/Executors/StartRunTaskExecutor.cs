using System;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AuditEvents;
using xggameplan.RunManagement;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task for start run, executes Smooth (if required) and then attempts to start each scenario. If scenario cannot be
    /// started (due to no AutoBook) then an AutoBook will be provisioned and the scenario will be started when it is ready.
    /// </summary>
    public class StartRunTaskExecutor : TaskExecutorBase, ITaskExecutor
    {
        private readonly IRunManager _runManager;

        public StartRunTaskExecutor(IRunManager runManager, IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository)
        {
            _runManager = runManager;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public TaskResult Execute(TaskInstance taskInstance)
        {
            var taskResult = new TaskResult();
            Guid runId = Guid.Empty;

            try
            {
                StartActiveNotifier(taskInstance.Id);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.InProgress, null);

                // Read parameters
                runId = new Guid(taskInstance.Parameters["RunId"].ToString());
                var runInstances = _runManager.AllScenariosStartRun(runId);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.CompletedSuccess, DateTime.UtcNow);
                taskResult.Status = TaskInstanceStatues.CompletedSuccess;
            }
            catch (Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"{exception.StackTrace}"));///testing
                _auditEventRepository.Insert(
                    AuditEventFactory.CreateAuditEventForException(0, 0, $"Error executing task for start run (TaskInstanceID={taskInstance.Id.ToString()}, RunID={runId.ToString()})",
                    exception)
                    );

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
