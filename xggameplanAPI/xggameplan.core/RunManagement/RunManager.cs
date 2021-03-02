using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Audit;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.common.Services;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Helpers;
using xggameplan.core.Hubs;
using xggameplan.core.Interfaces;
using xggameplan.core.OutputProcessors.Processors;
using xggameplan.core.RunManagement;
using xggameplan.core.RunManagement.Notifications;
using xggameplan.core.Services;
using xggameplan.core.Tasks;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.Model;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository.Memory;
using xggameplan.RunManagement.Notifications;

using static xggameplan.common.Helpers.LogAsString;

namespace xggameplan.RunManagement
{
    /// <summary>
    /// <para>Provides management of AutoBook runs.</para>
    /// <para>
    /// Due to this being a multi-threaded environment, we need to prevent
    /// various issues:
    /// <list type="bullet">
    /// <item>
    /// Two app instances attempting to start run on same free AutoBook instance
    /// </item>
    /// <item>Two app instances attempting to start run for same scenario on</item>
    /// separate AutoBook instances.
    /// </list>
    /// </para>
    /// </summary>
    public class RunManager : IRunManager
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAutoBooks _autoBooks;
        private readonly IRunRepository _runRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ISpotRepository _spotRepository;
        private readonly IProductRepository _productRepository;
        private readonly IClashRepository _clashRepository;
        private readonly IUniverseRepository _universeRepository;
        private readonly IRatingsScheduleRepository _ratingsScheduleRepository;
        private readonly IISRSettingsRepository _isrSettingsRepository;
        private readonly IRSSettingsRepository _rsSettingsRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly IClearanceRepository _clearanceRepository;
        private readonly OptimiserInputFiles _optimiserInputFiles;
        private readonly INotificationCollection _notifications;
        private readonly ISmoothProcessor _smoothProcessor;
        private readonly IMapper _mapper;
        private readonly IHubNotification<RunNotification> _completedRunNotification;
        private readonly RunCompletionNotifier _runCompletionNotifier;
        private readonly RunInstanceCreator _runInstanceCreator;
        private readonly RunScenarioTaskExecutor _runScenarioTaskExecutor;
        private readonly IConfiguration _configuration;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IFeatureManager _featureManager;
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;
        private readonly IAutoBookInstanceConfigurationRepository _autoBookInstanceConfigurationRepository;
        private readonly IRecalculateBreakAvailabilityService _recalculateBreakAvailabilityService;
        private readonly IHubNotification<ScenarioNotificationModel> _scenarioNotification;
        private readonly IRunCleaner _runCleaner;
        private readonly ISystemLogicalDateService _systemLogicalDateService;
        private readonly IKPICalculationScopeFactory _kpiCalculationScopeFactory;

        public RunManager(
            IRepositoryFactory repositoryFactory,
            IAutoBooks autoBooks,
            ITenantSettingsRepository tenantSettingsRepository,
            IOutputFileRepository outputFileRepository,
            IAuditEventRepository auditEventRepository,
            ICampaignRepository campaignRepository,
            ISalesAreaRepository salesAreaRepository,
            ISpotRepository spotRepository,
            IProductRepository productRepository,
            IClashRepository clashRepository,
            IUniverseRepository universeRepository,
            IRatingsScheduleRepository ratingsScheduleRepository,
            IISRSettingsRepository isrSettingsRepository,
            IRSSettingsRepository rsSettingsRepository,
            IDemographicRepository demographicRepository,
            IClearanceRepository clearanceRepository,
            OptimiserInputFiles serviceBase,
            INotificationCollection notifications,
            ISmoothProcessor smoothProcessor,
            IMapper mapper,
            IHubNotification<RunNotification> completedRunNotification,
            IRunRepository runRepository,
            RunCompletionNotifier runCompletionNotifier,
            RunInstanceCreator runInstanceCreator,
            RunScenarioTaskExecutor runScenarioTaskExecutor,
            IConfiguration configuration,
            ISynchronizationService synchronizationService,
            IFeatureManager featureManager,
            IPipelineAuditEventRepository pipelineAuditEventRepository,
            IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository,
            IRecalculateBreakAvailabilityService recalculateBreakAvailabilityService,
            IHubNotification<ScenarioNotificationModel> scenarioNotification,
            IRunCleaner runCleaner,
            ISystemLogicalDateService systemLogicalDateService,
            IKPICalculationScopeFactory kpiCalculationScopeFactory)
        {
            _repositoryFactory = repositoryFactory;
            _autoBooks = autoBooks;
            _tenantSettingsRepository = tenantSettingsRepository;
            _outputFileRepository = outputFileRepository;
            _auditEventRepository = auditEventRepository;
            _campaignRepository = campaignRepository;
            _salesAreaRepository = salesAreaRepository;
            _spotRepository = spotRepository;
            _productRepository = productRepository;
            _clashRepository = clashRepository;
            _universeRepository = universeRepository;
            _ratingsScheduleRepository = ratingsScheduleRepository;
            _isrSettingsRepository = isrSettingsRepository;
            _rsSettingsRepository = rsSettingsRepository;
            _demographicRepository = demographicRepository;
            _clashRepository = clashRepository;
            _clearanceRepository = clearanceRepository;
            _optimiserInputFiles = serviceBase;
            _notifications = notifications;
            _smoothProcessor = smoothProcessor;
            _mapper = mapper;
            _runRepository = runRepository;
            _completedRunNotification = completedRunNotification;
            _runCompletionNotifier = runCompletionNotifier;
            _runInstanceCreator = runInstanceCreator;
            _runScenarioTaskExecutor = runScenarioTaskExecutor;
            _configuration = configuration;
            _synchronizationService = synchronizationService;
            _featureManager = featureManager;
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
            _autoBookInstanceConfigurationRepository = autoBookInstanceConfigurationRepository;
            _recalculateBreakAvailabilityService = recalculateBreakAvailabilityService;
            _scenarioNotification = scenarioNotification;
            _runCleaner = runCleaner;
            _systemLogicalDateService = systemLogicalDateService;
            _kpiCalculationScopeFactory = kpiCalculationScopeFactory;
        }

