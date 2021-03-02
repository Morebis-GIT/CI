using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.common.BackgroundJobs;
using xggameplan.core.RunManagement;
using xggameplan.core.Services;
using xggameplan.core.Tasks;
using xggameplan.core.Tasks.Executors;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Jobs;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.RunManagement;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for managing AutoBook instances
    /// </summary>
    [RoutePrefix("AutoBooks")]
    public class AutoBooksController : ApiController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAutoBooks _autoBooks;
        private readonly IAutoBookRepository _autoBookRepository;
        private readonly IAutoBookInstanceConfigurationRepository _autoBookInstanceConfigurationRepository;
        private readonly IAutoBookSettingsRepository _autoBookSettingsRepository;
        private readonly IRunRepository _runRepository;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IProviderLogsAPI _providerLogsAPI;
        private readonly RunInstanceCreator _runInstanceCreator;
        private readonly IMapper _mapper;
        private readonly AutoBookSettings _autoBookSettings;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IConfiguration _configuration;
        private readonly TenantIdentifier _tenantIdentifier;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IAutoBookTaskReportRepository _autoBookTaskReportRepository;
        private readonly ISystemLogicalDateService _systemLogicalDateService;

        public AutoBooksController(IRepositoryFactory repositoryFactory, IAutoBooks autoBooks, IAutoBookRepository autoBookRepository,
                                   IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository,
                                   IAutoBookSettingsRepository autoBookSettingsRepository, IRunRepository runRepository,
                                   IAuditEventRepository auditEventRepository, IProviderLogsAPI providerLogsAPI,
                                   RunInstanceCreator runInstanceCreator, IMapper mapper, AutoBookSettings autoBookSettings,
                                   IBackgroundJobManager backgroundJobManager, IConfiguration configuration,
                                   TenantIdentifier tenantIdentifier, ISynchronizationService synchronizationService,
                                   IAutoBookTaskReportRepository autoBookTaskReportRepository, ISystemLogicalDateService systemLogicalDateService)
        {
            _repositoryFactory = repositoryFactory;
            _autoBooks = autoBooks;
            _autoBookRepository = autoBookRepository;
            _autoBookInstanceConfigurationRepository = autoBookInstanceConfigurationRepository;
            _autoBookSettingsRepository = autoBookSettingsRepository;
            _runRepository = runRepository;
            _auditEventRepository = auditEventRepository;
            _providerLogsAPI = providerLogsAPI;
            _runInstanceCreator = runInstanceCreator;
            _mapper = mapper;
            _autoBookSettings = autoBookSettings;
            _backgroundJobManager = backgroundJobManager;
            _configuration = configuration;
            _tenantIdentifier = tenantIdentifier;
            _synchronizationService = synchronizationService;
            _autoBookTaskReportRepository = autoBookTaskReportRepository;
            _systemLogicalDateService = systemLogicalDateService;
        }

        /// <summary>
        /// Returns list of AutoBook instances
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(List<AutoBookModel>))]
        public IEnumerable<AutoBookModel> GetAutoBooks()
        {
            var autoBooks = _autoBookRepository.GetAll();
            var autoBookModels = new List<AutoBookModel>();
            autoBooks.ToList().ForEach(autoBook => autoBookModels.Add(GetAutoBookModel(autoBook)));
            return autoBookModels;
        }

        /// <summary>
        /// Executes AutoBook provisioning
        /// </summary>
        /// <returns></returns>
        [Route("ExecuteProvisioning")]
        [AuthorizeRequest("AutoBooks")]
        public IHttpActionResult PutExecuteProvisioning()
        {
            _autoBooks.ExecuteProvisionining();
            return Ok();
        }

        /// <summary>
        /// Returns AutoBookModel from AutoBook. AutoBookModel returns some additional dynamic properties from the AutoBook
        /// instance (E.g. Version, Actual status). AutoBook.Status should be up to date but we can't guarantee.
        /// </summary>
        /// <param name="autoBook"></param>
        /// <returns></returns>
        private AutoBookModel GetAutoBookModel(AutoBook autoBook)
        {
            var autoBookModel = _mapper.Map<AutoBookModel>(autoBook);
            try
            {
                // Get PAAutoBook from Provisioning API
                PAAutoBook paAutoBook = _autoBooks.GetPAAutoBook(autoBook);
                if (paAutoBook is null)
                {
                    autoBookModel.Status = AutoBookStatuses.Fatal_Error;
                    return autoBookModel;
                }

                // Get AutoBook interface so that we can query the AutoBook for dynamic properties
                IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
                autoBookModel.Version = paAutoBook.Version;
                autoBookModel.Status = (paAutoBook.Provisioned ? autoBookInterface.GetStatus() : AutoBookStatuses.Provisioning);
            }
            catch
            {
                return autoBookModel;
            };
            return autoBookModel;
        }

        /// <summary>
        /// Returns AutoBook audit trail for Scenario ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [Route("{id}/scenarios/{scenarioId}/audittrail")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(String))]
        public IHttpActionResult GetScenarioAuditTrail([FromUri] string id, [FromUri] Guid scenarioId)
        {
            // Get AutoBook
            var autoBook = _autoBookRepository.Get(id);
            if (autoBook == null)
            {
                return NotFound();
            }

            // Get run
            var run = _runRepository.FindByScenarioId(scenarioId);
            if (run == null)
            {
                return NotFound();
            }

            // Get audit trail
            IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
            var auditTrail = autoBookInterface.GetAuditTrail(scenarioId);
            return Ok(auditTrail.Message);
        }

        /// <summary>
        /// Returns AutoBook log for Scenario ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [Route("{id}/scenarios/{scenarioId}/logs")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(String))]
        public IHttpActionResult GetScenarioLogs([FromUri] string id, [FromUri] Guid scenarioId)
        {
            // Get AutoBook
            var autoBook = _autoBookRepository.Get(id);
            if (autoBook == null)
            {
                return NotFound();
            }

            // Get run
            var run = _runRepository.FindByScenarioId(scenarioId);
            if (run == null)
            {
                return NotFound();
            }

            IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
            var logs = autoBookInterface.GetLogs(scenarioId);
            return Ok(logs.Message);
        }

        /// <summary>
        /// Returns AutoBook instance by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(AutoBookModel))]
        public IHttpActionResult GetAutoBook(string id)
        {
            var item = _autoBookRepository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            var autoBookModel = _mapper.Map<AutoBookModel>(GetAutoBookModel(item));
            return Ok(autoBookModel);
        }

        /// <summary>
        /// Restarts AutoBook instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/restart")]
        [AuthorizeRequest("AutoBooks")]
        public IHttpActionResult PostRestart(string id)
        {
            var autoBook = _autoBookRepository.Get(id);
            if (autoBook == null)
            {
                return NotFound();
            }

            _autoBooks.Restart(autoBook);
            return Ok();
        }

        /// <summary>
        /// Creates AutoBook instance. The AutoBook instance will be provisioned in the background.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(AutoBookModel))]
        public IHttpActionResult Post([FromBody] CreateAutoBookModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid AutoBook parameters");
            }

            // Check if provisioning is locked
            if (_autoBookSettings.Locked)
            {
                this.Error().UnknownError("Cannot create AutoBook instance because provisioning functions are temporarily locked");
            }

            // Check instance type
            var autoBookType = _autoBookInstanceConfigurationRepository.Get(model.InstanceConfigurationId);
            if (autoBookType == null)
            {
                return this.Error().InvalidParameters("Cannot create AutoBook instance because the instance configuration is not valid");
            }

            // Check if instancing limit reached
            if (_autoBooks.Instances >= _autoBooks.Settings.MaxInstances)
            {
                return BadRequest("Cannot create AutoBook instance because the instance limit has been reached");
            }

            // Check that the AutoBook API version if set
            if (String.IsNullOrEmpty(_autoBooks.Settings.ApplicationVersion))
            {
                this.Error().UnknownError("Cannot create AutoBook instance because the version is not set in the configuration");
            }

            // Set AutoBook properties
            var autoBook = new AutoBook()
            {
                InstanceConfigurationId = model.InstanceConfigurationId,
                Status = AutoBookStatuses.Provisioning,
                TimeCreated = _systemLogicalDateService.GetSystemLogicalDate(),
                Locked = false,
            };

            // Start creating autobook
            _backgroundJobManager.StartJob<AutoBookCreateBackgroundJob>(new BackgroundJobParameter<AutoBook>(autoBook));

            var autoBookModel = _mapper.Map<AutoBookModel>(autoBook);
            return Ok(autoBookModel);
        }

        /// <summary>
        /// Updates AutoBook instance
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(AutoBookModel))]
        public IHttpActionResult PutUpdate([FromBody] AutoBookModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid AutoBook parameters");
            }

            // Get AutoBook details
            var autoBook = _autoBookRepository.Get(command.Id);
            _autoBookRepository.Update(autoBook);
            _autoBookRepository.SaveChanges();

            var autoBookModel = _mapper.Map<AutoBookModel>(autoBook);
            return Ok(autoBookModel);
        }

        /// <summary>
        /// Resets scenario status to Scheduled if it is running. This is necessary if there is an issue with the AutoBook instance and
        /// we need to reset it so that it gets retried later.
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        private void ResetScenarioToScheduledIfRunning(Guid runId, Guid scenarioId)
        {
            Run oldRun = _runRepository.Find(runId);
            RunScenario oldScenario = oldRun?.Scenarios.Find(currentScenario => currentScenario.Id == scenarioId);

            if (oldScenario != null &&
                Array.IndexOf(new ScenarioStatuses[]
                {
                    ScenarioStatuses.Starting,
                    ScenarioStatuses.Smoothing,
                    ScenarioStatuses.InProgress,
                    ScenarioStatuses.GettingResults
                }, oldScenario.Status) != -1)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $@"Resetting old scenario to Scheduled for ScenarioId {scenarioId} because AutoBook has indicated that it is Idle"));

                oldScenario.Status = ScenarioStatuses.Scheduled;
                _runRepository.Update(oldRun);
            }
        }

        /// <summary>
        /// Updates AutoBook instance. This is for internal use by the AutoBook API in order to notify status changes.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(AutoBookModel))]
        public IHttpActionResult Put([FromUri] string id,
                                    [FromBody] UpdateAutoBookModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"Processing AutoBook PUT - Error Invalid Model"));
                return this.Error().InvalidParameters("Invalid AutoBook parameters");
            }
            else
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"Processing AutoBook PUT - command: {command}, id = {id}"));
            }

            AutoBookModel autoBookModel = null;

            if (!_autoBooks.Settings.AutoDistributed)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"Processing AutoBook PUT - AutoProvisioned mode: Status is set to Idle"));
                // Check AutoBook exists
                var autoBook = _autoBookRepository.Get(id);
                if (autoBook == null)
                {
                    return NotFound();
                }

                var currentAutoBookTask = autoBook.Task;

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $@"Updating AutoBook (AutoBookID={id}, API={command.Api}, Status={command.Status}, " +
                    $@"NewRunID={(command.Task != null ? command.Task.RunId.ToString() : "None")}, " +
                    $@"NewScenarioID={(command.Task != null ? command.Task.ScenarioId.ToString() : "None")}), " +
                    $@"CurrentRunID={(currentAutoBookTask != null ? currentAutoBookTask.RunId.ToString() : "None")}, " +
                    $@"CurrentScenarioID={(currentAutoBookTask != null ? currentAutoBookTask.ScenarioId.ToString() : "None")})"));

                // If AutoBook previous status was Provisioning and now Idle then log event for provisioned
                if (autoBook.Status == AutoBookStatuses.Provisioning && command.Status == AutoBookStatuses.Idle)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForAutoBookEvent(0, 0,
                        autoBook.Id, AutoBookEventIDs.AutoBookProvisioned, null));
                }

                // Update proprerties
                autoBook.Api = (command.Api == null) ? autoBook.Api : command.Api;
                autoBook.Status = command.Status;
                autoBook.Task = null;
                if (command.Task != null)
                {
                    autoBook.Task = new AutoBookTask()
                    {
                        RunId = command.Task.RunId,
                        ScenarioId = command.Task.ScenarioId
                    };
                }

                try
                {
                    // Update before we do anything else
                    _autoBookRepository.Update(autoBook);
                    _autoBookRepository.SaveChanges();

                    // Get the run instance
                    Run run = (autoBook.Task != null) ? _runRepository.Find(autoBook.Task.RunId) : null;
                    RunInstance runInstance = (run != null) ? _runInstanceCreator.Create(run.Id, autoBook.Task.ScenarioId) : null;
                    var scenario = (run == null) ? null : run.Scenarios.Find(currentScenario => currentScenario.Id == autoBook.Task.ScenarioId);

                    switch (autoBook.Status)
                    {
                        case AutoBookStatuses.Idle: // System provisioned, start task
                            _autoBooks.GetInterface(autoBook).ResetFree();
                            _autoBookRepository.SaveChanges(); // Ensure changes exist before we start any background tasks

                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                $"AutoBook status has been set to Idle (AutoBookID={id})"));

                            if (currentAutoBookTask != null)
                            {
                                var currentRun = _runRepository.Find(currentAutoBookTask.RunId);
                                var currentScenario = currentRun?.Scenarios.FirstOrDefault(x => x.Id == currentAutoBookTask.ScenarioId);

                                if (currentScenario != null && !currentScenario.IsCompleted)
                                {
                                    RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository,
                                        currentRun.Id, new List<Guid>() { currentScenario.Id },
                                        new List<ScenarioStatuses>() { ScenarioStatuses.CompletedError },
                                        null, new List<DateTime?>() { _systemLogicalDateService.GetSystemLogicalDate() });

                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                        "AutoBook status is being set to Idle but the previous task has not been completed " +
                                        $@"(AutoBookID={id}, RunID={currentRun.Id}, ScenarioID={currentScenario.Id})"));

                                    return this.Error().InvalidParameters("AutoBook status is being set to Idle but the previous task has not been completed");
                                }
                            }

                            StartAutoBookRun(autoBook);

                            break;

                        case AutoBookStatuses.Task_Completed:
                            if (run == null)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                    $"AutoBook status is being set to Task_Completed but the run was not found " +
                                    $@"(AutoBookID={id}, RunID={(autoBook.Task != null ? autoBook.Task.RunId.ToString() : "Unknown")})"));
                                return this.Error().InvalidParameters("Run does not exist");
                            }

                            if (scenario.IsCompleted)
                            {
                                // A bug has been seen with AutoBook API that duplicates the calls (Task_Completed, Idle, Task_Completed, Idle).
                                // We don't want to process the results again.
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                    "AutoBook status is being set to Task_Completed but the scenario has already been completed " +
                                    $@"(AutoBookID={id}, RunID={(autoBook.Task != null ? autoBook.Task.RunId.ToString() : "Unknown")}, ScenarioID={scenario.Id})"));
                            }
                            else
                            {
                                RunManager.UpdateRun(_repositoryFactory, run.Id, false); // Unlock run on first result

                                // Reset AutoBook instance to free for re-use
                                autoBook.LastRunCompleted = _systemLogicalDateService.GetSystemLogicalDate();
                                _autoBooks.GetInterface(autoBook).ResetFree();
                                _autoBookRepository.SaveChanges(); // Ensure changes exist before we start any background tasks
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                    $"AutoBook status has been set to Task_Completed " +
                                    $"(AutoBookID={id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId})"));

                                StartAutoBookHandleCompletedSuccess(autoBook, runInstance);
                            }

                            break;

                        case AutoBookStatuses.Task_Error:
                            if (run == null)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                    "AutoBook status is being set to Task_Error but the run was not found " +
                                    $@"(AutoBookID={id}, RunID={(autoBook.Task != null ? autoBook.Task.RunId.ToString() : "Unknown")})"));
                                return this.Error().InvalidParameters("Run does not exist");
                            }

                            RunManager.UpdateRun(_repositoryFactory, run.Id, false); // Unlock run on first result

                            // Reset AutoBook instance to free for re-use
                            autoBook.LastRunCompleted = _systemLogicalDateService.GetSystemLogicalDate();
                            _autoBooks.GetInterface(autoBook).ResetFree();
                            _autoBookRepository.SaveChanges(); // Ensure changes exist before we start any background tasks
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                $"AutoBook status has been set to Task_Error " +
                                $"(AutoBookID={id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId})"));

                            StartAutoBookHandleCompletedTaskError(autoBook, runInstance);
                            break;

                        case AutoBookStatuses.Fatal_Error:
                            if (run != null)
                            {
                                RunManager.UpdateRun(_repositoryFactory, run.Id, false); // Unlock run on first result
                            }

                            // Ensure AutoBook locked so that it can't be re-used. We need to try and restart
                            autoBook.Locked = true;
                            _autoBookRepository.Update(autoBook);
                            _autoBookRepository.SaveChanges(); // Ensure changes exist before we start any background tasks
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                $"AutoBook status has been set to Fatal_Error " +
                                $"(AutoBookID={id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId})"));

                            StartAutoBookHandleCompletedFatalError(autoBook, runInstance);
                            break;
                    }
                }
                catch // Error, set scenario status to final status otherwise it remains in progress
                {
                    switch (autoBook.Status)
                    {
                        case AutoBookStatuses.Task_Completed:
                        case AutoBookStatuses.Task_Error:
                        case AutoBookStatuses.Fatal_Error:
                            if (autoBook.Task != null)
                            {
                                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository,
                                    autoBook.Task.RunId, new List<Guid>() { autoBook.Task.ScenarioId },
                                    new List<ScenarioStatuses>() { ScenarioStatuses.CompletedError },
                                    null, new List<DateTime?>() { _systemLogicalDateService.GetSystemLogicalDate() });
                            }

                            break;
                    }

                    throw;
                }
                finally
                {
                    Run run = autoBook.Task != null ? _runRepository.Find(autoBook.Task.RunId) : null;
                    if (run != null && run.IsCompleted)
                    {
                        _synchronizationService.Release(run.Id);
                    }
                }

                autoBookModel = _mapper.Map<AutoBookModel>(autoBook);
            }
            else
            {
                //mock an autoBook
                AutoBook autoBook = new AutoBook();
                AutoBookTask Task = new AutoBookTask();
                autoBook.Task = Task;
                autoBook.Id = id;
                autoBook.Api = command.Api;
                autoBook.Status = command.Status;
                autoBook.Task.RunId = (command.Task != null ? command.Task.RunId : Guid.Empty);
                autoBook.Task.ScenarioId = (command.Task != null ? command.Task.ScenarioId : Guid.Empty);

                var run = _runRepository.Find(autoBook.Task.RunId);
                RunInstance runInstance = (run != null) ? _runInstanceCreator.Create(run.Id, autoBook.Task.ScenarioId) : null;
                var scenario = (run == null) ? null : run.Scenarios.Find(currentScenario => currentScenario.Id == autoBook.Task.ScenarioId);

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                  $"Processing AutoBook PUT - AutoDistributed mode: Status is set to Idle RunID = {autoBook.Task.RunId.ToString()}, ScenarioId = {autoBook.Task.ScenarioId.ToString()}"));

                try
                {
                    switch (command.Status)
                    {
                        case AutoBookStatuses.Idle:
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"AutoDistributed mode: Status is set to Idle"));
                            break;

                        case AutoBookStatuses.Task_Completed:
                            if (run == null)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                    $"AutoDistributed mode: Status is set to Task_Completed but the run was not found RunID = {autoBook.Task.RunId.ToString()}"));
                                return this.Error().InvalidParameters("Run does not exist");
                            }

                            if (scenario.IsCompleted)
                            {
                                // A bug has been seen with AutoBook API that duplicates the calls (Task_Completed, Idle, Task_Completed, Idle).
                                // We don't want to process the results again.
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                    $"AutoDistributed mode: Status is set to Task_Completed but the scenario has already been completed RunID = {autoBook.Task.RunId.ToString()}, ScenarioID = {scenario.Id}"));
                            }
                            else
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                    $"AutoDistributed mode: Status is set to Task_Completed RunID = {runInstance.RunId}, ScenarioID = {runInstance.ScenarioId}"));

                                RunManager.UpdateRun(_repositoryFactory, run.Id, false); // Unlock run on first result
                                StartAutoBookHandleCompletedSuccess(autoBook, runInstance);
                            }
                            break;

                        case AutoBookStatuses.Task_Error:
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                    $"AutoDistributed mode: Status is set to Task_Error RunID = {runInstance.RunId}, ScenarioID = {runInstance.ScenarioId}"));

                            RunManager.UpdateRun(_repositoryFactory, run.Id, false); // Unlock run on first result
                            StartAutoBookHandleCompletedTaskError(autoBook, runInstance);
                            break;

                        case AutoBookStatuses.Fatal_Error:
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                    $"AutoDistributed mode: Status is set to Fatal_Error RunID = {runInstance.RunId}, ScenarioID = {runInstance.ScenarioId}"));

                            StartAutoBookHandleCompletedFatalError(autoBook, runInstance);
                            break;
                    }
                }
                catch
                {
                    switch (command.Status)
                    {
                        case AutoBookStatuses.Task_Completed:
                        case AutoBookStatuses.Task_Error:
                        case AutoBookStatuses.Fatal_Error:
                            if (command.Task != null)
                            {
                                RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository,
                                    autoBook.Task.RunId, new List<Guid>() { autoBook.Task.ScenarioId },
                                    new List<ScenarioStatuses>() { ScenarioStatuses.CompletedError },
                                    null, new List<DateTime?>() { _systemLogicalDateService.GetSystemLogicalDate() });
                            }

                            break;
                    }
                    throw;
                }
                finally
                {
                    if (run != null && run.IsCompleted)
                    {
                        _synchronizationService.Release(run.Id);
                    }
                }

                autoBookModel = _mapper.Map<AutoBookModel>(autoBook);
            }

            return Ok(autoBookModel);
        }

        private void StartAutoBookHandleCompletedFatalError(AutoBook autoBook, RunInstance runInstance)
        {
            TaskInstance taskInstance = TaskInstanceFactory.CreateScenarioCompletedTaskInstance(_tenantIdentifier.Id, runInstance.RunId,
                runInstance.ScenarioId, autoBook.Id, AutoBookStatuses.Fatal_Error);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookHandleCompletedFatalError taskinstance To Be Executed " +
                            $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id})"));

            TaskResult taskResult = new ProcessTaskExecutor(HostingEnvironment.MapPath("/"), _repositoryFactory,
                TimeSpan.Zero, _auditEventRepository, _configuration).Execute(taskInstance);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookHandleCompletedFatalError taskinstance Executed " +
                            $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status})"));

            switch (taskResult.Status)
            {
                case TaskInstanceStatues.Starting:
                case TaskInstanceStatues.CompletedError:
                    var exceptionMessage = taskResult.Exception == null ? null : taskResult.Exception.Message;
                    throw new Exception($"Unable to start task to handle Fatal_Error from AutoBook " +
                                        $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status}, " +
                                        $"Exception={exceptionMessage})");
            }
        }

        /// <summary>
        /// Asynchronously starts AutoBook task. Will receive a PUT when Status changes.
        /// </summary>
        /// <param name="autoBook"></param>
        private void StartAutoBookRun(AutoBook autoBook)
        {
            TaskInstance taskInstance = TaskInstanceFactory.CreateStartNextScenarioTaskInstance(_tenantIdentifier.Id, autoBook.Id);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookRun taskinstance To Be Executed " +
                            $"(AutoBookID={autoBook.Id}, taskInstanceID={taskInstance.Id})"));

            TaskResult taskResult = ExecuteTask(taskInstance);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookRun taskinstance Executed " +
                            $"(AutoBookID={autoBook.Id}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status})"));

            switch (taskResult.Status)
            {
                case TaskInstanceStatues.Starting:
                case TaskInstanceStatues.CompletedError:
                    var exceptionMessage = taskResult.Exception == null ? null : taskResult.Exception.Message;
                    throw new Exception($"Unable to task to start run on Autobook " +
                                        $"(AutoBookID={autoBook.Id}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status}, " +
                                        $"Exception={exceptionMessage})");
            }
        }

        private TaskResult ExecuteTask(TaskInstance taskInstance) => new ProcessTaskExecutor(HostingEnvironment.MapPath("/"), _repositoryFactory,
                        TimeSpan.Zero, _auditEventRepository, _configuration).Execute(taskInstance);

        /// <summary>
        /// Asynchronously starts getting AutoBook results.
        /// </summary>
        /// <param name="autoBook"></param>
        /// <param name="runInstance"></param>
        private void StartAutoBookHandleCompletedSuccess(AutoBook autoBook, RunInstance runInstance)
        {
            TaskInstance taskInstance = TaskInstanceFactory.CreateScenarioCompletedTaskInstance(_tenantIdentifier.Id, runInstance.RunId,
                runInstance.ScenarioId, autoBook.Id, AutoBookStatuses.Task_Completed);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookHandleCompletedSuccess taskinstance To Be Executed " +
                           $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id})"));

            TaskResult taskResult = ExecuteTask(taskInstance);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookHandleCompletedSuccess taskinstance Executed " +
                           $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status})"));

            switch (taskResult.Status)
            {
                case TaskInstanceStatues.Starting:
                case TaskInstanceStatues.CompletedError:
                    var exceptionMessage = taskResult.Exception?.Message ?? String.Empty;
                    throw new Exception($"Unable to start task to handle Task_Completed from AutoBook " +
                                        $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status}, " +
                                        $"Exception={exceptionMessage})");
            }
        }

        private void StartAutoBookHandleCompletedTaskError(AutoBook autoBook, RunInstance runInstance)
        {
            TaskInstance taskInstance = TaskInstanceFactory.CreateScenarioCompletedTaskInstance(_tenantIdentifier.Id, runInstance.RunId,
                runInstance.ScenarioId, autoBook.Id, AutoBookStatuses.Task_Error);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookHandleCompletedTaskError taskinstance To Be Executed " +
                           $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id})"));

            TaskResult taskResult = ExecuteTask(taskInstance);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"StartAutoBookHandleCompletedTaskError taskinstance Executed " +
                           $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status})"));

            switch (taskResult.Status)
            {
                case TaskInstanceStatues.Starting:
                case TaskInstanceStatues.CompletedError:
                    var exceptionMessage = taskResult.Exception?.Message ?? String.Empty;
                    throw new Exception($"Unable to start task to handle Task_Error from AutoBook " +
                                        $"(AutoBookID={autoBook.Id}, RunID={runInstance.RunId}, ScenarioID={runInstance.ScenarioId}, taskInstanceID={taskInstance.Id}, taskResultStatus={taskResult.Status}, " +
                                        $"Exception={exceptionMessage})");
            }
        }

        /// <summary>
        /// Deletes AutoBook instance. AutoBook cannot be deleted if it is in use.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("AutoBooks")]
        public IHttpActionResult Delete(string id)
        {
            AutoBook autoBook = _autoBookRepository.Get(id);
            if (autoBook == null)    // AutoBook does not exist
            {
                return NotFound();
            }
            else
            {
                if (_runRepository.GetAllActive().Any())
                {
                    return this.Error().UnknownError("Cannot delete AutoBook because runs are active");
                }

                if (_autoBookSettings.Locked)
                {
                    return this.Error().UnknownError("Cannot delete AutoBook instance because provisioning functions are temporarily locked");
                }

                // Validate for deletion
                if (autoBook.Api != null)
                {
                    _autoBooks.ValidateForDelete(autoBook);
                }

                // Start delete
                _backgroundJobManager.StartJob<AutoBookDeleteBackgroundJob>(
                    new BackgroundJobParameter<AutoBook>(autoBook));
            }
            return Ok();
        }

        /// <summary>
        /// Returns AutoBook settings
        /// </summary>
        /// <returns></returns>
        [Route("Settings")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(AutoBookSettingsModel))]
        public IHttpActionResult GetAutoBookSettings()
        {
            var autoBookSettings = _autoBookSettingsRepository.Get();
            if (autoBookSettings == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AutoBookSettingsModel>(autoBookSettings));
        }

        /// <summary>
        /// Returns AutoBook instance configurations, only active configurations
        /// </summary>
        /// <returns></returns>
        [Route("InstanceConfigurations")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(List<AutoBookInstanceConfigurationModel>))]
        public IHttpActionResult GetAutoBookInstanceConfigurations()
        {
            var autoBookInstanceConfigurations = _autoBookInstanceConfigurationRepository.GetAll();

            if (!autoBookInstanceConfigurations.Any())    // Create defaults
            {
                autoBookInstanceConfigurations = _autoBooks.CreateDefaultInstanceConfigurations();
            }

            // Return instance configuration that have criteria, any with no criteria are temporarily disabled
            // until we've determined the resource limits
            var autoBookInstanceConfigurationModels = _mapper.Map<List<AutoBookInstanceConfigurationModel>>(autoBookInstanceConfigurations
                        .Where(ic => ic.CriteriaList != null &&
                               ic.CriteriaList.Any()));

            return Ok(autoBookInstanceConfigurationModels);
        }

        /// <summary>
        /// Updates AutoBook settings
        /// </summary>
        /// <returns></returns>
        [Route("Settings")]
        [AuthorizeRequest("AutoBooks")]
        public IHttpActionResult PutSettings([FromBody] AutoBookSettingsModel command)
        {
            var autoBookSettings = _autoBookSettingsRepository.Get();
            if (autoBookSettings == null)
            {
                return NotFound();
            }

            // Check that settings aren't locked
            if (autoBookSettings.Locked)
            {
                this.Error().UnknownError("Settings are currently locked");
            }

            autoBookSettings.Update(command.ProvisioningAPIURL,
                                    command.AutoProvisioning,
                                    command.AutoDistributed,
                                    command.MinLifetime,
                                    command.MaxLifetime,
                                    command.CreationTimeout,
                                    command.MinInstances,
                                    command.MaxInstances,
                                    command.ApplicationVersion,
                                    command.BinariesVersion);
            _autoBookSettingsRepository.AddOrUpdate(autoBookSettings);
            _autoBookSettingsRepository.SaveChanges();

            // Log changes
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"AutoBook provisioning has changed (Auto Provisioning={command.AutoProvisioning}, " +
                $"AutoBook distributed has changed (Auto Distributed={command.AutoDistributed}, " +
                $"Provisioning API URL={command.ProvisioningAPIURL}, Min Instances={command.MinInstances}, " +
                $"Max Instances={command.MaxInstances}, Min Lifetime={command.MinLifetime.ToTimeSpan().TotalSeconds} secs, " +
                $"Max Lifetime={command.MaxLifetime.ToTimeSpan().TotalSeconds} secs, " +
                $"Creation Timeout={command.CreationTimeout.ToTimeSpan().TotalSeconds} secs, " +
                $"Application Version={command.ApplicationVersion}, Binaries Version={command.BinariesVersion})"));
            return Ok();
        }

        /// <summary>
        /// Returns Logs from Autobook Provider for a specified date
        /// </summary>
        /// <param name="logDate">YYYY-MM-DD</param>
        /// <returns></returns>
        [Route("Provider/Logs/{logDate}")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(List<ProviderLog>))]
        public IHttpActionResult GetAutobookProviderLogs(string logDate)
        {
            if (!ValidateDateFormat(logDate))
            {
                return BadRequest("Invalid date format: please format date as YYYY-MM-DD");
            }
            List<ProviderLog> logs = _providerLogsAPI.GetLogs(logDate);
            if (logs is null)
            {
                return NotFound();
            }
            return Ok(logs);
        }

        private bool ValidateDateFormat(string logDate)
        {
            try
            {
                DateTime myDate = DateTime.ParseExact(logDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns list of AutoBookTaskReports
        /// </summary>
        /// <returns></returns>
        [Route("task-report")]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(List<AutoBookTaskReport>))]
        public IEnumerable<AutoBookTaskReport> GetAutoBookTaskReports()
        {
            var autoBookTaskReports = _autoBookTaskReportRepository.GetAll();

            return autoBookTaskReports;
        }

        /// <summary>
        /// Creates an AutoBookTaskReport
        /// </summary>
        [Route("task-report")]
        [AuthorizeRequest("AutoBooks")]
        public IHttpActionResult Post([FromBody] AutoBookTaskReportModel autoBookTaskReport)
        {
            if (!ModelState.IsValid || autoBookTaskReport == null)
            {
                return BadRequest(ModelState);
            }

            _autoBookTaskReportRepository.Add(Mappings.MapToAutoBookTaskReport(autoBookTaskReport, _mapper));
            _autoBookTaskReportRepository.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Get a AutoBookTaskReport by scenarioID
        /// </summary>
        /// <param name = "id" ></param >
        [Route("task-report/{id}")]
        [HttpGet]
        [AuthorizeRequest("AutoBooks")]
        [ResponseType(typeof(AutoBookTaskReport))]
        public IHttpActionResult GetByScenarioID(Guid id)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            AutoBookTaskReport autoBookTaskReport = _autoBookTaskReportRepository.GetByScenarioId(id);

            if (autoBookTaskReport == null)
            {
                return this.Error().ResourceNotFound($"No AutoBook task report found for scenario {id}");
            }

            return Ok(autoBookTaskReport);
        }
    }
}
