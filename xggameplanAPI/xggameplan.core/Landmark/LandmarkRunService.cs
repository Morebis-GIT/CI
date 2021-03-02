using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.Hubs;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.core.OutputProcessors.Processors;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;

namespace xggameplan.core.Landmark
{
    public class LandmarkRunService : ILandmarkRunService
    {
        private const string KpiFileName = "kpi.out";
        private const string LmkKpiFileName = "lmk_kpi.out";
        private const string ConversionEfficiencyFileName = "conv_eff.out";
        private const string ReserveRatingsFileName = "resv_rtgs.out";

        private readonly ExternalScenarioStatus[] _triggeredInLandmarkStatuses =
        {
            ExternalScenarioStatus.Accepted,
            ExternalScenarioStatus.Scheduled,
            ExternalScenarioStatus.Running
        };

        private readonly ILandmarkApiClient _landmarkApi;
        private readonly IRunRepository _runRepository;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly ILandmarkAutoBookPayloadProvider _landmarkAutoBookPayloadProvider;
        private readonly IHubNotification<LandmarkRunStatusNotification> _runStatusChangedNotifier;
        private readonly IHubNotification<InfoMessageNotification> _infoMessageNotification;
        private readonly ILandmarkHttpPoliciesHolder _policiesHolder;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IClock _clock;
        private readonly IKPICalculationScopeFactory _kpiCalculationScopeFactory;
        private readonly IMapper _mapper;

        public LandmarkRunService(
            ILandmarkApiClient landmarkApi,
            IRunRepository runRepository,
            IScenarioResultRepository scenarioResultRepository,
            ILandmarkAutoBookPayloadProvider landmarkAutoBookPayloadProvider,
            IHubNotification<LandmarkRunStatusNotification> runStatusChangedNotifier,
            IHubNotification<InfoMessageNotification> infoMessageNotification,
            ILandmarkHttpPoliciesHolder policiesHolder,
            IAuditEventRepository auditEventRepository,
            IClock clock,
            IKPICalculationScopeFactory kpiCalculationScopeFactory,
            IMapper mapper)
        {
            _landmarkApi = landmarkApi;
            _runRepository = runRepository;
            _scenarioResultRepository = scenarioResultRepository;
            _landmarkAutoBookPayloadProvider = landmarkAutoBookPayloadProvider;
            _runStatusChangedNotifier = runStatusChangedNotifier;
            _infoMessageNotification = infoMessageNotification;
            _policiesHolder = policiesHolder;
            _auditEventRepository = auditEventRepository;
            _clock = clock;
            _kpiCalculationScopeFactory = kpiCalculationScopeFactory;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<LandmarkTriggerRunResult> TriggerRunAsync(LandmarkRunTriggerModel command, ScheduledRunSettingsModel scheduledRunSettings = null)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            RaiseInfo($"Starting Landmark run trigger process for ScenarioId: {command.ScenarioId}");

            var run = command.ScenarioId != Guid.Empty ? _runRepository.FindByScenarioId(command.ScenarioId) : default;
            if (run is null)
            {
                throw new InvalidOperationException($"Run for scenario {command.ScenarioId} was not found");
            }

            var scenario = run.Scenarios.SingleOrDefault(s => s.Id == command.ScenarioId);
            if (scenario is null)
            {
                throw new InvalidOperationException($"Scenario {command.ScenarioId} was not found");
            }

            try
            {
                var request = new LandmarkBookingRequest
                {
                    InputFiles = _landmarkAutoBookPayloadProvider.GetFiles(run.Id, command.ScenarioId).ToList()
                };

                var autoBookTriggerResult = await _landmarkApi.TriggerRunAsync(request, scheduledRunSettings).ConfigureAwait(false);

                scenario.ExternalRunInfo = new ExternalRunInfo
                {
                    ExternalRunId = autoBookTriggerResult.ProcessingId,
                    ExternalStatus = scheduledRunSettings is null
                        ? ExternalScenarioStatus.Accepted
                        : ExternalScenarioStatus.Scheduled,
                    ExternalStatusModifiedDate = _clock.GetCurrentInstant().ToDateTimeUtc(),

                    QueueName = scheduledRunSettings?.QueueName,
                    Priority = scheduledRunSettings?.Priority,
                    ScheduledDateTime = scheduledRunSettings?.DateTime,
                    Comment = scheduledRunSettings?.Comment,

                    CreatorId = scheduledRunSettings?.CreatorId,
                    CreatedDateTime = _clock.GetCurrentInstant().ToDateTimeUtc()
                };

                _runRepository.Update(run);
                _runRepository.SaveChanges();

                RaiseInfo($"Landmark run triggered for Run: {run.Id} and Scenario: {command.ScenarioId}. Processing id: {autoBookTriggerResult.ProcessingId}");

                return new LandmarkTriggerRunResult
                {
                    RunId = run.Id,
                    ExternalRunId = autoBookTriggerResult.ProcessingId,
                    Status = scheduledRunSettings is null
                        ? ExternalScenarioStatus.Accepted
                        : ExternalScenarioStatus.Scheduled
                };
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Landmark run triggering error, Run: {run.Id}, Scenario: {command.ScenarioId}", e));
                throw;
            }
        }