        private void RaiseInfo(string message) =>
                _auditEventRepository.Insert(
                    AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        $"{LogEntryDiscriminator}{message}"
                        )
                    );

        private void RaiseWarning(string message) =>
            _auditEventRepository.Insert(
                AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                    $"{LogEntryDiscriminator}{message}"
                    )
                );

        private void RaiseException(string message, Exception exception) =>
            _auditEventRepository.Insert(
                AuditEventFactory.CreateAuditEventForException(0, 0,
                    $"{LogEntryDiscriminator}{message}",
                    exception
                    )
                );

        /// <summary>
        /// <para>
        /// Resets the scenario to Scheduled. If scenario already completed (or
        /// cancelled) then do nothing.
        /// </para>
        /// <para>TODO: Consider if there might be an issue if scenario assigned to another AutoBook</para>
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        private void ResetScenarioToScheduled(Guid runId, Guid scenarioId)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var runRepository = scope.CreateRepository<IRunRepository>();
            Run run = runRepository.Find(runId);
            RunScenario scenario = run.Scenarios.FirstOrDefault(x => x.Id == scenarioId);

            if (scenario.IsScheduledOrRunning)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"Resetting scenario to Scheduled due to AutoBook error (RunID={runId.ToString()}, ScenarioID={scenarioId.ToString()})"));
                UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, runId, new List<Guid>() { scenarioId }, new List<ScenarioStatuses>() { ScenarioStatuses.Scheduled });
            }
        }

        /// <summary>
        /// Webhook to an external system to ask for the Inventory to be Locked
        /// for this Run
        /// </summary>
        /// <param name="run"></param>
        public bool InventoryLock(Run run)
        {
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            if (tenantSettings?.WebhookSettings != null)
            {
                WebhookSettings inventoryLockSettings = tenantSettings.WebhookSettings
                    .FirstOrDefault(il => il.EventType == WebhookEvents.InventoryLock);

                if (inventoryLockSettings != null)
                {
                    try
                    {
                        // Check HTTP notification
                        INotification<HTTPNotificationSettings> httpNotification = _notifications?.GetNotification<HTTPNotificationSettings>();
                        if (inventoryLockSettings.HTTP != null && httpNotification != null && inventoryLockSettings.HTTP.Enabled)
                        {
                            httpNotification.InventoryLock(run, inventoryLockSettings.HTTP);
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Inventory lock requested (RunID={run.Id.ToString()}))"));
                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error Attempting to Lock Inventory", exception);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Webhook to an external system to ask for the Inventory to be
        /// Unlocked/Released for this Run Optionally there is the ability for a
        /// user to "Choose a particular Scenario" when releasing the lock This
        /// will be used by the Inventory System to trigger the collection of
        /// output for that Scenario
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        public bool InventoryUnlock(Run run, Guid? scenarioId)
        {
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            if (tenantSettings != null && tenantSettings.WebhookSettings != null)
            {
                WebhookSettings inventoryLockSettings = tenantSettings.WebhookSettings
                    .FirstOrDefault(il => il.EventType == WebhookEvents.InventoryUnlock);

                if (inventoryLockSettings != null)
                {
                    try
                    {
                        // Check HTTP notification
                        INotification<HTTPNotificationSettings> httpNotification = _notifications?.GetNotification<HTTPNotificationSettings>();
                        if (inventoryLockSettings.HTTP != null && httpNotification != null && inventoryLockSettings.HTTP.Enabled)
                        {
                            httpNotification.InventoryUnlock(run, scenarioId, inventoryLockSettings.HTTP);
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Inventory unlock requested (RunID={run.Id.ToString()}, ScenarioId={scenarioId.ToString()})"));
                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error Attempting to Unlock Inventory", exception);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Resets data for the run, schedule data (so that repeating a run
        /// leaves the system in the same state at the end) and run output data
        /// (so that we don't crash or duplicate data if a run is reset re-executed).
        /// </summary>
        /// <param name="run"></param>
        private void ResetDataForStartRun(Run run)
        {
            RaiseInfo($"Resetting previous run output data if it exists (RunID={Log(run.Id)})");

            var runReset = new RunReset(_repositoryFactory);
            runReset.ResetSmoothFailures(run.Id);

            IReadOnlyList<string> processesToReset = new List<string>() { "autobook", "isr", "rzr" };

            run.Scenarios.ForEach(runScenario =>
                runReset.ResetScenarioOutputData(runScenario.Id, processesToReset, true, true, true)
            );

            RaiseInfo($"Reset previous run output data if exists (RunID={Log(run.Id)})");
        }

        private void RecalculateBreakAvailability(Run run, IReadOnlyList<SalesArea> salesAreas)
        {
            _recalculateBreakAvailabilityService.Execute(
                new DateTimeRange(
                    DateHelper.CreateStartDateTime(run.StartDate, run.StartTime),
                    DateHelper.CreateEndDateTime(run.EndDate, run.EndTime)),
                salesAreas);
        }

        /// <summary>
        /// Executes Smooth
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        private void ExecuteSmooth(Run run, IReadOnlyList<SalesArea> salesAreas)
        {
            RaiseInfo($"Executing Smooth{Ellipsis}");

            try
            {
                run.Scenarios.ForEach(
                    scenario =>
                    {
                        _auditEventRepository.Insert(
                            AuditEventFactory.CreateAuditEventForSmoothPipelineStart(0, 0,
                                PipelineEventIDs.STARTED_SMOOTHING_INPUT_FILES,
                                run.Id,
                                scenario.Id,
                                null,
                                null
                                )
                            );

                        _pipelineAuditEventRepository.Add(
                            PipelineEventHelper.CreatePipelineAuditEvent(
                                AuditEventTypes.GamePlanRun,
                                PipelineEventIDs.STARTED_SMOOTHING_INPUT_FILES,
                                run.Id,
                                scenario.Id,
                                null));
                    });

                _smoothProcessor.Execute(run, salesAreas, RaiseInfo, RaiseWarning, RaiseException);

                run.Scenarios.ForEach(
                    scenario =>
                    {
                        _auditEventRepository.Insert(
                            AuditEventFactory.CreateAuditEventForSmoothPipelineEnd(0, 0,
                                PipelineEventIDs.FINISHED_SMOOTHING_INPUT_FILES,
                                run.Id,
                                scenario.Id,
                                null,
                                null,
                                null,
                                null
                                )
                            );

                        _pipelineAuditEventRepository.Add(
                            PipelineEventHelper.CreatePipelineAuditEvent(
                                AuditEventTypes.GamePlanRun,
                                PipelineEventIDs.FINISHED_SMOOTHING_INPUT_FILES,
                                run.Id,
                                scenario.Id,
                                null));
                    });

                RecalculateBreakAvailabilityAfterSmoothFinishes(run, salesAreas);
            }
            catch (Exception exception)
            {
                run.Scenarios.ForEach(
                    scenario =>
                    {
                        _auditEventRepository.Insert(
                                                AuditEventFactory.CreateAuditEventForSmoothPipelineEnd(0, 0,
                                                    PipelineEventIDs.FINISHED_SMOOTHING_INPUT_FILES,
                                                    run.Id,
                                                    scenario.Id,
                                                    null,
                                                    $"Error executing Smooth (RunID={Log(run.Id)})",
                                                    exception.Message,
                                                    exception
                                                    )
                                                );
                        _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                           PipelineEventIDs.FINISHED_SMOOTHING_INPUT_FILES, run.Id, scenario.Id, exception.Message));
                    });
            }
            finally
            {
                RaiseInfo("Executed Smooth.");
                _pipelineAuditEventRepository.SaveChanges();
            }
        }

        private void RecalculateBreakAvailabilityBeforeRunStarts(Run run, IReadOnlyList<SalesArea> salesAreas)
        {
            RaiseInfo($"Recalculating break availability before run starts{Ellipsis}");

            Guid runId = run.Id;

            run.Scenarios.ForEach(
                scenario =>
                {
                    _auditEventRepository.Insert(
                                        AuditEventFactory.CreateAuditEventForPreRunRecalculateBreakAvailabilityStart(0, 0,
                                            runId, scenario.Id));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                PipelineEventIDs.STARTED_PRE_RUN_RECALCULATING_BREAK_AVAILABILITY, runId, scenario.Id, null));
                });

            RecalculateBreakAvailability(run, salesAreas);

            run.Scenarios.ForEach(
                scenario =>
                {
                    _auditEventRepository.Insert(
                                        AuditEventFactory.CreateAuditEventForPreRunRecalculateBreakAvailabilityEnd(0, 0,
                                            runId, scenario.Id));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                PipelineEventIDs.FINISHED_PRE_RUN_RECALCULATING_BREAK_AVAILABILITY, runId, scenario.Id, null));
                });

            RaiseInfo("Recalculated break availability before run starts.");
        }

        private void RecalculateBreakAvailabilityAfterSmoothFinishes(Run run, IReadOnlyList<SalesArea> salesAreas)
        {
            RaiseInfo($"Recalculating break availability after Smooth{Ellipsis}");

            Guid runId = run.Id;

            run.Scenarios.ForEach(
                scenario =>
                {
                    _auditEventRepository.Insert(
                                        AuditEventFactory.CreateAuditEventForPostSmoothRecalculateBreakAvailabilityStart(0, 0,
                                            runId, scenario.Id));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                                PipelineEventIDs.STARTED_POST_SMOOTH_RECALCULATING_BREAK_AVAILABILITY, runId, scenario.Id, null));
                });

            RecalculateBreakAvailability(run, salesAreas);

            run.Scenarios.ForEach(
                scenario =>
                {
                    _auditEventRepository.Insert(
                                        AuditEventFactory.CreateAuditEventForPostSmoothRecalculateBreakAvailabilityEnd(0, 0,
                                            runId, scenario.Id));

                    _pipelineAuditEventRepository.Add(PipelineEventHelper.CreatePipelineAuditEvent(AuditEventTypes.GamePlanRun,
                               PipelineEventIDs.FINISHED_POST_SMOOTH_RECALCULATING_BREAK_AVAILABILITY, runId, scenario.Id, null));
                });

            RaiseInfo("Recalculated break availability after Smooth.");
        }

        /// <summary>
        /// <para>
        /// Starts run of all scenarios. If free AutoBook then scenario is started on it. If no free AutoBook then new creation of new instance
        /// is started, will be notified of creation complete by PUT later, will then start scenario.
        /// </para>
        /// <para>
        /// StartRun is a long running procedure and we need to be careful because it is multi-threaded and we shouldn't store a Run instance
        /// for long periods of time because it might become out of date. For this reason then, when we need to update the scenario status, then
        /// we get a fresh repository, load the run and then save the changes.
        /// </para>
        /// </summary>
        /// <param name="runId"></param>
        /// <returns>List of RunInstances for all started scenarios</returns>
        public List<RunInstance> AllScenariosStartRun(Guid runId)
        {
            var scenarioSyncStatuses = new ConcurrentDictionary<Guid, bool>();

            try
            {
                RaiseInfo($"Begin {nameof(AllScenariosStartRun)}(Guid {runId.ToString()}) AutoBooks.Instances: {_autoBooks.Instances.ToString()}");

                using var scope = _repositoryFactory.BeginRepositoryScope();
                var runInstances = new ConcurrentBag<RunInstance>();
                var scenarioTasks = new List<Task>();

                var repositories = scope.CreateRepositories(
                    typeof(IRunRepository),
                    typeof(IAutoBookInstanceConfigurationRepository),
                    typeof(ICampaignRepository),
                    typeof(IDemographicRepository)
                );

                var runRepository = repositories.Get<IRunRepository>();
                var autoBookInstanceConfigurationRepository = repositories.Get<IAutoBookInstanceConfigurationRepository>();
                var campaignRepository = repositories.Get<ICampaignRepository>();
                var demographicRepository = repositories.Get<IDemographicRepository>();

                var run = runRepository.Find(runId);
                DateTime? startedDateTime = DateTime.UtcNow;

                bool isOptimiserNeeded = run.IsOptimiserNeeded;
                bool isSmoothNeeded = run.Smooth;

                IReadOnlyList<SalesArea> salesAreas = GetSalesAreas(run, _salesAreaRepository.GetAll());
                List<AutoBookInstanceConfiguration> autoBookInstanceConfigurationsForRun = new List<AutoBookInstanceConfiguration>();

                int breaks = GetBreakCountEstimate(run, salesAreas, _repositoryFactory);
                double autoBookRequiredStorageGB = _autoBooks.GetRequiredStorageForBreakCountInGB(breaks);
                RaiseInfo($"Estimate of Run size for AutoBook type, run Id: {run.Id}: Sales Areas: {salesAreas.Count.ToString()} Estimated Breaks: {breaks.ToString()} RequiredStorage: {autoBookRequiredStorageGB.ToString()}");

                if (isOptimiserNeeded)
                {
                    var allAutoBookInstanceConfigurations = autoBookInstanceConfigurationRepository.GetAll();

                    autoBookInstanceConfigurationsForRun = GetInstanceConfigurationsOrderedByCost(run, _salesAreaRepository.GetAll().ToList(), campaignRepository.GetAllFlat().ToList(),
                        demographicRepository.GetAll().ToList(), allAutoBookInstanceConfigurations);

                    var autoBookTypeDescriptions = new StringBuilder();
                    foreach (var currentAutoBookInstanceConfiguration in autoBookInstanceConfigurationsForRun)
                    {
                        _ = autoBookTypeDescriptions.Append(currentAutoBookInstanceConfiguration.Description + " ");
                    }
                    RaiseInfo($"AutoBook configurations suitable for RunID: {run.Id.ToString()} are: {autoBookTypeDescriptions.ToString()}");

                    RaiseInfo($"AutoDistributed - avoiding manage create and delete");
                    if (!_autoBooks.Settings.AutoDistributed)
                    {
                        if (_autoBooks.Settings.AutoProvisioning)
                        {
                            // Provision autobooks
                            using (MachineLock.Create("xggameplan.AWSAutoBooks.CheckProvisioning", new TimeSpan(0, 10, 0)))
                            {
                                var manageAutoBooks = ManageAutoBooksBeforeRunStart();
                                manageAutoBooks.CreateAndDelete(run, autoBookInstanceConfigurationsForRun.First());
                            }
                        }
                    }
                }

                // Set default status to set each scenario at the end, Scheduled allows retry. If scenario is started then we remove the scenario from the list because it had it's
                // status updated. If error is not transient then status may be set to CompletedError.
                var newScenarioStatuses = new ConcurrentDictionary<Guid, ScenarioStatuses>();
                run.Scenarios.ForEach(scenario => newScenarioStatuses.TryAdd(scenario.Id, ScenarioStatuses.CompletedError));
                run.Scenarios.ForEach(scenario => scenarioSyncStatuses.TryAdd(scenario.Id, true));

                try
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForSystemState(0, 0, "Before run started: ", GetSystemState(_repositoryFactory)));

                    // Lock to prevent other StartRun method from getting this run and starting it
                    using (MachineLock.Create(nameof(AllScenariosStartRun), TimeSpan.FromMinutes(10)))
                    {
                        // Flag run as locked. Logically we set Status=Scheduled when run is executed but there's
                        // no point doing it here because we set to Starting immediately afterwards.
                        // We set it to Scheduled below if we can't start it.
                        run.IsLocked = true;

                        foreach (var currentScenario in run.Scenarios)
                        {
                            currentScenario.Status = run.Smooth
                                ? ScenarioStatuses.Smoothing
                                : ScenarioStatuses.Starting;
                            currentScenario.StartedDateTime = startedDateTime;
                        }

                        runRepository.Update(run);
                        runRepository.SaveChanges();
                    }

                    RaiseInfo($"ResetDataForStartRun RunID={run.Id.ToString()}");
                    try
                    {
                        ResetDataForStartRun(run); //we are crashing here intermittently with a Raven stale index when removing Recommendations, so catching that and resetting the Run
                    }
                    catch (Exception exception)
                    {
                        //try this
                        RaiseInfo($"Error in ResetDataForStartRun, resetting scenarios to Pending RunID={run.Id.ToString()}, need to re-Execute the Run");
                        run.Scenarios.ForEach(scenario =>
                        newScenarioStatuses.TryUpdate(scenario.Id, ScenarioStatuses.Pending, ScenarioStatuses.CompletedError));

                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error in ResetDataForStartRun RunID={run.Id}", exception));
                        throw;
                    }

                    RecalculateBreakAvailabilityBeforeRunStarts(run, salesAreas);

                    if (isSmoothNeeded)
                    {
                        ExecuteSmooth(run, salesAreas);
                    }
                    else
                    {
                        RaiseInfo($"Not executing Smooth because it is disabled (RunID={run.Id.ToString()})");

                        UpdateDataForSmoothDisabled(run, salesAreas);
                    }

                    if (isOptimiserNeeded)
                    {
                        // Change scenario status to starting, might currently be Smoothing
                        RaiseInfo($"Optimiser needed setting scenario statuses to starting RunID: {run.Id.ToString()}");
                        if (run.Scenarios.Any(s => s.Status != ScenarioStatuses.Starting))
                        {
                            UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, runId, run.Scenarios.Select(s => s.Id).ToList(),
                                     run.Scenarios.Select(s => ScenarioStatuses.Starting).ToList());
                        }

                        scenarioTasks.AddRange(
                            run.Scenarios.Select(s => _runScenarioTaskExecutor.Execute(run, s,
                                autoBookInstanceConfigurationsForRun.AsReadOnly(), autoBookRequiredStorageGB, runInstances,
                                newScenarioStatuses, scenarioSyncStatuses, _autoBooks.Settings.AutoDistributed)));
                    }
                    else    // No AutoBooks required
                    {
                        // Set scenarios as completed
                        run.Scenarios.ForEach(scenario => newScenarioStatuses[scenario.Id] = ScenarioStatuses.CompletedSuccess);
                    }
                }
                catch (Exception exception)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        $"Error starting run (RunID={run.Id}", exception));
                    throw;
                }
                finally
                {
                    // Wait for all scenarios
                    if (isOptimiserNeeded)
                    {
                        if (scenarioTasks.Count > 0)
                        {
                            try
                            {
                                Task.WaitAll(scenarioTasks.ToArray());
                            }
                            catch (Exception exception)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                                    $"Error waiting for scenario tasks to complete (RunID={run.Id})",
                                    exception));
                            }
                        }
                    }

                    // Update any scenarios that need to be updated. E.g. Reset to Scheduled so that it gets retried later or
                    // CompletedError if the error is not transient. We check the current status because it has been seen
                    // that there was a big delay (threading?) and one of the scenarios completed.
                    List<Guid> scenarioIds = new List<Guid>();
                    List<ScenarioStatuses> scenarioStatuses = new List<ScenarioStatuses>();
                    List<DateTime?> scenarioStartedDateTimes = new List<DateTime?>();
                    List<DateTime?> scenarioCompletedDateTimes = new List<DateTime?>();
                    DateTime? completedDateTime = DateTime.UtcNow;
                    int countScenariosCompleted = 0;

                    using (var scope2 = _repositoryFactory.BeginRepositoryScope())
                    {
                        // Check current scenario statuses in case some have
                        // unexpectedly completed
                        var runRepository2 = scope2.CreateRepository<IRunRepository>();
                        var run2 = runRepository2.Find(runId);

                        foreach (var scenario in run2.Scenarios)
                        {
                            if (!isOptimiserNeeded)    // AutoBooks not needed
                            {
                                scenarioIds.Add(scenario.Id);
                                scenarioStatuses.Add(newScenarioStatuses[scenario.Id]);
                                countScenariosCompleted++;
                                scenarioStartedDateTimes.Add(startedDateTime);
                                scenarioCompletedDateTimes.Add(completedDateTime);
                            }
                            else if (scenario.IsCompleted)   // Run completed, don't set the status
                            {
                                countScenariosCompleted++;
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"Scenario has unexpectedly completed before all of the scenarios could be started (RunID={runId}, ScenarioID={scenario.Id}, Status={scenario.Status})"));
                            }
                            else if (newScenarioStatuses.ContainsKey(scenario.Id))   // Need to update status
                            {
                                scenarioIds.Add(scenario.Id);
                                scenarioStatuses.Add(newScenarioStatuses[scenario.Id]);

                                // Set Scenario.StartedDateTime & Scenario.CompletedDateTime
                                switch (newScenarioStatuses[scenario.Id])
                                {
                                    case ScenarioStatuses.CompletedError:
                                    case ScenarioStatuses.CompletedSuccess:
                                    case ScenarioStatuses.Deleted:
                                        countScenariosCompleted++;
                                        scenarioStartedDateTimes.Add(startedDateTime);
                                        scenarioCompletedDateTimes.Add(completedDateTime);
                                        break;

                                    case ScenarioStatuses.Scheduled:
                                        scenarioStartedDateTimes.Add(null);
                                        scenarioCompletedDateTimes.Add(null);
                                        break;

                                    default: // Starting, Smoothing etc
                                        scenarioStartedDateTimes.Add(startedDateTime);
                                        scenarioCompletedDateTimes.Add(null);
                                        break;
                                }
                            }
                        }
                    }

                    if (!isOptimiserNeeded)
                    {
                        // Create empty run output
                        CreateEmptyRunOutput(run, completedDateTime.Value);

                        // Generate 'Run completed' notification
                        try
                        {
                            foreach (var runScenario in run.Scenarios)
                            {
                                _runCompletionNotifier.Notify(run, runScenario, scenarioStatuses[run.Scenarios.IndexOf(runScenario)] == ScenarioStatuses.CompletedSuccess);
                            }
                            _runCompletionNotifier.Notify(run, scenarioStatuses.Count(s => s == ScenarioStatuses.CompletedSuccess) == run.Scenarios.Count);
                        }
                        catch { };      // Ignore error
                    }

                    if (scenarioIds.Count > 0)
                    {
                        UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, runId, scenarioIds, scenarioStatuses,
                               scenarioStartedDateTimes, scenarioCompletedDateTimes);
                    }

                    // If run has reached final status with failure then log event
                    if (countScenariosCompleted == run.Scenarios.Count)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunCompleted(0, 0, run.Id, null));
                    }

                    // Log system state
                    try
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForSystemState(0, 0, "After run started", GetSystemState(_repositoryFactory)));
                    }
                    catch
                    {
                    }
                }

                return runInstances.ToList();
            }
            finally
            {
                if (scenarioSyncStatuses.Values.All(s => s))
                {
                    _synchronizationService.Release(runId);
                }
            }
        }

        public void CreateInputFilesForTest(Run run)
        {
            _ = _optimiserInputFiles.PopulateRunData(run);

            foreach (var scenario in run.Scenarios)
            {
                _ = _optimiserInputFiles.PopulateScenarioData(run, scenario.Id);
            }
        }

        public void ProcessSpotsOutputFileForTest(Run run, string testFileFolder)
        {
            var scenarioId = run.Scenarios.First().Id;

            using var kpiCalculationScope = _kpiCalculationScopeFactory.CreateCalculationScope(run.Id, scenarioId);
            var dataSnapshot = kpiCalculationScope.Resolve<IOutputDataSnapshot>();
            var spotsReqmOutputFileProcessor = new SpotsReqmOutputFileProcessor(dataSnapshot, _auditEventRepository, _campaignRepository, _mapper);

            _ = spotsReqmOutputFileProcessor.ProcessFile(scenarioId, testFileFolder);
        }

        /// <summary>
        /// Updates scenario status for multiple scenarios and forces save
        /// changes. This method is necessary because starting/completing
        /// scenario run are long running and scenarios can be started/completed
        /// on different threads. We need to be careful of getting a Run from a
        /// repository and using it for a long period of time, particularly if
        /// it could be modified elsewhere (such as another thread).
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="runId"></param>
        /// <param name="scenarioIds"></param>
        /// <param name="scenarioStatuses"></param>
        /// <param name="scenarioStartedDateTimes"></param>
        /// <param name="scenarioCompletedDateTimes"></param>
        /// <returns></returns>
        public static void UpdateScenarioStatuses(IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository, Guid runId, List<Guid> scenarioIds, List<ScenarioStatuses> scenarioStatuses,
                                                List<DateTime?> scenarioStartedDateTimes = null, List<DateTime?> scenarioCompletedDateTimes = null)
        {
            using var scope = repositoryFactory.BeginRepositoryScope();
            var runRepository = scope.CreateRepository<IRunRepository>();

            using (MachineLock.Create(String.Format("xggameplan.UpdateRun.{0}", runId), TimeSpan.FromMinutes(5)))     // Prevent simultaneous updates
            {
                var run = runRepository.Find(runId);

                for (int index = 0; index < scenarioIds.Count; index++)
                {
                    var scenario = run.Scenarios.Find(s => s.Id == scenarioIds[index]);

                    if ((scenario.Status == ScenarioStatuses.CompletedError || scenario.Status == ScenarioStatuses.CompletedSuccess)
                        && scenarioStatuses[index] == ScenarioStatuses.InProgress)
                    {
                        throw new Exception($"Reverting scenario's completed status occurred. RunId:" +
                            $" {runId}, ScenarioId: {scenario.Id}, Current Scenario Status: {scenario.Status.ToString()}");
                    }

                    if (scenario.Status == ScenarioStatuses.CompletedError
                        && scenarioStatuses[index] != ScenarioStatuses.CompletedError)
                    {
                        //preventing status change when scenario status is CompletedError already
                        throw new Exception($"Cannot transition the scenario (Id: {scenario.Id}) status " +
                            $"from {scenario.Status.ToString()} to {scenarioStatuses[index].ToString()}. RunId: {runId}");
                    }

                    //changing statuses
                    if (auditEventRepository != null)
                    {
                        auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(default, default,
                            $"Transition of scenario's (Id: {scenario.Id}) status" +
                            $" from {scenario.Status.ToString()} to {scenarioStatuses[index].ToString()}. RunId: {runId}"));
                    }

                    scenario.Status = scenarioStatuses[index];

                    if (scenarioStartedDateTimes != null)
                    {
                        scenario.StartedDateTime = scenarioStartedDateTimes[index];
                    }
                    if (scenarioCompletedDateTimes != null)
                    {
                        scenario.CompletedDateTime = scenarioCompletedDateTimes[index];
                    }
                }

                runRepository.Update(run);
                runRepository.SaveChanges();
                return;
            }
        }

        /// <summary>
        /// Updates main run properties and forces save changes. This is
        /// necessary because we need to be careful about the Run document being
        /// updated while multiple scenarios are in progress.
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="runId"></param>
        /// <param name="isLocked"></param>
        /// <returns></returns>
        public static Run UpdateRun(IRepositoryFactory repositoryFactory, Guid runId, bool? isLocked)
        {
            using var scope = repositoryFactory.BeginRepositoryScope();
            var runRepository = scope.CreateRepository<IRunRepository>();
            using (MachineLock.Create(String.Format("xggameplan.UpdateRun.{0}", runId), new TimeSpan(0, 5, 0)))     // Prevent simultaneous updates
            {
                var run = runRepository.Find(runId);
                if (isLocked != null)
                {
                    run.IsLocked = isLocked.Value;
                }

                runRepository.Update(run);
                runRepository.SaveChanges();
                return run;
            }
        }

        /// <summary>
        /// Starts run of next scheduled scenario on specific AutoBook instance.
        /// </summary>
        /// <param name="autoBook"></param>
        /// <returns>Whether any run started</returns>
        public RunInstance NextScheduledScenarioStartRun(AutoBook autoBook)
        {
            if (autoBook.Locked)
            {
                throw new ArgumentException($"{nameof(NextScheduledScenarioStartRun)} Cannot start run for AutoBook {autoBook.Id} because it is locked");
            }

            RaiseInfo($"Starting {nameof(NextScheduledScenarioStartRun)} next run on AutoBook {autoBook.Id}");

            RunInstance runInstance = null;
            Guid scenarioId = Guid.Empty;
            Run run = null;
            bool startSuccess = false;
            try
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                var repositories = scope.CreateRepositories(
                    typeof(IRunRepository),
                    typeof(ISalesAreaRepository),
                    typeof(ICampaignRepository),
                    typeof(IAutoBookInstanceConfigurationRepository),
                    typeof(IDemographicRepository)
                );
                var runRepository = repositories.Get<IRunRepository>();
                var salesAreaRepository = repositories.Get<ISalesAreaRepository>();
                var campaignRepository = repositories.Get<ICampaignRepository>();
                var autoBookInstanceConfigurationRepository = repositories.Get<IAutoBookInstanceConfigurationRepository>();
                var demographicRepository = repositories.Get<IDemographicRepository>();

                DateTime? startedDateTime = DateTime.UtcNow;

                try
                {
                    var allSalesAreas = salesAreaRepository.GetAll().ToList();
                    var allAutoBookInstanceConfigurations = autoBookInstanceConfigurationRepository.GetAll().ToList();
                    var allCampaigns = campaignRepository.GetAllFlat().ToList();
                    var demographics = demographicRepository.GetAll().ToList();

                    // Lock to prevent other StartRun method from getting the same run
                    using (MachineLock.Create("xggameplan.RunManager.NextScheduledScenarioStartRun", TimeSpan.FromMinutes(10)))
                    {
                        // Get next scheduled run and scenario that can be executed on this AutoBook instance
                        RaiseInfo($"Starting {nameof(NextScheduledScenarioStartRun)} autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                        run = GetNextScheduledRunAndScenario(autoBook, runRepository, allAutoBookInstanceConfigurations, allSalesAreas, allCampaigns, demographics, out scenarioId);

                        // Flag run starting
                        if (run != null) // Run found
                        {
                            UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, run.Id,
                              new List<Guid>() { scenarioId },
                               new List<ScenarioStatuses>() { ScenarioStatuses.Starting },
                               new List<DateTime?>() { startedDateTime });
                            RaiseInfo($"StartRun(autobook) Updated ScenarioStatus RunId: {run.Id} ScenarioId: {scenarioId} autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                        }
                    }

                    if (run != null)
                    {
                        RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} Starting Run RunId: {run.Id} ScenarioId: {scenarioId} autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                        runInstance = _runInstanceCreator.Create(run.Id, scenarioId);
                        IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
                        runInstance.UploadInputFilesStartAutoBookRun(autoBookInterface, autoBook);
                        startSuccess = true;
                    }

                    else if (_autoBooks.Settings.AutoProvisioning)
                    {
                        //Run is null - delete the autobook if we've reached the max, then create a new one of suitable size if still some to process.
                        RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} Run is null autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                        var runsActive = runRepository.GetAllActive(); //returns all that have scenarios that are scheduled or running

                        if (runsActive.Any())
                        {
                            RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} {runsActive.Count()} Runs in progress. autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                            bool autoBookCreated = false;

                            foreach (var thisRun in runsActive)
                            {
                                RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} Foreach Run in progress RunId: {thisRun.Id} autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");

                                var autoBookConfigurationsForThisRun = GetInstanceConfigurationsOrderedByCost(thisRun, _salesAreaRepository.GetAll().ToList(),
                                    campaignRepository.GetAllFlat().ToList(), demographics, allAutoBookInstanceConfigurations);

                                var autoBookTypeMin = autoBookConfigurationsForThisRun.First(); //smallest suitable for this run
                                var manageAutoBooks = ManageAutoBooksBeforeRunStart();

                                foreach (var thisScenario in thisRun.Scenarios)
                                {
                                    RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} Foreach Scenario in run.scenario ScenarioId: {thisScenario.Id}  RunId: {thisRun.Id} autobookId: {autoBook.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                                    if (thisScenario.Status == ScenarioStatuses.Scheduled)
                                    {
                                        if ((_autoBooks.Instances >= _autoBooks.Settings.MaxInstances || _autoBooks.Settings.MaxInstances == 0) && (AutoBook.IsOKForDelete(autoBook.Status)))
                                        {
                                            var delid = autoBook.Id;
                                            manageAutoBooks.DeleteAutoBook(autoBook);
                                            RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} Deleted autobookId {delid} for ScenarioId: {thisScenario.Id}  RunId: {thisRun.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                                        }

                                        var newautobook = manageAutoBooks.CreateAutoBookOfType(autoBookTypeMin.Id); //Create one of correct size... PUT will pick up scenario when it goes idle
                                        autoBookCreated = true;
                                        RaiseInfo($"{nameof(NextScheduledScenarioStartRun)} Created autobook {newautobook.Id} for ScenarioId: {thisScenario.Id}  RunId: {thisRun.Id} AutoBooks.Instances: {_autoBooks.Instances}");
                                        break;
                                    }
                                }

                                if (autoBookCreated)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    RaiseException($"{nameof(NextScheduledScenarioStartRun)} Try Block failed starting next scenario " +
                        $"(AutoBookID={autoBook.Id}, RunID={(run == null ? "None" : run.Id.ToString())}, " +
                        $"ScenarioID={(scenarioId == Guid.Empty ? "None" : scenarioId.ToString())})",
                        exception);
                    throw;
                }
                finally
                {
                    // If run didn't start revert scenario status so it gets retried later
                    if (run != null && !startSuccess)
                    {
                        DateTime? completedDateTime = DateTime.UtcNow;
                        UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, run.Id,
                                new List<Guid>() { scenarioId },
                                new List<ScenarioStatuses>() { ScenarioStatuses.CompletedError }, null,
                                new List<DateTime?>() { completedDateTime });
                    }
                }

                return (startSuccess) ? runInstance : null;
            }
            finally
            {
                if (run != null)
                {
                    using var scope = _repositoryFactory.BeginRepositoryScope();
                    var runRepository = scope.CreateRepository<IRunRepository>();
                    run = runRepository.Find(run.Id);
                    if (run.IsCompleted)
                    {
                        _synchronizationService.Release(run.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Delete run. Cancels any running scenario and deletes all data.
        /// </summary>
        /// <param name="runId">The run identifier.</param>
        public void DeleteRun(Guid runId)
        {
            Task.Run(async () =>
            {
                await _runCleaner.ExecuteAsync(runId).ConfigureAwait(false);
            }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns next Run with next scheduled scenario that can be executed on the AutoBook
        ///
        /// TODO: Improve this to return the largest run that can be executed on this AutoBook. This doesn't matter if all scenarios are the
        /// same size but logically then we can support simultaneously having scenarios of different sizes.
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        private Run GetNextScheduledRunAndScenario(
            AutoBook autoBook,
            IRunRepository runRepository,
            List<AutoBookInstanceConfiguration> allAutoBookInstanceConfigurations,
            List<SalesArea> allSalesAreas,
            List<CampaignReducedModel> allCampaigns,
            List<Demographic> demographics,
            out Guid scenarioId)
        {
            scenarioId = Guid.Empty;
            AutoBookInstanceConfiguration instanceConfiguration = allAutoBookInstanceConfigurations.Find(ic => ic.Id == autoBook.InstanceConfigurationId);

            // Check each run and break out of foreach loop when run found that can be executed on this autobook
            Run nextRun = null;
            foreach (Run run in runRepository.GetAll().OrderByDescending(x => x.CustomId))    // Order for predicatable behaviour
            {
                RaiseInfo($"Checking if run: {run.Id} can be executed on AutoBook: {autoBook.Id}");
                if (run.ScheduledScenarios.Count > 0)   // Run has scheduled scenarios
                {
                    var autoBookInstanceConfigurations = GetInstanceConfigurationsOrderedByCost(run, allSalesAreas, allCampaigns, demographics, allAutoBookInstanceConfigurations);

                    if (autoBookInstanceConfigurations.Select(x => x.Id == autoBook.InstanceConfigurationId).Any())// Check if current AutoBook type can execute this run
                    {
                        RaiseInfo($"Run {run.Id} can be executed on AutoBook {autoBook.Id}");
                        nextRun = run;
                        scenarioId = run.ScheduledScenarios.First().Id;
                        break;
                    }
                    else
                    {
                        RaiseInfo($"Run {run.Id} cannot be executed on AutoBook {autoBook.Id}");
                    }
                }
            }

            if (nextRun == null)
            {
                RaiseInfo($"No next scenario to execute on AutoBook : {autoBook.Id}");
            }
            else
            {
                RaiseInfo($"Next scenario to execute on AutoBook is RunID={nextRun.Id}, ScenarioID={scenarioId}, AutoBookID={autoBook.Id}");
            }
            return nextRun;
        }

        public List<AutoBookInstanceConfiguration> GetInstanceConfigurationsOrderedByCost(
            Run run,
            List<SalesArea> allSalesAreas,
            List<CampaignReducedModel> allCampaigns,
            List<Demographic> demographics,
            List<AutoBookInstanceConfiguration> allAutoBookInstanceConfigurations)

        {
            List<SalesArea> runSalesAreas = GetSalesAreas(run, allSalesAreas);
            var runCampaignsCnt = GetRunCampaigns(run, allCampaigns).Count;
            int breaksCnt = GetBreakCountEstimate(run, runSalesAreas, _repositoryFactory);

            var autoBookInstanceConfigurations = _autoBooks.GetInstanceConfigurationsAscByCost(run, allAutoBookInstanceConfigurations,
                runSalesAreas.Count, runCampaignsCnt, demographics.Count, breaksCnt);

            return autoBookInstanceConfigurations;
        }

        public static List<SalesArea> GetSalesAreas(Run run, IEnumerable<SalesArea> allSalesAreas)
        {
            var result = new List<SalesArea>();

            if (run.SalesAreaPriorities.Count > 0)
            {
                foreach (var sap in run.SalesAreaPriorities)
                {
                    if (sap.Priority != SalesAreaPriorityType.Exclude)
                    {
                        result.Add(allSalesAreas.First(x => x.Name == sap.SalesArea));
                    }
                }
            }
            else
            {
                result.AddRange(allSalesAreas);
            }

            return result;
        }

        /// <summary>
        /// Validates before the run can be started. We perform basic checks in
        /// order to identify issues now rather than later during the run.
        /// </summary>
        /// <param name="run"></param>
        public List<SystemMessage> ValidateForStartRun(Run run)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var scheduleRepository = scope.CreateRepository<IScheduleRepository>();

            var systemMessageRepository = new SystemMessageRepository();

            // TODO: Set IMaintenanceSettingsRepository
            var validateForRun = new ValidateForRun(
                _repositoryFactory,
                _isrSettingsRepository,
                _rsSettingsRepository,
                _outputFileRepository,
                _universeRepository,
                _spotRepository,
                scheduleRepository,
                _ratingsScheduleRepository,
                _productRepository,
                _clashRepository,
                systemMessageRepository,
                _autoBooks,
                _tenantSettingsRepository,
                _clearanceRepository,
                _featureManager,
                _systemLogicalDateService);

            return validateForRun.Validate(run);
        }

        /// <summary>
        /// Returns estimated number of breaks in run, counting actual breaks would take too long. We assume that the number of breaks are similar for each sales area.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        /// <returns></returns>
        public static int GetBreakCountEstimate(Run run, IReadOnlyList<SalesArea> salesAreas, IRepositoryFactory _repositoryFactory)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
            int days = (int)(run.EndDate - run.StartDate).TotalDays + 1;

            // Get number of breaks, estimate from first schedule - checking the first ten for non zero values of breaks
            DateTime scheduleDate = run.StartDate.AddDays(-1);
            int schedulesChecked = 0;
            int breakCount = 0;

            do
            {
                schedulesChecked++;
                scheduleDate = scheduleDate.AddDays(1);
                var breaksCount = scheduleRepository.GetScheduleBreaksCount(salesAreas[0].Name, scheduleDate.Date);

                breaksCount = (breaksCount == 0 ? breaksCount : (breakCount * days * salesAreas.Count));    //get zero for breaksCout or get (breaksCont * days * salesarea count)
            } while (breakCount == 0 && scheduleDate < run.EndDate && schedulesChecked < 10);   //repeat until not zero OR schedule date < endDate OR first ten schedules have been checked

            return breakCount;
        }

        /// <summary>
        /// Returns the system state for the run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="repositoryFactory"></param>
        /// <returns></returns>
        public static SystemState GetSystemState(IRepositoryFactory repositoryFactory)
        {
            SystemState systemState = new SystemState();
            using (var scope = repositoryFactory.BeginRepositoryScope())
            {
                var repositories = scope.CreateRepositories(
                    typeof(IBreakRepository),
                    typeof(ICampaignRepository),
                    typeof(IClashRepository),
                    typeof(IProductRepository),
                    typeof(IProgrammeDictionaryRepository),
                    typeof(IProgrammeRepository),
                    typeof(IRatingsScheduleRepository),
                    typeof(IScheduleRepository),
                    typeof(ISpotRepository)
                );

                var breakRepository = repositories.Get<IBreakRepository>();
                var campaignRepository = repositories.Get<ICampaignRepository>();
                var clashRepository = repositories.Get<IClashRepository>();
                var productRepository = repositories.Get<IProductRepository>();
                var programmeDictionaryRepository = repositories.Get<IProgrammeDictionaryRepository>();
                var programmeRepository = repositories.Get<IProgrammeRepository>();
                var ratingsScheduleRepository = repositories.Get<IRatingsScheduleRepository>();
                var scheduleRepository = repositories.Get<IScheduleRepository>();
                var spotRepository = repositories.Get<ISpotRepository>();

                // Set object counts to monitor
                systemState.ObjectCounts.Add("Breaks", breakRepository.CountAll);
                systemState.ObjectCounts.Add("Campaigns", campaignRepository.CountAll);
                systemState.ObjectCounts.Add("Campaigns (Active)", campaignRepository.CountAllActive);
                systemState.ObjectCounts.Add("Clash", clashRepository.CountAll);
                systemState.ObjectCounts.Add("Products", productRepository.CountAll);
                systemState.ObjectCounts.Add("Programme Dictionaries", programmeDictionaryRepository.CountAll);
                systemState.ObjectCounts.Add("Programmes", programmeRepository.CountAll);
                systemState.ObjectCounts.Add("Ratings Prediction Schedules", ratingsScheduleRepository.CountAll);
                systemState.ObjectCounts.Add("Schedules", scheduleRepository.CountAll);
                systemState.ObjectCounts.Add("Spots", spotRepository.CountAll);
            }
            return systemState;
        }

        /// <summary>
        /// Gets campaigns for run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="allCampaigns"></param>
        /// <returns></returns>
        public static List<CampaignReducedModel> GetRunCampaigns(Run run, IEnumerable<CampaignReducedModel> allCampaigns)
        {
            if (run.Campaigns.Count == 0)
            {
                return allCampaigns.Where(c => c.IsActive).ToList();
            }

            // Specific campaigns
            var externalRefs = new HashSet<string>(run.Campaigns.Select(c => c.ExternalId));
            return allCampaigns.Where(c => externalRefs.Contains(c.ExternalId)).ToList();
        }

        /// <summary>
        /// Checks run system state, logs event if different
        /// </summary>
        private bool CheckSystemState(Guid runId, SystemState systemState, SystemState lastSystemState)
        {
            bool changed = false;
            if (lastSystemState != null && !systemState.IsSame(lastSystemState))
            {
                changed = true;
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, String.Format("System state has changed during run: RunID={0}; Current:{1}; Previous:{2}",
                                                        runId, systemState.Description, lastSystemState.Description)));
            }
            return changed;
        }

        /// <summary>
        /// Updates data that is necessary due to Smooth being disabled.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        private void UpdateDataForSmoothDisabled(Run run, IReadOnlyList<SalesArea> salesAreas)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("Saving spot placements because Smooth is disabled and Smooth would normally do this (RunID={0})", run.Id)));
            ScheduleReset scheduleReset = new ScheduleReset(_repositoryFactory);
            scheduleReset.UpdateSpotPlacementData(salesAreas, run.StartDate.Date, run.EndDate.Date);
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("Saved spot placements (RunID={0})", run.Id)));
        }

        public List<Run> HandleCrashedRuns()
        {
            List<Run> crashedRuns = new List<Run>();

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                // Get repositories
                var repositories = scope.CreateRepositories(
                    typeof(IRunRepository),
                    typeof(ITaskInstanceRepository)
                );
                var runRepository = repositories.Get<IRunRepository>();
                var taskInstanceRepository = repositories.Get<ITaskInstanceRepository>();

                // Set the completion timeout for runs
                // TODO: Determine based on run size
                TimeSpan completionTimeout = TimeSpan.FromMinutes(60 * 5);

                // Set time when task instances should have notified that
                // they're active, include a tolerance
                DateTime minLastTimeActive = DateTime.UtcNow.AddSeconds(-(TaskInstance.ActiveFrequencySeconds * 5));

                // Get all recent tasks that haven't completed
                var taskInstances = taskInstanceRepository.GetAll().Where(ti => ti.TimeCreated >= DateTime.UtcNow.AddHours(-48));

                // Get all recent runs
                var runs = runRepository.GetAll().Where(r => r.CreatedDateTime >= DateTime.UtcNow.AddHours(-48));

                foreach (var run in runs)
                {
                    // Get crashed scenarios
                    List<RunScenario> crashedScenarios = run.Scenarios.Where(s => HasScenarioRunCrashed(run, s, completionTimeout, taskInstances.ToList())).ToList();

                    if (crashedScenarios.Any())
                    {
                        HandleCrashedRun(run, crashedScenarios);
                        crashedRuns.Add(run);
                    }
                }
            }
            return crashedRuns;
        }

        /// Determines if scenario run has crashed </summary> <param
        /// name="run"></param> <param name="scenario"></param> <param
        /// name="completionTimeout"></param> <param
        /// name="taskInstances"></param> <returns></returns>
        private bool HasScenarioRunCrashed(Run run, RunScenario scenario, TimeSpan completionTimeout, List<TaskInstance> taskInstances)
        {
            bool hasCrashed = false;
            switch (scenario.Status)
            {
                case ScenarioStatuses.Starting:
                case ScenarioStatuses.Smoothing:    // Statuses during the StartRun task
                    // Check the StartRun task for this run, if in progress then
                    // active notification will be updated frequently
                    foreach (var taskInstance in taskInstances.Where(ti => ti.TaskId == TaskIds.StartRun))
                    {
                        if (taskInstance.Status == TaskInstanceStatues.Starting || taskInstance.Status == TaskInstanceStatues.InProgress)   // Not complete
                        {
                            if (taskInstance.TimeLastActive <= DateTime.UtcNow.AddSeconds(-(TaskInstance.ActiveFrequencySeconds * 5)))    // No recent active notification
                            {
                                Guid runId = new Guid(taskInstance.Parameters["RunId"].ToString());
                                if (runId == run.Id)
                                {
                                    hasCrashed = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case ScenarioStatuses.InProgress:
                case ScenarioStatuses.GettingResults:  // Statuses after run started
                    hasCrashed = scenario.StartedDateTime != null && completionTimeout.TotalMilliseconds > 0 && scenario.StartedDateTime.Value.Add(completionTimeout) < DateTime.UtcNow;
                    break;
            }
            return hasCrashed;
        }

        /// <summary>
        /// Handles crashed run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="crashedScenarios"></param>
        private void HandleCrashedRun(Run run, List<RunScenario> crashedScenarios)
        {
            DateTime completedTime = DateTime.UtcNow;

            // Update any crashed
            if (crashedScenarios.Any())
            {
                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, run.Id, crashedScenarios.Select(s => s.Id).ToList(),
                                                                   crashedScenarios.Select(s => ScenarioStatuses.CompletedError).ToList(),
                                                                   null,
                                                                   crashedScenarios.Select(s => (DateTime?)completedTime).ToList());
            }
        }

        private static double GetStorageGB(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = value.ToUpper().Replace("GB", "");
                string[] elements = value.Split(' ');
                if (Double.TryParse(elements[0], out double total))    // Typically "n/a" if not known
                {
                    return total;
                }
            }
            return AutoBookSettings.UnknownStorageGB;      // Unknown
        }

        /// <summary>
        /// Creates empty output for run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="completedDateTime"></param>
        private void CreateEmptyRunOutput(
            Run run,
            DateTime completedDateTime)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var scenarioResultRepository = scope.CreateRepository<IScenarioResultRepository>();
            var failuresRepository = scope.CreateRepository<IFailuresRepository>();

            foreach (var scenario in run.Scenarios)
            {
                ScenarioResult scenarioResult = new ScenarioResult()
                {
                    Id = scenario.Id,
                    Metrics = new List<KPI>(),
                    TimeCompleted = completedDateTime
                };
                scenarioResultRepository.Add(scenarioResult);

                Failures failures = new Failures()
                {
                    Id = scenario.Id,
                };
                failuresRepository.Add(failures);
            }

            scenarioResultRepository.SaveChanges();
            failuresRepository.SaveChanges();
        }

        public void CreateNotificationForCompletedRun(Run run)
        {
            var runNotification = new RunNotification()
            {
                id = run.Id,
                description = run.Description,
                endDate = DateTime.UtcNow,
                endTime = DateTime.UtcNow.TimeOfDay,
                status = run.RunStatus
            };
            _completedRunNotification.Notify(runNotification);
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunCompleted(0, 0, run.Id, "Completed Run Notification Sent"));
        }

        public void BroadcastScenario(Guid id, ScenarioStatuses status, int currentStep, int totalSteps)
        {
            _scenarioNotification.Notify(new ScenarioNotificationModel()
            {
                id = id,
                status = status.ToString(),
                currentStep = currentStep,
                totalSteps = totalSteps
            });

            RaiseInfo($"Sent scenario status {status} update for id: {id} - step {currentStep} of {totalSteps}");
        }

        public bool Exists(Guid runId)
        {
            return _runRepository.Find(runId) != null;
        }

        public void ApplyCampaignProcessesConfigurations(
            IEnumerable<CampaignRunProcessesSettings> campaignRunProcessesSettings, Guid? runId = null)
        {
            var externalRefs = campaignRunProcessesSettings.Select(s => s.ExternalId).ToList();
            var campaignIdsToRefsMap = _campaignRepository.FindByRefs(externalRefs)
                .Select(c => (Uid: c.Id, c.ExternalId)).ToArray();

            var relatedRuns = !runId.HasValue
                ? _runRepository
                    .GetRunsByCampaignExternalIdsAndStatus(campaignIdsToRefsMap.Select(m => m.ExternalId),
                        RunStatus.NotStarted).ToArray()
                : new[] { _runRepository.Find(runId.Value) };

            foreach (var settings in campaignRunProcessesSettings)
            {
                var (Uid, ExternalId) =
                    campaignIdsToRefsMap.FirstOrDefault(m => m.ExternalId == settings.ExternalId);

                foreach (var run in relatedRuns)
                {
                    if (run.Campaigns.Any() && run.Campaigns.All(c => c.ExternalId != ExternalId))
                    {
                        break;
                    }

                    var runCampaignProcessesSetting =
                        run.CampaignsProcessesSettings.FirstOrDefault(s =>
                            s.ExternalId == settings.ExternalId);

                    if (runCampaignProcessesSetting != null)
                    {
                        _ = runCampaignProcessesSetting.Update(settings);
                    }
                    else
                    {
                        run.CampaignsProcessesSettings.Add(_mapper.Map<CampaignRunProcessesSettings>(settings));
                    }

                    _runRepository.Update(run);
                }
            }

            _runRepository.SaveChanges();
        }

        private ManageAutoBooksCreateAndDelete ManageAutoBooksBeforeRunStart()
        {
            var manageAutoBooks = new ManageAutoBooksCreateAndDelete(
                _auditEventRepository,
                _configuration,
                _autoBookInstanceConfigurationRepository,
                _autoBooks,
                _runRepository);

            return manageAutoBooks;
        }
    }
}
