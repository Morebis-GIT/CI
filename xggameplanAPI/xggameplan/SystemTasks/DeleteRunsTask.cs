using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.AuditEvents;
using xggameplan.RunManagement;

namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Task for deleting old runs
    /// </summary>
    internal class DeleteRunsTask : ISystemTask
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IRunManager _runManager;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly TimeSpan _runRetention;

        public DeleteRunsTask(
            IAuditEventRepository auditEventRepository,
            IRunManager runManager,
            IRepositoryFactory repositoryFactory,
            TimeSpan runRetention)
        {
            _auditEventRepository = auditEventRepository;
            _runManager = runManager;
            _repositoryFactory = repositoryFactory;
            _runRetention = runRetention;
        }

        public string Id => "DeleteRunsTask";

        public List<SystemTaskResult> Execute()
        {
            IReadOnlyCollection<Run> GetRunsToDeleteForLastNDays(DateTime minCreatedDate)
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var runRepository = scope.CreateRepository<IRunRepository>();

                    return runRepository.GetAll()
                        .Where(r => r.CreatedDateTime.Year > 2000 && r.CreatedDateTime < minCreatedDate)
                        .OrderBy(r => r.CreatedDateTime)
                        .ToList();
                }
            }

            DateTime minCreated = DateTime.UtcNow.Subtract(_runRetention);
            IReadOnlyCollection<Run> runs = GetRunsToDeleteForLastNDays(minCreated);

            var results = new List<SystemTaskResult>();
            foreach (var run in runs)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Deleting run {run.Id} ({run.Description})"
                    ));

                try
                {
                    Run.ValidateForDelete(run);

                    _runManager.DeleteRun(run.Id);

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunDeleted(0, 0, run.Id, null));
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        $"Deleted run {run.Id} ({run.Description})"
                        ));
                }
                catch (Exception exception)
                {
                    results.Add(
                        new SystemTaskResult(SystemTaskResult.ResultTypes.Error, Id,
                            $"Error deleting run {run.Id} ({run.Description}): {exception.Message}"
                            )
                        );
                }
            }

            return results;
        }
    }
}
