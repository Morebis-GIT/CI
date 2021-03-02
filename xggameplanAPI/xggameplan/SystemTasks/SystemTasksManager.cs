using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using xggameplan.AuditEvents;
using xggameplan.common.Types;
using xggameplan.RunManagement;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Performs system tasks
    /// </summary>
    public class SystemTasksManager : ISystemTasksManager
    {
        private readonly RootFolder _rootFolder;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAccessTokensRepository _accessTokenRepository;
        private readonly IRunManager _runManager;

        public SystemTasksManager(
            RootFolder rootFolder,
            IAuditEventRepository auditEventRepository, IRepositoryFactory repositoryFactory, IRunManager runManager,
            IAccessTokensRepository accessTokensRepository
            )
        {
            _rootFolder = rootFolder;
            _auditEventRepository = auditEventRepository;
            _repositoryFactory = repositoryFactory;
            _accessTokenRepository = accessTokensRepository;
            _runManager = runManager;
        }

        /// <summary>
        /// Returns whether task exists
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>

        public bool TaskExists(string taskId) => GetSystemTasks().Find(mt => mt.Id.ToUpper() == taskId.ToUpper()) != null;

        /// <summary>
        /// Returns list of maintenance tasks
        /// </summary>
        /// <returns></returns>
        private List<ISystemTask> GetSystemTasks()
        {
            var systemTasks = new List<ISystemTask>
            {
                new SmoothDataTask(_auditEventRepository, _repositoryFactory, TimeSpan.FromDays(30)),
                new DeleteLogsTask(_rootFolder, _auditEventRepository, TimeSpan.FromDays(30)),
                new DeleteRunsTask(_auditEventRepository, _runManager, _repositoryFactory, TimeSpan.FromDays(30)),
                new DeleteAccessTokensTask(_auditEventRepository, _accessTokenRepository),
                new CrashedRunsTask(_runManager)
            };
            return systemTasks;
        }

        /// <summary>
        /// Executes systems task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public List<SystemTaskResult> ExecuteTask(string taskId)
        {
            var allSystemTaskResults = new List<SystemTaskResult>();
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("Executing system task {0}", taskId)));

            try
            {
                // Get task to run
                var systemTask = GetSystemTasks().Where(mt => mt.Id == taskId).First();
                var systemTaskResults = systemTask.Execute();
                allSystemTaskResults.AddRange(systemTaskResults);

                // Log results
                foreach (var systemTaskResult in systemTaskResults)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("System task result for {0}: {1}: {2}", taskId, systemTaskResult.ResultType, systemTaskResult.Message)));
                }
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("Executed system task {0}", taskId)));
            }
            catch (System.Exception exception)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, String.Format("Error executing system task {0}", taskId), exception));
                throw;
            }
            return allSystemTaskResults;
        }
    }
}