        public async Task<CancelRunResult> CancelRunAsync(Guid scenarioId)
        {
            var run = _runRepository.GetByScenarioId(scenarioId)
                .FirstOrDefault();

            var scenario = run?.Scenarios.SingleOrDefault(s =>
                s.ScenarioId == scenarioId && s.ExternalRunInfo != null &&
                s.ExternalRunInfo.ExternalStatus != ExternalScenarioStatus.Cancelled &&
                s.ExternalRunInfo.ExternalStatus != ExternalScenarioStatus.Completed &&
                s.ExternalRunInfo.ExternalStatus != ExternalScenarioStatus.Running);

            if (scenario is null)
            {
                throw new InvalidOperationException($"This scenario can't be cancelled on Landmark: {scenarioId}");
            }

            var externalRunInfo = scenario.ExternalRunInfo;

            var isCancelled = await _landmarkApi.CancelRunAsync(externalRunInfo.ExternalRunId).ConfigureAwait(false);

            if (isCancelled)
            {
                externalRunInfo.ExternalStatusModifiedDate = DateTime.UtcNow;
                externalRunInfo.ExternalStatus = ExternalScenarioStatus.Cancelled;

                _runRepository.Update(run);
                _runRepository.SaveChanges();

                var runNotification = new LandmarkRunStatusNotification
                {
                    runId = run.Id,
                    externalRunId = externalRunInfo.ExternalRunId,
                    status = nameof(ExternalScenarioStatus.Cancelled),
                    date = DateTime.UtcNow,
                    time = DateTime.UtcNow.TimeOfDay
                };

                _runStatusChangedNotifier.Notify(runNotification);
            }

            return new CancelRunResult
            {
                ScenarioId = scenario.Id,
                IsCancelled = isCancelled,
                ExternalRunId = externalRunInfo.ExternalRunId
            };
        }

