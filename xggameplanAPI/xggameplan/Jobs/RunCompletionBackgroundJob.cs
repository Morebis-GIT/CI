using System;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.AuditEvents;
using xggameplan.common.BackgroundJobs;
using xggameplan.RunManagement;

namespace xggameplan.Jobs
{
    public class RunCompletionBackgroundJob : IBackgroundJob
    {
        private Run _run;
        private readonly IRunManager _runManager;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IRepositoryFactory _repositoryFactory;

        public RunCompletionBackgroundJob(
            IRunManager runManager,
            IAuditEventRepository auditEventRepository,
            IRepositoryFactory repositoryFactory)
        {
            _runManager = runManager;
            _auditEventRepository = auditEventRepository;
            _repositoryFactory = repositoryFactory;
        }

        public async Task Execute(Guid runId)
        {
            var runCompleted = false;
            try
            {
                runCompleted = await WaitForRunCompletedAsync(runId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                    $"Completed run notification error (RunID={runId})", ex));
            }
            finally
            {
                if (runCompleted)
                {
                    _runManager.CreateNotificationForCompletedRun(_run);
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        $"Completed run notification has sent (RunID={runId})"));
                }
                else
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        $"Completed run notification warning, Run not found (RunID={runId})"));
                }
            }
        }

        private async Task<bool> WaitForRunCompletedAsync(Guid runId)
        {
            var isRunCompleted = false;
            do
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var runRepository = scope.CreateRepository<IRunRepository>();
                    _run = runRepository.Find(runId);
                    if (_run is null)
                    {
                        return false;
                    }

                    BroadcastScenarioSteps(scope, _run.TotalSteps + GetAdditionalSteps(scope));

                    if (_run.RunStatus == RunStatus.Complete || _run.RunStatus == RunStatus.Errors)
                    {
                        isRunCompleted = true;
                    }
                    else
                    {
                        await Task.Delay(5000).ConfigureAwait(false);
                    }
                }
            } while (!isRunCompleted);

            return true;

            int GetAdditionalSteps(IRepositoryScope scope)
            {
                int additionalSteps = 0;

                var tenantRepository = scope.CreateRepository<ITenantSettingsRepository>();
                if (tenantRepository != null)
                {
                    var tenantSetting = tenantRepository.Get();
                    var runEventSetting = tenantSetting.RunEventSettings.FirstOrDefault();
                    if (runEventSetting != null)
                    {
                        additionalSteps += runEventSetting.Email.Enabled ? 1 : 0;
                        additionalSteps += runEventSetting.HTTP.Enabled ? 1 : 0;
                    }
                }

                return additionalSteps;
            }

            void BroadcastScenarioSteps(IRepositoryScope scope, int totalSteps)
            {
                var pipelineRepository = scope.CreateRepository<IPipelineAuditEventRepository>();
                if (pipelineRepository != null)
                {
                    var allPipelinEventsForRun = pipelineRepository.GetAllByRunId(_run.Id);

                    _run.Scenarios.ForEach(scenario =>
                    {
                        var count = pipelineRepository.GetAllByScenarioId(scenario.Id).Count();
                        count += allPipelinEventsForRun
                            .Where(pipelineAuditEvent => pipelineAuditEvent.ScenarioId == Guid.Empty)
                            .Count();

                        _runManager.BroadcastScenario(
                         scenario.Id,
                         scenario.Status,
                         count > 0 ? count : 1,
                         totalSteps);
                    });
                }
            }
        }
    }
}
