using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.AuditEvents;

namespace xggameplan.core.Tasks.Executors
{
    /// <summary>
    /// Executes task for test, allows us to test that the mechanism for executing tasks in a separate process
    /// </summary>
    internal class TestTaskExecutor : TaskExecutorBase, ITaskExecutor
    {
        public TestTaskExecutor(IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository)
        {
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public TaskResult Execute(TaskInstance taskInstance)
        {
            var taskResult = new TaskResult();

            try
            {
                StartActiveNotifier(taskInstance.Id);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.InProgress, null);

                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    // Test repository, confirms that we're able to pick up config settings
                    var campaignRepository = scope.CreateRepository<ICampaignRepository>();
                    var campaigns = campaignRepository.GetAll();
                }

                // Do work
                int seconds = Convert.ToInt32(taskInstance.Parameters["Seconds"].ToString());
                DoWork(seconds);

                TaskUtilities.UpdateTaskInstanceStatus(taskInstance.Id, _repositoryFactory, TaskInstanceStatues.CompletedSuccess, DateTime.UtcNow);
                taskResult.Status = TaskInstanceStatues.CompletedSuccess;
            }
            catch (Exception exception)
            {
                _auditEventRepository.Insert(
                    AuditEventFactory.CreateAuditEventForException(0, 0, $"Error executing task for test (TaskInstanceID={taskInstance.Id.ToString()})", exception)
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

        private void DoWork(int seconds)
        {
            // Simulate delay peforming work
            DateTime wait = DateTime.UtcNow.AddSeconds(seconds);
            do
            {
                System.Threading.Thread.Sleep(200);
            } while (DateTime.UtcNow < wait);
        }
    }
}
