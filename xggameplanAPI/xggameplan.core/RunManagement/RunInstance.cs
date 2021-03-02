using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.core.BRS;
using xggameplan.core.Helpers;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.core.RunManagement;
using xggameplan.Model;

namespace xggameplan.RunManagement
{
    /// <summary>
    /// Run instance for a scenario.
    /// </summary>
    public class RunInstance
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;

        private readonly IAutoBookInputHandler _autoBookInputHandler;
        private readonly IAutoBookOutputHandler _autoBookOutputHandler;
        private readonly RunCompletionNotifier _runCompletionNotifier;
        private readonly ScenarioSnapshotGenerator _scenarioSnapshotGenerator;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;
        private readonly IBRSIndicatorManager _brsIndicatorManager;
        private readonly ILandmarkRunService _landmarkRunService;

        private readonly IAutoBooks _autoBooks;

        private readonly IConfiguration _configuration;

        public RunInstance(
            Guid runId,
            Guid scenarioId,
            IRepositoryFactory repositoryFactory,
            IAuditEventRepository auditEventRepository,
            IAutoBookInputHandler autoBookInputHandler, IAutoBookOutputHandler autoBookOutputHandler,
            RunCompletionNotifier runCompletionNotifier,
            ScenarioSnapshotGenerator scenarioSnapshotGenerator,
            ISynchronizationService synchronizationService,
            IPipelineAuditEventRepository pipelineAuditEventRepository,
            IBRSIndicatorManager brsIndicatorManager,
            ILandmarkRunService landmarkRunService,
            IAutoBooks autoBooks,
            IConfiguration configuration)
        {
            RunId = runId;
            ScenarioId = scenarioId;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
            _autoBookInputHandler = autoBookInputHandler;
            _autoBookOutputHandler = autoBookOutputHandler;
            _runCompletionNotifier = runCompletionNotifier;
            _scenarioSnapshotGenerator = scenarioSnapshotGenerator;
            _synchronizationService = synchronizationService;
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
            _brsIndicatorManager = brsIndicatorManager;
            _landmarkRunService = landmarkRunService;
            _autoBooks = autoBooks;
            _configuration = configuration;
        }

        public Guid RunId { get; }

        public Guid ScenarioId { get; }

        /// <summary>
        /// Starts run. Uploads input data and then instructs AutoBook instance to start
        /// </summary>
        /// <param name="autoBookInterface"></param>
        /// <param name="autoBook"></param>
        public void UploadInputFilesStartAutoBookRun(IAutoBook autoBookInterface, AutoBook autoBook)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var runRepository = scope.CreateRepository<IRunRepository>();
            var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();