        /// <inheritdoc />
        public async Task UpdateRunStatusesAsync()
        {
            var runs = _runRepository.FindTriggeredInLandmark();
            if (runs is null || !runs.Any())
            {
                return;
            }

            var jobs = await _landmarkApi.GetAllJobsAsync().ConfigureAwait(false);

            foreach (var run in runs)
            {
                var scenario = run.Scenarios.SingleOrDefault(s => s.ExternalRunInfo != null && _triggeredInLandmarkStatuses.Contains(s.ExternalRunInfo.ExternalStatus));
                if (scenario is null)
                {
                    throw new InvalidOperationException($"There are no triggered scenarios, Run: {run.Id}");
                }

                var externalRunInfo = scenario.ExternalRunInfo;

                RaiseInfo($"Getting status of Landmark run, ProcessingId: {externalRunInfo.ExternalRunId}");

                try
                {
                    var autoBookGetRunStatusResult = await _landmarkApi.GetRunStatusAsync(externalRunInfo.ExternalRunId)
                        .ConfigureAwait(false);

                    RaiseInfo($"Landmark run status {autoBookGetRunStatusResult.JobStatus}");

                    if (externalRunInfo.ExternalStatus == autoBookGetRunStatusResult.JobStatus)
                    {
                        continue;
                    }

                    if (externalRunInfo.ExternalStatus == ExternalScenarioStatus.Running && autoBookGetRunStatusResult.JobStatus == ExternalScenarioStatus.Accepted)
                    {
                        continue;
                    }

                    try
                    {
                        if (autoBookGetRunStatusResult.JobStatus == ExternalScenarioStatus.Completed)
                        {
                            ProcessResults(run, scenario, autoBookGetRunStatusResult);
                        }

                        var newStatus = autoBookGetRunStatusResult.JobStatus;

                        if (externalRunInfo.ExternalStatus == ExternalScenarioStatus.Scheduled && autoBookGetRunStatusResult.JobStatus == ExternalScenarioStatus.Accepted)
                        {
                            var jobInfo = jobs.FirstOrDefault(x => x.ProcessingId == externalRunInfo.ExternalRunId);

                            RaiseInfo($"Job info ({jobInfo.ProcessingId}) : {jobInfo.Status}");

                            if (jobInfo?.Status == "R")
                            {
                                newStatus = ExternalScenarioStatus.Running;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        RaiseInfo($"Update status ({externalRunInfo.ExternalRunId}): {externalRunInfo.ExternalStatus} : {newStatus}");

                        externalRunInfo.ExternalStatus = newStatus;
                    }
                    catch (Exception e)
                    {
                        var auditEvent = AuditEventFactory.CreateAuditEventForException(
                                0,
                                0,
                                $"Error occurred while processing results: {externalRunInfo.ExternalRunId}",
                                e
                            );

                        _auditEventRepository.Insert(auditEvent);

                        externalRunInfo.ExternalStatus = ExternalScenarioStatus.InvalidResponse;
                    }

                    externalRunInfo.ExternalStatusModifiedDate = DateTime.UtcNow;

                    _runRepository.Update(run);
                    _runRepository.SaveChanges();

                    var runNotification = new LandmarkRunStatusNotification
                    {
                        runId = run.Id,
                        externalRunId = externalRunInfo.ExternalRunId,
                        status = externalRunInfo.ExternalStatus.ToString(),
                        date = DateTime.UtcNow,
                        time = DateTime.UtcNow.TimeOfDay
                    };

                    _runStatusChangedNotifier.Notify(runNotification);
                }
                catch (Exception e)
                {
                    var auditEvent = AuditEventFactory.CreateAuditEventForException(
                            0,
                            0,
                            $"Landmark getting status error, ProcessingId: {externalRunInfo.ExternalRunId}",
                            e
                        );

                    _auditEventRepository.Insert(auditEvent);

                    throw;
                }
            }
        }

        /// <inheritdoc />
        public async Task ProbeLandmarkAvailabilityAsync()
        {
            if (_policiesHolder.LandmarkIsNotAvailable && await _landmarkApi.ProbeLandmarkAsync().ConfigureAwait(false))
            {
                _policiesHolder.LandmarkIsNotAvailable = false;
                _infoMessageNotification.Notify(new InfoMessageNotification
                {
                    message = "Landmark Sales connection restored"
                });
            }
        }

        private void ProcessResults(Run run, RunScenario scenario, LandmarkJobStatusModel jobStatus)
        {
            var scenarioResult = _scenarioResultRepository.Find(scenario.Id) ?? new ScenarioResult
            {
                Id = scenario.Id,
                TimeCompleted = DateTime.UtcNow,
                LandmarkMetrics = new List<KPI>()
            };

            using (var kpiCalculationScope = _kpiCalculationScopeFactory.CreateCalculationScope(run.Id, scenario.Id))
            {
                var kpiCalculationContext = kpiCalculationScope.Resolve<IKPICalculationContext>();

                kpiCalculationContext.SetDefaultKpiDemos();

                ProcessKpiOutputFile(jobStatus.OutputFiles, scenarioResult);
                ProcessLmkKpiOutputFile(jobStatus.OutputFiles, scenarioResult);
                ProcessConversionEfficienciesOutputFile(jobStatus.OutputFiles, scenarioResult, kpiCalculationScope, run.Id, scenario.Id);
                ProcessReserveRatingsOutputFile(jobStatus.OutputFiles, scenarioResult, kpiCalculationScope, run.Id, scenario.Id);
            }

            _scenarioResultRepository.Add(scenarioResult);
            _scenarioResultRepository.SaveChanges();
        }

        private void ProcessKpiOutputFile(IEnumerable<LandmarkOutputFileModel> outputFileModels,
            ScenarioResult scenarioResult)
        {
            var kpiFileModel = outputFileModels.SingleOrDefault(f => string.Equals(f.FileName, KpiFileName, StringComparison.OrdinalIgnoreCase));
            if (kpiFileModel == null)
            {
                return;
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.out");
            try
            {
                File.WriteAllText(filePath, kpiFileModel.Payload);

                var processor = new KPIOutputFileProcessor(KPISource.Landmark);

                processor.ProcessFile(filePath, scenarioResult);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private void ProcessLmkKpiOutputFile(List<LandmarkOutputFileModel> outputFileModels, ScenarioResult scenarioResult)
        {
            var kpiFileModel = outputFileModels.SingleOrDefault(f => string.Equals(f.FileName, LmkKpiFileName, StringComparison.OrdinalIgnoreCase));
            if (kpiFileModel == null)
            {
                return;
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.out");
            try
            {
                File.WriteAllText(filePath, kpiFileModel.Payload);

                var processor = new LmkKpiOutputFileProcessor(KPISource.Landmark);

                processor.ProcessFile(filePath, scenarioResult);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private void ProcessConversionEfficienciesOutputFile(IEnumerable<LandmarkOutputFileModel> outputFileModels,
            ScenarioResult scenarioResult, IKPICalculationScope kpiCalculationScope, Guid runId, Guid scenarioId)
        {
            var kpiFileModel = outputFileModels.SingleOrDefault(f => string.Equals(f.FileName, ConversionEfficiencyFileName, StringComparison.OrdinalIgnoreCase));
            if (kpiFileModel is null)
            {
                return;
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.out");
            var calculationContext = kpiCalculationScope.Resolve<IKPICalculationContext>();
            var calculationManager = kpiCalculationScope.Resolve<IKPICalculationManager>();

            try
            {
                File.WriteAllText(filePath, kpiFileModel.Payload);

                var processor = new ConversionEfficiencyOutputFileProcessor(_mapper, null);
                var results = processor.Process(filePath);

                calculationContext.ConversionEfficiencies = results;

                var kpiResult = calculationManager.SetAudit(null)
                    .CalculateKPIs(new HashSet<string>
                    {
                        ScenarioKPINames.ConversionEfficiencyByDemo
                    }, runId, scenarioId);

                kpiResult.ForEach(x => x.ResultSource = KPISource.Landmark);

                scenarioResult.LandmarkMetrics.AddRange(kpiResult);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private void ProcessReserveRatingsOutputFile(IEnumerable<LandmarkOutputFileModel> outputFileModels,
            ScenarioResult scenarioResult, IKPICalculationScope kpiCalculationScope, Guid runId, Guid scenarioId)
        {
            var kpiFileModel = outputFileModels.SingleOrDefault(f => string.Equals(f.FileName, ReserveRatingsFileName, StringComparison.OrdinalIgnoreCase));
            if (kpiFileModel is null)
            {
                return;
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.out");
            var calculationContext = kpiCalculationScope.Resolve<IKPICalculationContext>();
            var calculationManager = kpiCalculationScope.Resolve<IKPICalculationManager>();

            try
            {
                File.WriteAllText(filePath, kpiFileModel.Payload);

                var processor = new ReserveRatingsOutputFileProcessor(_mapper, null);

                var results = processor.Process(filePath);

                calculationContext.ReserveRatings = results;

                var kpiResult = calculationManager.SetAudit(null)
                    .CalculateKPIs(new HashSet<string>
                    {
                        ScenarioKPINames.AvailableRatingsByDemo,
                        ScenarioKPINames.ReservedRatingsByDemo
                    }, runId, scenarioId);

                kpiResult.ForEach(x => x.ResultSource = KPISource.Landmark);

                scenarioResult.LandmarkMetrics.AddRange(kpiResult);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        private void RaiseInfo(string message) => _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, message));
    }
}
