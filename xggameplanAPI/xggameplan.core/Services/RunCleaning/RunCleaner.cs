using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.cloudaccess.Interfaces;
using xggameplan.RunManagement;

namespace xggameplan.core.Services.RunCleaning
{
    /// <summary>
    /// Exposes generic functionality to remove run(s) from database
    /// using appropriate repositories.
    /// </summary>
    /// <seealso cref="xggameplan.core.Services.RunCleaning.RunCleanerBase" />
    public class RunCleaner : RunCleanerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public RunCleaner(IRepositoryFactory repositoryFactory, ICloudStorageV2 cloudStorage) : base(cloudStorage)
        {
            _repositoryFactory = repositoryFactory;
        }

        protected override Task<IReadOnlyCollection<RunDeletionInfo>> GetRunDeletionInfoAsync(
            IReadOnlyCollection<Guid> runIds,
            CancellationToken cancellationToken)
        {
            if (runIds.Count == 0)
            {
                return Task.FromResult<IReadOnlyCollection<RunDeletionInfo>>(null);
            }

            var runs = new List<Run>(runIds.Count);

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                if (runIds.Count == 1)
                {
                    runs.Add(scope.CreateRepository<IRunRepository>().Find(runIds.First()));
                }
                else
                {
                    runs.AddRange(scope.CreateRepository<IRunRepository>().FindByIds(runIds));
                }

                cancellationToken.ThrowIfCancellationRequested();

                return Task.FromResult<IReadOnlyCollection<RunDeletionInfo>>(
                    runs.Where(r => !(r is null)).Select(r => new RunDeletionInfo
                    {
                        RunId = r.Id,
                        ScenarioIds = r.Scenarios?.Select(x => x.ScenarioId).ToArray() ?? Array.Empty<Guid>()
                    }).ToArray());
            }
        }

        protected override Task DeleteRunDataAsync(
            IReadOnlyCollection<RunDeletionInfo> runDeletionInfos,
            CancellationToken cancellationToken)
        {
            var runReset = new RunReset(_repositoryFactory);

            foreach (var runDeletionInfo in runDeletionInfos)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    try
                    {
                        var repositories = scope.CreateRepositories(
                            typeof(IRunRepository),
                            typeof(IScenarioRepository),
                            typeof(IPassRepository),
                            typeof(IScenarioCampaignResultRepository),
                            typeof(IScenarioCampaignFailureRepository),
                            typeof(IPipelineAuditEventRepository),
                            typeof(IAutoBookTaskReportRepository)
                        );

                        var runRepository = repositories.Get<IRunRepository>();
                        var scenarioRepository = repositories.Get<IScenarioRepository>();
                        var passRepository = repositories.Get<IPassRepository>();
                        var scenarioCampaignResultRepository = repositories.Get<IScenarioCampaignResultRepository>();
                        var scenarioCampaignFailureRepository = repositories.Get<IScenarioCampaignFailureRepository>();
                        var pipelineAuditEventRepository = repositories.Get<IPipelineAuditEventRepository>();
                        var autoBookTaskReportRepository = repositories.Get<IAutoBookTaskReportRepository>();

                        // Don't delete ScenarioResults, KPI data is needed
                        foreach (var scenarioId in runDeletionInfo.ScenarioIds)
                        {
                            runReset.ResetScenarioOutputData(
                                scenarioId,
                                new List<string> { "smooth", "autobook", "isr", "rzr" },
                                resetAutoBookFailures: true,
                                resetResultFiles: true);
                        }

                        runReset.ResetSmoothFailures(runDeletionInfo.RunId);

                        foreach (var scenarioId in runDeletionInfo.ScenarioIds)
                        {
                            passRepository.RemoveByScenarioId(scenarioId);
                            scenarioRepository.Delete(scenarioId);
                            scenarioCampaignResultRepository.Delete(scenarioId);
                            scenarioCampaignFailureRepository.RemoveByScenarioId(scenarioId);
                        }

                        var pipelineItems = pipelineAuditEventRepository.GetAllByRunId(runDeletionInfo.RunId);
                        if (pipelineItems != null && pipelineItems.Any())
                        {
                            pipelineAuditEventRepository.DeleteRange(pipelineItems);
                        }

                        pipelineAuditEventRepository.SaveChanges();

                        var autobookTaskReports = autoBookTaskReportRepository.GetAllByRunId(runDeletionInfo.RunId);
                        if (autobookTaskReports != null && autobookTaskReports.Any())
                        {
                            autoBookTaskReportRepository.DeleteRange(autobookTaskReports);
                        }

                        autoBookTaskReportRepository.SaveChanges();

                        runRepository.Remove(runDeletionInfo.RunId);
                        runRepository.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new RunCleaningException(
                            $"Deletion of run with '{runDeletionInfo.RunId}' id has been failed. See inner exception for more details.",
                            ex);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