            bool startedRun = false;
            try
            {
                // Update scenario status to Starting, Scenario.StartedDateTime has already been set
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId,
                    new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.Starting });

                // Get run
                var run = runRepository.Find(RunId);

                // Record which run/scenario is being processed
                //IAutoBookRepository autoBookRepository = (IAutoBookRepository)repositories[typeof(IAutoBookRepository)];
                //AutoBook localAutoBook = autoBookRepository.Find(autoBook.Id);
                lock (autoBook)
                {
                    autoBook.Status = AutoBookStatuses.In_Progress;
                    autoBook.LastRunStarted = DateTime.UtcNow;
                    autoBook.Locked = true;
                    autoBook.Task = new AutoBookTask()
                    {
                        RunId = RunId,
                        ScenarioId = ScenarioId
                    };
                }

                autoBookRepository.Update(autoBook);
                autoBookRepository.SaveChanges(); // Force save

                // Upload input data
                _autoBookInputHandler.Handle(run, ScenarioId);

                // Instruct AutoBook to start processing
                bool loggedNotifyFinished = false;
                try
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0,
                        PipelineEventIDs.STARTED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, autoBook.Id, null));

                    GetAutoBookStatusModel autoBookStatusModel = autoBookInterface.StartAutoBookRun(RunId, ScenarioId);

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                        PipelineEventIDs.STARTED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null));

                    if (autoBookStatusModel.Status == AutoBookStatuses.In_Progress) // Task_Error or Fatal_Error
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, autoBook.Id,
                            null, null, null));

                        _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null));

                        loggedNotifyFinished = true;
                    }
                    else
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, autoBook.Id,
                            null,
                            String.Format(
                                "AutoBook API unexpectedly returned status {0} when instructing it to start run",
                                autoBookStatusModel.ToString()), null));

                        _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId,
                            $"AutoBook API unexpectedly returned status {autoBookStatusModel.ToString()} " +
                            "when instructing it to start run"));

                        loggedNotifyFinished = true;
                        throw new Exception(String.Format(
                            "AutoBook returned status {0} when starting run (AutoBookID={1})",
                            autoBookStatusModel.Status, autoBook.Id));
                    }
                }
                catch (System.Exception exception)
                {
                    if (!loggedNotifyFinished)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, autoBook.Id,
                            null, exception.Message, exception));

                        _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, exception.Message));
                    }

                    throw;
                }
                finally
                {
                    _pipelineAuditEventRepository.SaveChanges();
                }

                // Flag as InProgress
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId,
                    new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.InProgress });
                startedRun = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Clean up failure
                if (!startedRun)
                {
                    // Reset to free, unlocks AutoBook instance so that it can be re-used
                    autoBookInterface.ResetFree();
                    autoBookRepository.SaveChanges();
                    _pipelineAuditEventRepository.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Uploads input data and then send API post for AutoBookRequest
        /// </summary>
        /// <param name="autoBookInterface"></param>
        /// <param name="autoBook"></param>
        public void UploadInputFilesAndCreateAutoBookRequest(IReadOnlyCollection<AutoBookInstanceConfiguration> autoBookInstanceConfigurations, double storageGB)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"AutoDistributed - {nameof(UploadInputFilesAndCreateAutoBookRequest)}"));

            using var scope = _repositoryFactory.BeginRepositoryScope();
            var runRepository = scope.CreateRepository<IRunRepository>();

            bool startedRun = false;
            try
            {
                // Update scenario status to Starting, Scenario.StartedDateTime has already been set
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId,
                    new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.Starting });

                var run = runRepository.Find(RunId); // Get run

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"AutoDistributed - about to enter input handler"));

                _autoBookInputHandler.Handle(run, ScenarioId); // Upload input data

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"AutoDistributed - uploaded files for run: {RunId.ToString()} scenario: {ScenarioId.ToString()}"));

                // Create AutoBookRequest
                bool loggedNotifyFinished = false;
                try
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineStart(0, 0,
                        PipelineEventIDs.STARTED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null, null));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                            PipelineEventIDs.STARTED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null));

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"AutoDistributed - Create AutBookRequest for run: {run.Id} scenario: {ScenarioId}"));

                    var result = CreateAutoBookRequest(autoBookInstanceConfigurations);

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"AutoDistributed - Created AutBookRequest for run: {run.Id} scenario: {ScenarioId}, result: {result}"));

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                        PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null, null, null, null));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                        PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null));

                    loggedNotifyFinished = true;
                }
                catch (System.Exception exception)
                {
                    if (!loggedNotifyFinished)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForGameplanPipelineEnd(0, 0,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, null, null, exception.Message, exception));

                        _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                            PipelineEventIDs.FINISHED_NOTIFYING_AUTOBOOK_API, RunId, ScenarioId, exception.Message));
                    }

                    throw;
                }
                finally
                {
                    _pipelineAuditEventRepository.SaveChanges();
                }

                // Flag as InProgress
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId,
                    new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.InProgress });
                startedRun = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Clean up failure
                if (!startedRun)
                {
                    _pipelineAuditEventRepository.SaveChanges();
                }
            }
        }

        private string CreateAutoBookRequest(IReadOnlyCollection<AutoBookInstanceConfiguration> autoBookInstanceConfigurations)
        {
            //AutoDistributed - autoBookRequest needs to be sent with the new API autodistributed model
            // "Id": "imag-dev07-1",
            var respondTo = "http://" + _configuration["Environment:Id"] + "-api.xggamplan.com/autobooks/";

            AutoBookRequestModel autoBookRequest = new AutoBookRequestModel
            {
                respondTo = respondTo,
                runId = RunId.ToString(),
                scenarioId = ScenarioId.ToString(),
                instanceType = autoBookInstanceConfigurations.First().InstanceType,             //check
                storageSizeGB = autoBookInstanceConfigurations.First().StorageSizeGb,    //check
                version = _autoBooks.Settings.ApplicationVersion,
                binariesVersion = _autoBooks.Settings.BinariesVersion,
                maxInstances = _autoBooks.Settings.MaxInstances,
                mock = "false"     //set to true in dev stage, not needed in prod
                                   //Mock = false in prod, will actually call the nec. lambda:
                                   // otherwise respond with : will have invoked:
                                   // bash aws/request-invoke.sh http://imag-dev07-1-api.xggamplan.com/autobooks/   t3.medium 50 v4.4.0 xg-gameplan-autobook-binaries.2.40.012.120 3
            };
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"AutoDistributed - AutoBookRequest object: " +
                $"{autoBookRequest.respondTo}, {autoBookRequest.runId}, {autoBookRequest.scenarioId}, {autoBookRequest.instanceType}, " +
                $"{autoBookRequest.storageSizeGB}, {autoBookRequest.version}, {autoBookRequest.binariesVersion}, " +
                $"{autoBookRequest.maxInstances}, {autoBookRequest.mock}"));

            return _autoBooks.CreateAutoBookRequestRun(autoBookRequest);
        }

        /// <summary>
        /// Handles completed successfully, downloads and processes output data. If we can't process the output data (E.g. Cloud issues)
        /// then that is a failure too.
        /// </summary>
        public void HandleCompletedSuccess()
        {
            bool success = false;
            IRunRepository runRepository = null;
            try
            {
                try
                {
                    using var scope = _repositoryFactory.BeginRepositoryScope();
                    // Update status to GettingResults
                    RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId, new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.GettingResults });

                    // Get run details
                    runRepository = scope.CreateRepository<IRunRepository>();
                    var scenarioResultRepository = scope.CreateRepository<IScenarioResultRepository>();
                    var run = runRepository.Find(RunId);
                    var scenario = run.Scenarios.Find(currentScenario => currentScenario.Id == ScenarioId);

                    // Get results
                    _autoBookOutputHandler.Handle(run, ScenarioId);

                    success = true;

                    // Notification run scenario completed
                    try
                    {
                        _runCompletionNotifier.Notify(run, scenario, success);
                    }
                    catch (System.Exception exception)
                    {
                        // Log failure, don't throw
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating notification for run completed success (RunID={0}, ScenarioID={1})", RunId, ScenarioId), exception));
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    // Update status to CompletedSuccess/CompletedError
                    DateTime? completedDateTime = DateTime.UtcNow;
                    ScenarioStatuses scenarioStatus = success ? ScenarioStatuses.CompletedSuccess : ScenarioStatuses.CompletedError;
                    RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId, new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { scenarioStatus }, null, new List<DateTime?>() { completedDateTime });
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                // Load run so that we can check number of completed scenarios, use fresh repo instance in case run changed elsewhere
                runRepository = scope.CreateRepository<IRunRepository>();
                var run = runRepository.Find(RunId);

                // Generate notification if all scenarios completed, success if at least one scenario generates results
                if (run.Scenarios.Count == run.CompletedScenarios.Count)
                {
                    _synchronizationService.Release(run.Id);

                    int countSucccess = run.Scenarios.Where(currentScenario => currentScenario.Status == ScenarioStatuses.CompletedSuccess).ToList().Count;
                    try
                    {
                        _runCompletionNotifier.Notify(run, (countSucccess > 0));
                    }
                    catch (System.Exception exception)
                    {
                        // Log failure, don't throw
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating notification for run completed success (RunID={0})", RunId), exception));
                    }

                    try
                    {
                        // Calculate BRS Indicator

                        IEnumerable<ScenarioResult> calculatedScenarioResults = _brsIndicatorManager.CalculateBRSIndicatorsAfterRunCompleted(run);

                        SendToLandmark(run, calculatedScenarioResults);
                    }
                    catch (Exception exception)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error calculation brs indicator for run completed success (RunID={RunId})", exception));
                    }
                }
            }
        }

        /// <summary>
        /// Handles completed with error, no results
        /// </summary>
        public void HandleCompletedTaskError()
        {
            IRunRepository runRepository = null;
            try
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                // Flag scenario as CompletedError, no results available
                DateTime? completedDateTime = DateTime.UtcNow;
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId, new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.CompletedError }, null, new List<DateTime?>() { completedDateTime });

                // Get run details
                runRepository = scope.CreateRepository<IRunRepository>();
                var run = runRepository.Find(RunId);
                var scenario = run.Scenarios.Find(currentScenario => currentScenario.Id == ScenarioId);

                // Notification run scenario completed with error
                /* No notification of failure, Mulesoft expects us to only indicate successfull runs
                try
                {
                    _runManager.NotificationRunCompleted(run, scenario, false);
                }
                catch (System.Exception exception)
                {
                    // Log failure, don't throw
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating notification for run completed failure (RunID={0}, ScenarioID={1})", _runId, _scenarioId), exception));
                }
                */
            }
            catch
            {
                throw;
            }
            finally
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                // Load run so that we can check number of completed scenarios, use fresh repo instance in case run changed elsewhere
                runRepository = scope.CreateRepository<IRunRepository>();
                var run = runRepository.Find(RunId);

                // Generate notification if all scenarios completed, success if at least one scenario generated results
                if (run.Scenarios.Count == run.CompletedScenarios.Count)
                {
                    _synchronizationService.Release(run.Id);

                    int countSucccess = run.Scenarios.Where(currentScenario => currentScenario.Status == ScenarioStatuses.CompletedSuccess).ToList().Count;
                    try
                    {
                        _runCompletionNotifier.Notify(run, (countSucccess > 0));
                    }
                    catch (System.Exception exception)
                    {
                        // Log failure, don't throw
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating notification for run completed failure (RunID={0})", run.Id), exception));
                    }
                }
            }
        }

        /// <summary>
        /// Handles completed with error, no results
        /// </summary>
        public void HandleCompletedFatalError(AutoBook autoBook)
        {
            IRunRepository runRepository = null;
            try
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                // Get scenario snapshot for diagnostics
                //_scenarioSnapshotGenerator.Generate(autoBook, _scenarioId);

                // Flag scenario as CompletedError, no results available
                DateTime? completedDateTime = DateTime.UtcNow;
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, RunId, new List<Guid>() { ScenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.CompletedError }, null, new List<DateTime?>() { completedDateTime });

                // Get run details
                runRepository = scope.CreateRepository<IRunRepository>();
                var run = runRepository.Find(RunId);
                var scenario = run.Scenarios.Find(currentScenario => currentScenario.Id == ScenarioId);

                // Notification run scenario completed with error
                try
                {
                    _runCompletionNotifier.Notify(run, scenario, false);
                }
                catch (System.Exception exception)
                {
                    // Log failure, don't throw
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating notification for run completed failure (RunID={0}, ScenarioID={1})", RunId, ScenarioId), exception));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                // Load run so that we can check number of completed scenarios, use fresh repo instance in case run changed elsewhere
                runRepository = scope.CreateRepository<IRunRepository>();
                var run = runRepository.Find(RunId);

                // Generate notification if all scenarios completed, success if at least one scenario generated results
                if (run.Scenarios.Count == run.CompletedScenarios.Count)
                {
                    _synchronizationService.Release(run.Id);

                    int countSucccess = run.Scenarios.Where(currentScenario => currentScenario.Status == ScenarioStatuses.CompletedSuccess).ToList().Count;
                    try
                    {
                        _runCompletionNotifier.Notify(run, (countSucccess > 0));
                    }
                    catch (System.Exception exception)
                    {
                        // Log failure, don't throw
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, string.Format("Error generating notification for run completed failure (RunID={0})", run.Id), exception));
                    }
                }
            }
        }

        /// <summary>
        /// Handles cleanup after run completed
        /// </summary>
        public void HandleCompletedCleanUp()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            // Load run
            var runRepository = scope.CreateRepository<IRunRepository>();
            var run = runRepository.Find(RunId);

            _autoBookInputHandler.CleanUp(run, ScenarioId);
            _autoBookOutputHandler.CleanUp(run, ScenarioId);
        }

        public void SendToLandmark(Run run, IEnumerable<ScenarioResult> calculatedScenarioResults)
        {
            if (run.ScheduleSettings != null)
            {
                var bestScenarioId = Guid.Empty;

                if (run.Scenarios.Count == 1)
                {
                    bestScenarioId = run.Scenarios.First().Id;
                }
                else if (calculatedScenarioResults != null)
                {
                    var maxScore = calculatedScenarioResults.Max(x => x.BRSIndicator);

                    var scenariosWithMaxScore = calculatedScenarioResults.Where(x => x.BRSIndicator == maxScore);

                    if (scenariosWithMaxScore.Count() == 1)
                    {
                        bestScenarioId = scenariosWithMaxScore.First().Id;
                    }
                    else
                    {
                        var maxWeightedAverageCompletionScore = scenariosWithMaxScore
                            .SelectMany(x => x.Metrics)
                            .Where(x => x.Name == ScenarioKPINames.WeightedAverageCompletion)
                            .Max(x => x.Value);

                        var bestScenarios = scenariosWithMaxScore
                            .Where(x => x.Metrics
                                .FirstOrDefault(x => x.Name == ScenarioKPINames.WeightedAverageCompletion)?
                                .Value == maxWeightedAverageCompletionScore);

                        bestScenarioId = bestScenarios
                            .Join(run.Scenarios, x => x.Id, y => y.Id, (x, y) => y)
                            .OrderBy(x => x.Order)
                            .First().Id;
                    }
                }

                if (bestScenarioId != Guid.Empty)
                {
                    var settings = run.ScheduleSettings;

                    _ = _landmarkRunService.TriggerRunAsync(new LandmarkRunTriggerModel
                    {
                        ScenarioId = bestScenarioId
                    },
                    new model.Internal.Landmark.ScheduledRunSettingsModel
                    {
                        QueueName = settings.QueueName,
                        DateTime = settings.DateTime,
                        Comment = settings.Comment,
                        Priority = settings.Priority
                    }).GetAwaiter().GetResult();
                }
            }
        }
    }
}
