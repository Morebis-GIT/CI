using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using AutoMapper;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Queries;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using Microsoft.Extensions.Configuration;
using Raven.Abstractions.Extensions;
using Raven.Client.Linq;
using Swashbuckle.Swagger.Annotations;
using xggameplan.AuditEvents;
using xggameplan.Autopilot;
using xggameplan.common.BackgroundJobs;
using xggameplan.common.Services;
using xggameplan.common.Utilities;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.core.Services;
using xggameplan.core.Tasks;
using xggameplan.core.Tasks.Executors;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Jobs;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.Reports;
using xggameplan.Reports.DataAdapters;
using xggameplan.RunManagement;
using xggameplan.Services;
using xggameplan.Validations;
using xggameplan.Validations.Passes;
using xggameplan.Validations.Runs.Interfaces;
using xggameplan.Validations.Scenarios;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for managing runs
    /// </summary>
    [RoutePrefix("Runs")]
    public class RunsController : ApiController
    {
        //TODO: Consider moving start time of broadcast day to Tenant Settings
        private readonly TimeSpan _broadcastDayStartTime = TimeSpan.FromHours(6);

        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IConfiguration _configuration;
        private readonly IRunRepository _runRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAutopilotManager _autopilotManager;
        private readonly IIdentityGeneratorResolver _identityGeneratorResolver;
        private readonly IISRSettingsRepository _isrSettingsRepository;
        private readonly IRSSettingsRepository _rsSettingsRepository;
        private readonly ISmoothFailureRepository _smoothFailureRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly IRunManager _runManager;
        private readonly IMapper _mapper;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly IKPIComparisonConfigRepository _kpiComparisonConfigRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IPassRepository _passRepository;
        private readonly IEfficiencySettingsRepository _efficiencySettingsRepository;
        private readonly IFunctionalAreaRepository _functionalAreaRepository;
        private readonly IExcelReportGenerator _excelReportGenerator;
        private readonly IRunExcelReportDataAdapter _runReportDataAdapter;
        private readonly IModelDataValidator<AutopilotEngageModel> _autopilotEngageModelDataValidator;
        private readonly IISRGlobalSettingsRepository _isrGlobalSettingsRepository;
        private readonly IRSGlobalSettingsRepository _rsGlobalSettingsRepository;
        private readonly ISmoothFailureMessageRepository _smoothFailureMessageRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly TenantIdentifier _tenantIdentifier;
        private readonly ISynchronizationService _synchronizationService;
        private readonly ILandmarkRunService _landmarkRunService;
        private readonly IModelDataValidator<LandmarkRunTriggerModel> _landmarkRunTriggerModelValidator;
        private readonly IModelDataValidator<ScheduledRunSettingsModel> _scheduledRunSettingsModelValidator;
        private readonly IScenarioCampaignMetricRepository _scenarioCampaignMetricRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IRunsValidator _runsValidator;
        private readonly IAnalysisGroupRepository _analysisGroupRepository;
        private readonly IRunTypeRepository _runTypeRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IPassInspectorService _passInspectorService;
        private readonly IFacilityRepository _facilityRepository;

        private static readonly string[] acceptedPositionInProgrammeValues = new[] { "A", "C", "E" };

        public RunsController(IConfiguration configuration, IRunRepository runRepository, ITenantSettingsRepository tenantSettingsRepository,
            ISalesAreaRepository salesAreaRepository,
            ICampaignRepository campaignRepository, IISRSettingsRepository isrSettingsRepository,
            IRSSettingsRepository rsSettingsRepository,
            IRepositoryFactory repositoryFactory, IEfficiencySettingsRepository efficiencySettingsRepository,
            ISmoothFailureRepository smoothFailureRepository, IDemographicRepository demographicRepository,
            IAuditEventRepository auditEventRepository, IAutopilotManager autopilotManager,
            IIdentityGeneratorResolver identityGeneratorResolver, IRunManager runManager, IMapper mapper,
            IModelDataValidator<AutopilotEngageModel> autopilotEngageModelDataValidator,
            IKPIComparisonConfigRepository kpiComparisonConfigRepository,
            IScenarioResultRepository scenarioResultRepository, IScenarioRepository scenarioRepository,
            IPassRepository passRepository, IFunctionalAreaRepository functionalAreaRepository,
            IExcelReportGenerator excelReportGenerator, IRunExcelReportDataAdapter runReportDataAdapter,
            IISRGlobalSettingsRepository isrGlobalSettingsRepository,
            IRSGlobalSettingsRepository rsGlobalSettingsRepository,
            ISmoothFailureMessageRepository smoothFailureMessageRepository,
            IBackgroundJobManager backgroundJobManager,
            TenantIdentifier tenantIdentifier,
            ISynchronizationService synchronizationService,
            ILandmarkRunService landmarkRunService,
            IModelDataValidator<LandmarkRunTriggerModel> landmarkRunTriggerModelValidator,
            IModelDataValidator<ScheduledRunSettingsModel> scheduledRunSettingsModelValidator,
            IScenarioCampaignMetricRepository scenarioCampaignMetricRepository,
            IFeatureManager featureManager,
            IRunsValidator runsValidator,
            IAnalysisGroupRepository analysisGroupRepository,
            IRunTypeRepository runTypeRepository,
            IUsersRepository usersRepository,
            IPassInspectorService passInspectorService,
            IFacilityRepository facilityRepository)
        {
            _configuration = configuration;
            _runRepository = runRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _salesAreaRepository = salesAreaRepository;
            _campaignRepository = campaignRepository;
            _auditEventRepository = auditEventRepository;
            _autopilotManager = autopilotManager;
            _identityGeneratorResolver = identityGeneratorResolver;
            _isrSettingsRepository = isrSettingsRepository;
            _rsSettingsRepository = rsSettingsRepository;
            _smoothFailureRepository = smoothFailureRepository;
            _demographicRepository = demographicRepository;
            _repositoryFactory = repositoryFactory;
            _runManager = runManager;
            _mapper = mapper;
            _scenarioResultRepository = scenarioResultRepository;
            _kpiComparisonConfigRepository = kpiComparisonConfigRepository;
            _scenarioRepository = scenarioRepository;
            _passRepository = passRepository;
            _efficiencySettingsRepository = efficiencySettingsRepository;
            _functionalAreaRepository = functionalAreaRepository;
            _excelReportGenerator = excelReportGenerator;
            _runReportDataAdapter = runReportDataAdapter;
            _autopilotEngageModelDataValidator = autopilotEngageModelDataValidator;
            _isrGlobalSettingsRepository = isrGlobalSettingsRepository;
            _rsGlobalSettingsRepository = rsGlobalSettingsRepository;
            //SetAutoBookSettingsForTenantSettings();
            // Uncomment while testing static xml generation in local branch
            //_factory = cloudfactory;
            _smoothFailureMessageRepository = smoothFailureMessageRepository;
            _backgroundJobManager = backgroundJobManager;
            _tenantIdentifier = tenantIdentifier;
            _synchronizationService = synchronizationService;
            _landmarkRunService = landmarkRunService;
            _landmarkRunTriggerModelValidator = landmarkRunTriggerModelValidator;
            _scheduledRunSettingsModelValidator = scheduledRunSettingsModelValidator;
            _scenarioCampaignMetricRepository = scenarioCampaignMetricRepository;
            _featureManager = featureManager;
            _runsValidator = runsValidator;
            _analysisGroupRepository = analysisGroupRepository;
            _runTypeRepository = runTypeRepository;
            _usersRepository = usersRepository;
            _passInspectorService = passInspectorService;
            _facilityRepository = facilityRepository;
        }

        /// <summary>
        /// Returns RS settings for sales area
        /// </summary>
        /// <returns></returns>
        [Route("RSSettings")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RSSettingsModel))]
        public IHttpActionResult GetRSSettings([FromUri] string salesArea)
        {
            var rsSettings = _rsSettingsRepository.Find(salesArea);
            if (rsSettings == null)
            {
                return NotFound();
            }

            var rsSettingsModel = _mapper.Map<RSSettingsModel>(rsSettings);
            return Ok(rsSettingsModel);
        }

        /// <summary>
        /// Creates RS settings for sales area
        /// </summary>
        /// <returns></returns>
        [Route("RSSettings")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PostRSSettings([FromBody] RSSettingsModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            var rsSettingsExisting = _rsSettingsRepository.Find(command.SalesArea);
            if (rsSettingsExisting != null)
            {
                return this.Error().InvalidParameters("Right Sizer settings already exist");
            }

            // Validate
            var rsSettings = _mapper.Map<RSSettings>(command);
            ValidateForSave(rsSettings);

            // Save
            _rsSettingsRepository.Update(rsSettings);
            _rsSettingsRepository.SaveChanges();
            // Return model
            var rsSettingsModel = _mapper.Map<RSSettingsModel>(rsSettings);
            return Ok(rsSettingsModel);
        }

        /// <summary>
        /// Updates RS settings for one or more sales areas based on update mode
        /// </summary>
        /// <param name="command">
        /// Sales area input parameter then all settings (including
        /// demographics) are always updated
        /// </param>
        /// <param name="updateMode">
        /// <list>
        /// <item>0 = Current sales area only (include demographics)</item>
        /// <item>1 = All sales areas (include demographics)</item>
        /// <item>2 = All sales areas (exclude demographics)</item>
        /// <item>3 = All sales areas (Demographics only)</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        [Route("RSSettings")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PutRSSettings([FromBody] RSSettingsModel command, [FromUri] int updateMode = 0)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }
            if (Array.IndexOf(new int[] { 0, 1, 2, 3 }, updateMode) == -1)
            {
                return this.Error().InvalidParameters("Invalid update mode");
            }

            var rsSettings = _mapper.Map<RSSettings>(command);

            switch (updateMode)
            {
                case 0:     // Update this sales area only
                    var rsSettingsExisting = _rsSettingsRepository.Find(command.SalesArea);
                    if (rsSettingsExisting == null)
                    {
                        return NotFound();
                    }
                    rsSettingsExisting.UpdateFrom(rsSettings, 1);
                    ValidateForSave(rsSettingsExisting);
                    _rsSettingsRepository.Update(rsSettingsExisting);
                    break;

                case 1:     // Update all sales areas (incl demographics)
                case 2:     // Update all sales areas (excl demographics)
                case 3:     // Update all sales areas (Demographics only)
                    // Update & save
                    var rsSettingsList = _rsSettingsRepository.GetAll();
                    foreach (var rsSettingsCurrent in rsSettingsList)
                    {
                        if (rsSettingsCurrent.SalesArea == rsSettings.SalesArea)
                        {
                            rsSettingsCurrent.UpdateFrom(rsSettings, 1);      // Always update all settings
                        }
                        else
                        {
                            rsSettingsCurrent.UpdateFrom(rsSettings, updateMode);
                        }
                        ValidateForSave(rsSettingsCurrent);
                        _rsSettingsRepository.Update(rsSettingsCurrent);
                    }

                    // Add ISR settings for missing sales areas
                    var salesAreas = _salesAreaRepository.GetAll();
                    foreach (var salesArea in salesAreas.Where(sa => rsSettingsList.FindIndex(s => s.SalesArea == sa.Name) == -1))
                    {
                        RSSettings newRSSettings = new RSSettings() { SalesArea = salesArea.Name };
                        newRSSettings.UpdateFrom(rsSettings, updateMode);
                        ValidateForSave(newRSSettings);
                        _rsSettingsRepository.Add(new List<RSSettings>() { newRSSettings });
                    }
                    break;
            }

            _rsSettingsRepository.SaveChanges();
            // Return model
            var rsSettingsModel = _mapper.Map<RSSettingsModel>(rsSettings);
            return Ok(rsSettingsModel);
        }

        /// <summary>
        /// Compares RS settings for all sales areas based on compare mode:
        /// <BR/> 0=Full settings comparison (include demographics) <BR/> 1=Top
        /// level settings comparison (exclude demographics) <BR/> 2=Demographic
        /// settings only <BR/>
        /// </summary>
        /// <returns></returns>
        [Route("RSSettings/compare")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(ISRSettingsCompareModel))]
        public IHttpActionResult GetRSSettingsCompare([FromUri] int compareMode = 0)
        {
            if (Array.IndexOf(new int[] { 0, 1, 2 }, compareMode) == -1)
            {
                return this.Error().InvalidParameters("Invalid compare mode");
            }

            var rsSettingsCompareModel = new RSSettingsCompareModel() { IsIdenticalForAllSalesAreas = false };
            var rsSettingsList = _rsSettingsRepository.GetAll();
            if (rsSettingsList.Any())
            {
                rsSettingsCompareModel.IsIdenticalForAllSalesAreas = true;     // Until compared all settings
                var rsSettings = rsSettingsList[0];
                foreach (var rsSettingsCurrent in rsSettingsList.Where(i => i != rsSettingsList[0]))
                {
                    if (!rsSettings.IsSame(rsSettingsCurrent, compareMode))
                    {
                        rsSettingsCompareModel.IsIdenticalForAllSalesAreas = false;
                        break;
                    }
                }
            }
            return Ok(rsSettingsCompareModel);
        }

        /// <summary>
        /// Deletes RS settings for sales area
        /// </summary>
        /// <returns></returns>
        [Route("RSSettings")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult DeleteRSSettings([FromUri] string salesArea)
        {
            var rsSettings = _rsSettingsRepository.Find(salesArea);
            if (rsSettings == null)
            {
                return NotFound();
            }

            _rsSettingsRepository.Delete(rsSettings.SalesArea);

            return Ok();
        }

        /// <summary>
        /// Validates for saving RS settings
        /// </summary>
        /// <param name="rsSettings"></param>
        private void ValidateForSave(RSSettings rsSettings)
        {
            // Basic validation
            RSSettings.ValidateForSave(rsSettings);

            var demographics = _demographicRepository.GetAll().ToList();
            var salesArea = _salesAreaRepository.FindByNames(new List<string>() { rsSettings.SalesArea }).ToList().FirstOrDefault();
            if (salesArea == null)
            {
                throw new Exception(string.Format("Sales Area {0} is not valid", rsSettings.SalesArea));
            }

            if (rsSettings.DemographicsSettings != null)
            {
                var unknownDemographics = rsSettings.DemographicsSettings.Select(d => d.DemographicId).Where(sd => demographics.FindIndex(d => d.ExternalRef == sd) == -1).ToList();
                if (unknownDemographics.Any())
                {
                    throw new Exception(string.Format("Demographic {0} is not valid", unknownDemographics[0]));
                }
            }
        }

        /// <summary>
        /// Returns ISR settings for sales area
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(ISRSettingsModel))]
        public IHttpActionResult GetISRSettings([FromUri] string salesArea)
        {
            var isrSettings = _isrSettingsRepository.Find(salesArea);
            if (isrSettings == null)
            {
                return NotFound();
            }

            var isrSettingsModel = _mapper.Map<ISRSettingsModel>(isrSettings);
            return Ok(isrSettingsModel);
        }

        /// <summary>
        /// Creates ISR settings for sales area
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PostISRSettings([FromBody] ISRSettingsModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }

            var isrSettingsExisting = _isrSettingsRepository.Find(command.SalesArea);
            if (isrSettingsExisting != null)
            {
                return this.Error().InvalidParameters("ISR settings already exist");
            }

            // Validate
            var isrSettings = _mapper.Map<ISRSettings>(command);
            ValidateForSave(isrSettings);

            // Save
            _isrSettingsRepository.Add(new[] { isrSettings });
            _isrSettingsRepository.SaveChanges();

            // Return model
            var isrSettingsModel = _mapper.Map<ISRSettingsModel>(isrSettings);
            return Ok(isrSettingsModel);
        }

        /// <summary>
        /// Updates ISR settings for one or more sales areas based on update
        /// mode: <BR/> 0=Current sales area only (include demographics) <BR/>
        /// 1=All sales areas (include demographics) <BR/> 2=All sales areas
        /// (exclude demographics) <BR/> 3=All sales areas (Demographics only)
        /// <BR/><BR/> For sales area input parameter then all settings
        /// (including demographics) are always updated
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PutISRSettings([FromBody] ISRSettingsModel command, [FromUri] int updateMode = 0)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid parameters");
            }
            if (Array.IndexOf(new int[] { 0, 1, 2, 3 }, updateMode) == -1)
            {
                return this.Error().InvalidParameters("Invalid update mode");
            }

            var isrSettings = _mapper.Map<ISRSettings>(command);

            switch (updateMode)
            {
                case 0:     // Update this sales area only
                    var isrSettingsExisting = _isrSettingsRepository.Find(command.SalesArea);
                    if (isrSettingsExisting == null)
                    {
                        return NotFound();
                    }
                    isrSettingsExisting.UpdateFrom(isrSettings, 1);
                    ValidateForSave(isrSettingsExisting);
                    _isrSettingsRepository.Update(isrSettingsExisting);

                    break;

                case 1:     // Update all sales areas (incl demographics)
                case 2:     // Update all sales areas (excl demographics)
                case 3:     // Update all sales areas (Demographics only)
                    // Update & save
                    var isrSettingsList = _isrSettingsRepository.GetAll();
                    foreach (var isrSettingsCurrent in isrSettingsList)
                    {
                        if (isrSettingsCurrent.SalesArea == isrSettings.SalesArea)
                        {
                            isrSettingsCurrent.UpdateFrom(isrSettings, 1);      // Always update all settings
                        }
                        else
                        {
                            isrSettingsCurrent.UpdateFrom(isrSettings, updateMode);
                        }
                        ValidateForSave(isrSettingsCurrent);
                        _isrSettingsRepository.Update(isrSettingsCurrent);
                    }

                    // Add ISR settings for missing sales areas
                    var salesAreas = _salesAreaRepository.GetAll();
                    foreach (var salesArea in salesAreas.Where(sa => isrSettingsList.FindIndex(s => s.SalesArea == sa.Name) == -1))
                    {
                        ISRSettings newISRSettings = new ISRSettings() { SalesArea = salesArea.Name };
                        newISRSettings.UpdateFrom(isrSettings, updateMode);
                        ValidateForSave(newISRSettings);
                        _isrSettingsRepository.Add(new List<ISRSettings>() { newISRSettings });
                    }

                    break;
            }

            _isrSettingsRepository.SaveChanges();

            // Return model
            var isrSettingsModel = _mapper.Map<ISRSettingsModel>(isrSettings);
            return Ok(isrSettingsModel);
        }

        /// <summary>
        /// Compares ISR settings for all sales areas based on compare mode:
        /// <BR/> 0=Full settings comparison (include demographics) <BR/> 1=Top
        /// level settings comparison (exclude demographics) <BR/> 2=Demographic
        /// settings only <BR/>
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings/compare")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(ISRSettingsCompareModel))]
        public IHttpActionResult GetISRSettingsCompare([FromUri] int compareMode = 0)
        {
            if (Array.IndexOf(new int[] { 0, 1, 2 }, compareMode) == -1)
            {
                return this.Error().InvalidParameters("Invalid compare mode");
            }

            var isrSettingsCompareModel = new ISRSettingsCompareModel() { IsIdenticalForAllSalesAreas = false };
            var isrSettingsList = _isrSettingsRepository.GetAll();
            if (isrSettingsList.Any())
            {
                isrSettingsCompareModel.IsIdenticalForAllSalesAreas = true;     // Until compared all settings
                var isrSettings = isrSettingsList[0];
                foreach (var isrSettingsCurrent in isrSettingsList.Where(i => i != isrSettingsList[0]))
                {
                    if (!isrSettings.IsSame(isrSettingsCurrent, compareMode))
                    {
                        isrSettingsCompareModel.IsIdenticalForAllSalesAreas = false;
                        break;
                    }
                }
            }
            return Ok(isrSettingsCompareModel);
        }

        /// <summary>
        /// Deletes ISR settings for sales area
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult DeleteISRSettings([FromUri] string salesArea)
        {
            var isrSettings = _isrSettingsRepository.Find(salesArea);
            if (isrSettings == null)
            {
                return NotFound();
            }

            _isrSettingsRepository.Delete(isrSettings.SalesArea);
            _isrSettingsRepository.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns ISR global settings
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings/global")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(ISRGlobalSettingsModel))]
        public IHttpActionResult GetISRGlobalSettings()
        {
            var isrGlobalSettings = _isrGlobalSettingsRepository.Get();
            var isrGlobalSettingsModel = _mapper.Map<ISRGlobalSettingsModel>(isrGlobalSettings);

            return Ok(isrGlobalSettingsModel);
        }

        /// <summary>
        /// Updates ISR global settings
        /// </summary>
        /// <returns></returns>
        [Route("ISRSettings/global")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(ISRGlobalSettingsModel))]
        public IHttpActionResult PutISRGlobalSettings([FromBody] ISRGlobalSettingsModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return this.Error().InvalidParameters("Passed model is invalid.");
            }

            var payload = _mapper.Map<ISRGlobalSettings>(model);
            var settings = _isrGlobalSettingsRepository.Update(payload);

            return Ok(_mapper.Map<ISRGlobalSettingsModel>(settings));
        }

        /// <summary>
        /// Returns RS global settings
        /// </summary>
        /// <returns></returns>
        [Route("RSSettings/global")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RSGlobalSettingsModel))]
        public IHttpActionResult GetRSGlobalSettings()
        {
            var rsGlobalSettings = _rsGlobalSettingsRepository.Get();
            var rsGlobalSettingsModel = _mapper.Map<RSGlobalSettingsModel>(rsGlobalSettings);

            return Ok(rsGlobalSettingsModel);
        }

        /// <summary>
        /// Updates RS global settings
        /// </summary>
        /// <returns></returns>
        [Route("RSSettings/global")]
        [AuthorizeRequest("Runs")]
        [HttpPut]
        [ResponseType(typeof(ISRGlobalSettingsModel))]
        public IHttpActionResult PutRSGlobalSettings([FromBody] RSGlobalSettingsModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return this.Error().InvalidParameters("Passed model is invalid.");
            }

            var payload = _mapper.Map<RSGlobalSettings>(model);
            var settings = _rsGlobalSettingsRepository.Update(payload);

            return Ok(_mapper.Map<RSGlobalSettingsModel>(settings));
        }

        private bool CheckTimesWithinRange(DateTime innerRangeStartTime, DateTime innerRangeEndTime, DateTime outerRangeStartTime, DateTime outerRangeEndTime)
        {
            var isStartTimeWithinRun = innerRangeStartTime >= outerRangeStartTime && innerRangeStartTime <= outerRangeEndTime;
            var isEndTimeWithinRun = innerRangeEndTime >= outerRangeStartTime && innerRangeEndTime <= outerRangeEndTime;

            return isStartTimeWithinRun && isEndTimeWithinRun;
        }

        /// <summary>
        /// Validates for saving ISR settings
        /// </summary>
        /// <param name="isrSettings"></param>
        private void ValidateForSave(ISRSettings isrSettings)
        {
            // Basic validation
            ISRSettings.ValidateForSave(isrSettings);

            var demographics = _demographicRepository.GetAll().ToList();
            var salesArea = _salesAreaRepository.FindByNames(new List<string>() { isrSettings.SalesArea }).ToList().FirstOrDefault();
            if (salesArea == null)
            {
                throw new Exception(string.Format("Sales Area {0} is not valid", isrSettings.SalesArea));
            }

            // Check demographic settings
            if (isrSettings.DemographicsSettings != null)
            {
                var unknownDemographics = isrSettings.DemographicsSettings.Select(d => d.DemographicId).Where(sd => demographics.FindIndex(d => d.ExternalRef == sd) == -1).ToList();
                if (unknownDemographics.Any())
                {
                    throw new Exception(string.Format("Demographic {0} is not valid", unknownDemographics[0]));
                }
            }
        }

        /// <summary>
        /// Returns all runs
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Runs")]
        public IEnumerable<RunModel> Get([FromUri] string orderBy = "")
        {
            var result = new List<RunModel>();

            var runs = _runRepository.GetAll();
            if (runs != null && runs.Any())
            {
                orderBy = !String.IsNullOrWhiteSpace(orderBy) ? orderBy.ToLower() : string.Empty;
                switch (orderBy)
                {
                    case "executestarteddatetime":    // If all pending scenarios then order by Run.CreatedDateTime else Run.ExecuteStartedDateTime
                        {
                            runs = runs.OrderByDescending(x => x.Scenarios.Where(s => s.Status != ScenarioStatuses.Pending).Any() ?
                                                               x.ExecuteStartedDateTime.GetValueOrDefault(x.CreatedDateTime) :
                                                               x.CreatedDateTime).ThenByDescending(y => y.Description);
                            break;
                        }
                    default:
                        {
                            runs = runs.OrderByDescending(x => x.CreatedDateTime).ThenByDescending(y => y.Description);
                            break;
                        }
                }

                result = Mappings.MapToRunModels(runs, _scenarioRepository, _passRepository, _analysisGroupRepository, _tenantSettingsRepository, _mapper);
            }

            return result;
        }

        /// <summary>
        /// Creates input files in memory. FOR DEVELOPMENT TESTING ONLY
        /// </summary>
        /// <returns></returns>
        [Route("{id}/devtests/createinputfiles")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(List<AutoBookModel>))]
        public IHttpActionResult GetCreateInputFile([FromUri] Guid id)
        {
            var run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }

            _runManager.CreateInputFilesForTest(run);
            return Ok();
        }

        /// <summary>
        /// Processes spots output file. This is done for the first scenario
        /// only. Before using then you must copy the output files to the folder
        /// /Test/Scenario.{ScenarioId}. FOR DEVELOPMENT TESTING ONLY
        /// </summary>
        /// <returns></returns>
        [Route("{id}/devtests/processspotsoutputfile")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(List<AutoBookModel>))]
        public IHttpActionResult GetProcessSpotsOutputFile([FromUri] Guid id)
        {
            var run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }

            // Set input file path
            string testFileFolder = string.Format(@"{0}\Scenario.{1}\{2}", System.Web.Hosting.HostingEnvironment.MapPath("/Test"), run.Scenarios[0].Id);

            // Process files
            _runManager.ProcessSpotsOutputFileForTest(run, testFileFolder);

            return Ok();
        }

        /// <summary>
        /// Returns runs matching filter criteria
        /// </summary>
        private IEnumerable<Run> GetRuns(List<string> statuses,
                                    string fromDate = "",
                                    string toDate = "",
                                    string description = "",
                                    string sortOrder = "",
                                    int pageNumber = 0,
                                    int pageItems = 0)
        {
            const string dateFormat = "yyyy-MM-dd";

            // Validate parameters
            if (!String.IsNullOrEmpty(fromDate) && !DateHelper.CanConvertToDate(fromDate, dateFormat))
            {
                this.Error().InvalidParameters("Invalid FromDate");
            }
            if (!String.IsNullOrEmpty(toDate) && !DateHelper.CanConvertToDate(toDate, dateFormat))
            {
                this.Error().InvalidParameters("Invalid ToDate");
            }
            if (pageNumber < 0 || pageItems < 0)
            {
                this.Error().InvalidParameters("Invalid paging parameters");
            }
            else if (pageNumber > 0 && pageItems == 0)
            {
                this.Error().InvalidParameters("Number of items per page must be indicated when page number is specified");
            }

            // Parse input parameters
            DateTime fromDateValue = String.IsNullOrEmpty(fromDate) ? DateTime.UtcNow.AddYears(-100) : DateHelper.GetDate(fromDate, dateFormat, DateTimeKind.Utc);
            DateTime toDateValue = String.IsNullOrEmpty(toDate) ? DateTime.UtcNow.AddYears(100) : DateHelper.GetDate(toDate, dateFormat, DateTimeKind.Utc);

            List<ScenarioStatuses> scenarioStatuses = new List<ScenarioStatuses>();
            if (statuses != null)
            {
                foreach (string status in statuses.Where(x => !String.IsNullOrWhiteSpace(x)))
                {
                    if (EnumUtilities.CanConvertToEnum(typeof(ScenarioStatuses), status.Trim()))
                    {
                        scenarioStatuses.Add((ScenarioStatuses)Enum.Parse(typeof(ScenarioStatuses), status.Trim()));
                    }
                    else
                    {
                        this.Error().InvalidParameters(string.Format("Status {0} is invalid", status));
                    }
                }
            }

            // Get runs
            var runs = _runRepository.GetAll().Where(run =>
                    (   // Run date within range
                        (fromDateValue.Date >= run.StartDate.Date && fromDateValue.Date <= run.EndDate.Date) ||       // FromDate inside run range
                        (toDateValue.Date >= run.StartDate.Date && toDateValue.Date <= run.EndDate.Date) ||           // ToDate inside run range
                        (fromDateValue.Date < run.StartDate.Date && toDateValue.Date > run.EndDate.Date)              // Run range inside of FromDate-ToDate
                    ) &&
                    (   // Description contains text
                        String.IsNullOrEmpty(description) || run.Description.ToLower().Contains(description.ToLower())
                    ) &&
                    (   // Scenario filter
                        run.Scenarios.Where(x => Array.IndexOf(scenarioStatuses.ToArray(), x.Status) != -1).Any() || scenarioStatuses.Count == 0
                    ));

            // Sort
            sortOrder = String.IsNullOrEmpty(sortOrder) ? sortOrder : sortOrder.ToLower();
            if (String.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "createddatetime";
            }
            switch (sortOrder)
            {
                case "createddatetime": runs = runs.OrderBy(run => run.CreatedDateTime).ThenBy(x => x.Description); break;
                case "lastmodifieddatetime": runs = runs.OrderBy(run => run.LastModifiedDateTime).ThenBy(x => x.Description); break;
                case "executestarteddatetime": runs = runs.OrderByDescending(x => x.Scenarios.Where(s => s.Status != ScenarioStatuses.Pending).Any() ? x.ExecuteStartedDateTime : x.CreatedDateTime).ThenByDescending(y => y.Description); break;
                case "startdatetime": runs = runs.OrderBy(run => run.StartDate).ThenBy(x => x.Description); break;
                case "enddatetime": runs = runs.OrderBy(run => run.EndDate).ThenBy(x => x.Description); break;
                case "description": runs = runs.OrderBy(run => run.Description); break;
                default: this.Error().InvalidParameters("Invalid sort order"); break;
            }

            // Filter by page
            if (pageNumber > 0)
            {
                runs = xggameplan.common.Utilities.ListUtilities.GetPageItems<Run>(runs.ToList(), pageItems, pageNumber - 1);    // First page for GetPageItems is zero
            }
            return runs;
        }

        /// <summary>
        /// Returns summary for all runs matching filter criteria
        /// </summary>
        [Route("search/summary")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(FilterRunsSummaryModel))]
        public IHttpActionResult GetSearchFilterRunsSummary([FromUri] List<string> statuses = null,
                                         [FromUri] string fromDate = "",
                                         [FromUri] string toDate = "",
                                         [FromUri] string description = "",
                                         [FromUri] string sortOrder = "",
                                         [FromUri] int pageNumber = 0,
                                         [FromUri] int pageItems = 0)
        {
            // Get runs
            var runs = GetRuns(statuses, fromDate, toDate, description, sortOrder, pageNumber, pageItems);

            // Return summary
            return Ok(GetFilterRunSummaryModel(runs));
        }

        /// <summary>
        /// Returns filter run summary
        /// </summary>
        /// <param name="runs"></param>
        /// <returns></returns>
        private FilterRunsSummaryModel GetFilterRunSummaryModel(IEnumerable<Run> runs)
        {
            FilterRunsSummaryModel filterRunSummary = new FilterRunsSummaryModel();
            foreach (var run in runs)
            {
                // Add author
                if (filterRunSummary.Authors.FindIndex(x => x.Id == run.Author.Id) == -1)
                {
                    filterRunSummary.Authors.Add(new FilterRunsSummaryItemModel<int>() { Id = run.Author.Id, Name = run.Author.Name });
                }

                // Add scenario statuses
                foreach (var scenario in run.Scenarios)
                {
                    if (filterRunSummary.ScenarioStatuses.FindIndex(x => x.Id == scenario.Status) == -1)
                    {
                        filterRunSummary.ScenarioStatuses.Add(new FilterRunsSummaryItemModel<ScenarioStatuses>() { Id = scenario.Status, Name = scenario.Status.ToString() });        // TODO: Support scenario status translation
                    }
                }
            }
            filterRunSummary.Authors.Sort((x, y) => string.Compare(x.Name, y.Name));
            filterRunSummary.ScenarioStatuses.Sort((x, y) => string.Compare(x.Name, y.Name));
            return filterRunSummary;
        }

        /// <summary>
        /// Returns Smooth failures for the run
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/smooth/failures")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(List<SmoothFailureModel>))]
        public IHttpActionResult GetSmoothFailures(Guid id)
        {
            var item = _runRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            var smoothFailures = _smoothFailureRepository.GetByRunId(id);
            var smoothFailuresModel = GetSmoothFailureModels(smoothFailures);
            return Ok(smoothFailuresModel);
        }

        /// <summary>
        /// Export Smooth failures for the run
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/smooth/exportFailures")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(List<SmoothFailureModel>))]
        public IHttpActionResult ExportSmoothFailures(Guid id)
        {
            var run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }
            var smoothFailures = _smoothFailureRepository.GetByRunId(id);
            var smoothFailuresModel = GetSmoothFailureModels(smoothFailures);
            var failureMessages = _smoothFailureMessageRepository.GetAll();
            var reportModel = _runReportDataAdapter.Map(run, smoothFailuresModel, failureMessages, DateTime.UtcNow, _mapper);
            var content = _excelReportGenerator.GetSmoothFailuresExcelReport(reportModel);
            return BinaryResponse(content);
        }

        private List<SmoothFailureModel> GetSmoothFailureModels(IEnumerable<SmoothFailure> smoothFailures)
        {
            // Index sales areas by name
            var salesAreasByName = SalesArea.IndexListByName(_salesAreaRepository.GetAll());

            // Map SmoothFailures to SmoothFailureModels
            var smoothFailuresModel = _mapper.Map<List<SmoothFailureModel>>(smoothFailures);
            smoothFailuresModel.ForEach(sfm => sfm.SalesAreaShortName = salesAreasByName.ContainsKey(sfm.SalesArea) ? salesAreasByName[sfm.SalesArea].ShortName : sfm.SalesAreaShortName);
            return smoothFailuresModel;
        }

        /// <summary>
        /// Returns run by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult Get(Guid id)
        {
            var run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }

            List<CampaignWithProductFlatModel> allCampaigns = null;
            if (run.RunStatus == RunStatus.NotStarted)
            {
                //get all the campaigns with product info. this will be used to add CampaignPassPriorities for new Campaigns to all Scenarios in the Run
                var campaignsResult = _campaignRepository.GetWithProduct(null);
                allCampaigns = campaignsResult.Items != null && campaignsResult.Items.Any() ? campaignsResult.Items.ToList() : null;
            }

            var runModel = Mappings.MapToRunModel(run, _scenarioRepository, _passRepository, _tenantSettingsRepository,
                _mapper, _analysisGroupRepository, _functionalAreaRepository, _scenarioCampaignMetricRepository, allCampaigns);

            runModel.CampaignsProcessesSettings.ForEach(cps =>
            {
                runModel.Scenarios.ForEach(s => s.CampaignPassPriorities.ForEach(c =>
                {
                    if (c.Campaign != null && c.Campaign.ExternalId == cps.ExternalId)
                    {
                        if (cps.InefficientSpotRemoval.HasValue)
                        {
                            c.Campaign.InefficientSpotRemoval = cps.InefficientSpotRemoval.Value;
                        }
                        if (cps.IncludeRightSizer.HasValue)
                        {
                            c.Campaign.IncludeRightSizer = cps.IncludeRightSizer.Value;
                        }
                    }
                }));
            });

            return Ok(runModel);
        }

        /// <summary>
        /// Returns basic run data by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{runId}/get-basic-data")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult GetBasicRunData(Guid runId)
        {
            var run = _runRepository.Find(runId);
            if (run == null)
            {
                return NotFound();
            }
            var runModel = Mappings.MapToRunModel(run, _scenarioRepository, _mapper);
            return Ok(runModel);
        }

        /// <summary>
        /// Returns run notification list by run Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RunNotifications")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(List<RunNotification>))]
        public IHttpActionResult GetRunNotifications([FromBody] IEnumerable<Guid> ids)
        {
            IEnumerable<RunNotification> runNotificationList;
            var runList = new List<Run>();

            foreach (var id in ids)
            {
                var run = _runRepository.Find(id);
                if (run != null)
                {
                    if (!runList.Exists(e => e.Id == run.Id))
                    {
                        runList.Add(run);
                    }
                }
            }

            if (runList is null || !runList.Any())
            {
                runNotificationList = new List<RunNotification>();
            }
            else
            {
                runNotificationList = Mappings.MapToRunNotification(runList);
            }

            return Ok(runNotificationList
                .OrderBy(x => x.endDate).ThenBy(x => x.endTime));
        }

        /// <summary>
        /// returns an Excel file binaries as HttpResponseMessage containing the
        /// Passess and Scenario details of th egiven Run id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/ExportScenarioPasses")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult ExportScenarioPasses([FromUri] Guid id)
        {
            var run = _runRepository.Find(id);
            if (run == null)
            {
                return new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            var demographics = _demographicRepository.GetAll();
            List<CampaignWithProductFlatModel> allCampaigns = null;
            if (run.RunStatus == RunStatus.NotStarted)
            {
                //get all the campaigns with product info. this will be used to add CampaignPassPriorities for new Campaigns to all Scenarios in the Run
                var campaignsResult = _campaignRepository.GetWithProduct(null);
                allCampaigns = campaignsResult.Items != null && campaignsResult.Items.Any() ? campaignsResult.Items.ToList() : null;
            }

            var runModel = Mappings.MapToRunModel(run, _scenarioRepository, _passRepository, _tenantSettingsRepository,
                _mapper, _analysisGroupRepository, _functionalAreaRepository, null, allCampaigns);
            var excelReportRunModel = _runReportDataAdapter.Map(runModel, demographics, DateTime.Now);
            var content = _excelReportGenerator.GetRunExcelReport(excelReportRunModel);
            return BinaryResponse(content);
        }

        /// <summary>
        /// Calls Webhook to request an Inventory Lock for the Run
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/Inventory/Lock")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PostInventoryLock([FromUri] Guid id)
        {
            Run run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }

            if (run.InventoryLock != null && run.InventoryLock.Locked)
            {
                return BadRequest("Inventory already locked against this run");
            }

            try
            {
                if (_runManager.InventoryLock(run))
                {
                    run.InventoryLock = new InventoryLock { Locked = true, ChosenScenarioId = null };
                    _runRepository.Update(run);
                    _runRepository.SaveChanges();
                    return Ok();
                }
                return BadRequest("Failed to lock the Inventory against this run");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message != null)
                {
                    return BadRequest(ex.InnerException.Message);
                }

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Calls Webhook to request an Inventory Unlock for the Run
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}/Inventory/Unlock")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PostInventoryUnlock([FromUri] Guid id, [FromBody] InventoryUnlockRequestModel model)
        {
            Run run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }

            if (run.InventoryLock != null && !run.InventoryLock.Locked)
            {
                return BadRequest("Inventory already unlocked against this run");
            }

            try
            {
                if (_runManager.InventoryUnlock(run, model.ScenarioId))
                {
                    run.InventoryLock = new InventoryLock { Locked = false, ChosenScenarioId = model.ScenarioId };
                    _runRepository.Update(run);
                    _runRepository.SaveChanges();
                    return Ok();
                }
                return BadRequest("Failed to unlock the Inventory against this run");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message != null)
                {
                    return BadRequest(ex.InnerException.Message);
                }

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a run based on the supplied createRunModel
        /// </summary>
        /// <param name="createRunModel">The Create Run Model <see cref="CreateRunModel"/></param>
        /// <returns>The Created Run <see cref="RunModel"/> if successfull.</returns>
        /// <response code="200">
        /// Returns the Created Run <see cref="RunModel"/> with 200 OK Response
        /// </response>
        /// <response code="400">
        /// Returns 400 Bad Request, when the supplied createRunModel is
        /// invalid/fails validation
        /// </response>
        /// <response code="500">
        /// Returns 500 internal server erros for any failures
        /// </response>
        [Route("")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult Post([FromBody] CreateRunModel createRunModel)
        {
            if (createRunModel == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get all the campaigns with product info. This will be used to
            // validate CampaignPassPriorities and to add defaults
            var campaignsResult = _campaignRepository.GetWithProduct(null);
            var allCampaigns = campaignsResult.Items != null && campaignsResult.Items.Any() ? campaignsResult.Items.ToList() : null;
            var allSalesAreas = _salesAreaRepository.GetAll().ToList();

            // Get default scenrio if AddDefaultScenario is set. This will be
            // used for validation, creating default scenario and default
            // Campaign Pass Priorities
            Scenario defaultScenario = null;
            if (createRunModel.AddDefaultScenario)
            {
                defaultScenario = GetDefaultScenario();
            }

            //validate the createRunModel and send BadRequest if any validation fails
            var validationErrorMsg = ValidateForCreate(createRunModel, allCampaigns, allSalesAreas, defaultScenario);
            if (!string.IsNullOrWhiteSpace(validationErrorMsg))
            {
                return BadRequest(validationErrorMsg);
            }

            // Convert CreateRunModel To Run by processing scenarios, passes,
            // campaign pass priorities and sales area priorities
            Run run;
            try
            {
                run = ProcessCreateRunModelAndConvertToRun(createRunModel, allCampaigns, allSalesAreas, defaultScenario);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }

            var allScenarios = new List<Scenario>();
            var updatedScenarios = new List<Scenario>();
            var newScenarios = new List<Scenario>();
            var allPassesByScenario = new List<List<Pass>>();
            var updatedPasses = new List<Pass>();
            var newPasses = new List<Pass>();

            // Get changes for Scenarios & Pass repositories
            GetChangesForScenariosAndPasses(run.Scenarios, createRunModel.Scenarios, allScenarios, updatedScenarios, newScenarios,
                                            allPassesByScenario, updatedPasses, newPasses, allCampaigns, defaultScenario);

            // Validate the Run before saving
            _runsValidator.ValidateForSave(run, allScenarios, allPassesByScenario, allSalesAreas);

            if (newPasses != null && newPasses.Any())
            {
                _passRepository.Add(newPasses);
            }
            if (updatedPasses != null && updatedPasses.Any())
            {
                _passRepository.Update(updatedPasses);
            }
            _passRepository.SaveChanges();

            // Bulk Add/Update scenarios and save to db
            if (newScenarios != null && newScenarios.Any())
            {
                _scenarioRepository.Add(newScenarios);
            }
            if (updatedScenarios != null && updatedScenarios.Any())
            {
                _scenarioRepository.Update(updatedScenarios);
            }
            _scenarioRepository.SaveChanges();

            

            // apply Run processes settings for Campaign Pass Priorities in all Scenarios
            if (run.CampaignsProcessesSettings != null && run.CampaignsProcessesSettings.Any())
            {
                ApplyRunProcessesSettingsForCampaignPassPriorities(allScenarios, run.CampaignsProcessesSettings);
            }

            _runRepository.Add(run);
            _runRepository.SaveChanges();

           

            // Log event
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunCreated(0, 0, run.Id, null));

            newPasses.AddRange(updatedPasses);
            newScenarios.AddRange(updatedScenarios);
            var runModel = Mappings.MapToRunModel(run, _scenarioRepository, _passRepository, _tenantSettingsRepository,
                _mapper, _analysisGroupRepository, null, null, null, newPasses, newScenarios);

            return Ok(runModel);
        }

        /// <summary>
        /// Create a list of Sales Areas Priorities with default priority of 3
        /// </summary>
        /// <returns></returns>
        private static List<SalesAreaPriorityModel> CreateDefaultListOfSalesAreasPriorities(IReadOnlyCollection<SalesArea> allSalesAreas)
        {
            var salesAreaPriorities = new List<SalesAreaPriorityModel>();
            if (allSalesAreas != null && allSalesAreas.Any())
            {
                salesAreaPriorities.AddRange(allSalesAreas.Select(s => new SalesAreaPriorityModel
                {
                    SalesArea = s.Name,
                    Priority = SalesAreaPriorityType.Priority3
                }));
            }

            return salesAreaPriorities;
        }

        /// <summary>
        /// Gets changes for Scenario and Pass repositories
        /// </summary>
        /// <param name="runScenario"></param>
        /// <param name="createScenarioModel"></param>
        /// <param name="allScenarios"></param>
        /// <param name="updatedScenarios"></param>
        /// <param name="newScenarios"></param>
        /// <param name="allPassesByScenario"></param>
        /// <param name="updatedPasses"></param>
        /// <param name="newPasses"></param>
        private void GetChangesForScenarioAndPassRepositories(RunScenario runScenario, CreateScenarioModel createScenarioModel,
                                                    List<Scenario> allScenarios, List<Scenario> updatedScenarios, List<Scenario> newScenarios,
                                                    List<List<Pass>> allPassesByScenario, List<Pass> updatedPasses, List<Pass> newPasses)
        {
            // Check if scenario exists
            Scenario scenario = runScenario.Id == Guid.Empty ? null : _scenarioRepository.Get(runScenario.Id);

            if (scenario == null)    // New scenario, add to Scenario repository
            {
                scenario = _mapper.Map<Scenario>(createScenarioModel);
                if (scenario != null)
                {
                    scenario.Id = runScenario.Id;
                    IdUpdater.SetIds(scenario, _identityGeneratorResolver);     // Set Scenario.CustomId, Pass.Id if not set
                    runScenario.Id = scenario.Id;                               // If scenario has been given an id, it should be provided runScenario respectively
                    newScenarios.Add(scenario);
                }
            }
            else     // Existing scenario, update it
            {
                Mappings.ApplyToScenario(scenario, createScenarioModel);
                scenario.Id = runScenario.Id;
                IdUpdater.SetIds(scenario, _identityGeneratorResolver);     // Set Scenario.CustomId, Pass.Id if not set
                updatedScenarios.Add(scenario);
            }
            allScenarios.Add(scenario);

            // Check passes
            List<Pass> allPasses = new List<Pass>();
            for (int passIndex = 0; passIndex < createScenarioModel.Passes.Count; passIndex++)
            {
                PassReference passReference = scenario.Passes[passIndex];
                PassModel passModel = createScenarioModel.Passes[passIndex];
                Pass pass = _passRepository.Get(passReference.Id);

                if (pass == null)    // New pass, add to repository
                {
                    pass = _mapper.Map<Pass>(passModel);
                    if (pass != null)
                    {
                        pass.Id = passReference.Id;     // Set above in call to IdUpdater.SetIds(scenario)
                        newPasses.Add(pass);
                    }
                }
                else    // Existing pass, update it
                {
                    Mappings.ApplyToPass(pass, passModel, _mapper);
                    updatedPasses.Add(pass);
                }
                allPasses.Add(pass);
            }
            allPassesByScenario.Add(allPasses);
        }

        /// <summary>
        /// Update Scenario and Pass repositories for newly created scenario
        /// </summary>
        /// <param name="run"></param>
        /// <param name="updatedScenarios"></param>
        /// <param name="newScenarios"></param>
        /// <param name="updatedPasses"></param>
        /// <param name="newPasses"></param>
        /// <param name="deletedScenarioIds"></param>
        /// <param name="deletedPassIds"></param>
        private void UpdateScenarioAndPassRepositories(Run run,
                                                List<Scenario> updatedScenarios, List<Scenario> newScenarios,
                                                List<Pass> updatedPasses, List<Pass> newPasses,
                                                List<Guid> deletedScenarioIds, List<int> deletedPassIds)

        {
            // Add/update repository
            if (updatedScenarios != null && updatedScenarios.Any())
            {
                updatedScenarios.ForEach(currentScenario => _scenarioRepository.Update(currentScenario));
            }

            if (newScenarios != null && newScenarios.Any())
            {
                newScenarios.ForEach(currentScenario => _scenarioRepository.Add(currentScenario));
            }

            if (updatedPasses != null && updatedPasses.Any())
            {
                updatedPasses.ForEach(currentPass => _passRepository.Update(currentPass));
            }

            if (newPasses != null && newPasses.Any())
            {
                newPasses.ForEach(currentPass => _passRepository.Add(currentPass));
            }

            // Remove deleted scenarios/passes
            if (deletedScenarioIds != null && deletedScenarioIds.Any())
            {
                _scenarioRepository.Remove(deletedScenarioIds);
            }
            if (deletedPassIds != null && deletedPassIds.Any())
            {
                _passRepository.Remove(deletedPassIds);
            }
        }

        /// <summary>
        /// Creates a scenario using passed details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{id}/scenarios")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult Post([FromUri] Guid id,
                                      [FromBody] CreateScenarioModel command)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid scenario parameters");
            }

            var run = _runRepository.Find(id);
            if (run == null)
            {
                return NotFound();
            }

            // Pre-process (validate and trim) dates of scenario's passes
            foreach (var pass in command.Passes)
            {
                if (pass.PassSalesAreaPriorities is null)
                {
                    continue;
                }

                if (!pass.PassSalesAreaPriorities.AreDatesRetained)
                {
                    continue;
                }

                var isPassOutsideRun = pass.PassSalesAreaPriorities.EndDate < run.StartDate
                                       || pass.PassSalesAreaPriorities.StartDate > run.EndDate;

                if (isPassOutsideRun)
                {
                    return BadRequest($"Pass: {pass.Name}, SalesAreaPriorities has StartDate/EndDate completely outside run StartDate/EndDate");
                }

                if (pass.PassSalesAreaPriorities.StartDate < run.StartDate)
                {
                    pass.PassSalesAreaPriorities.StartDate = run.StartDate;
                }

                if (pass.PassSalesAreaPriorities.EndDate > run.EndDate)
                {
                    pass.PassSalesAreaPriorities.EndDate = run.EndDate;
                }
            }

            // If received scenario is from library,
            // prepare it with it's passes to be cloned and put into the run
            if (command.IsLibraried)
            {
                command.Id = Guid.Empty;
                command.IsLibraried = false;
                command.Passes.ForEach(pass =>
                {
                    pass.Id = default;
                    pass.IsLibraried = false;

                    if (pass.PassSalesAreaPriorities is null)
                    {
                        return;
                    }

                    if (!pass.PassSalesAreaPriorities.AreTimesRetained)
                    {
                        pass.PassSalesAreaPriorities.IsPeakTime = false;
                        pass.PassSalesAreaPriorities.IsOffPeakTime = false;
                        pass.PassSalesAreaPriorities.IsMidnightTime = false;
                        pass.PassSalesAreaPriorities.StartTime = run.StartTime;
                        pass.PassSalesAreaPriorities.EndTime = run.EndTime;
                    }

                    if (pass.PassSalesAreaPriorities.AreDatesRetained)
                    {
                        return;
                    }

                    pass.PassSalesAreaPriorities.StartDate = run.StartDate;
                    pass.PassSalesAreaPriorities.EndDate = run.EndDate;
                });
            }

            var runScenario = new RunScenario() { Id = command.Id == Guid.Empty ? new Guid() : command.Id };    // Allow caller to generate their own ScenarioID
            runScenario.ResetToPendingStatus();

            run.LastModifiedDateTime = DateTime.UtcNow;
            run.Scenarios.Add(runScenario);

            // Get changes for Scenario & Pass repositories
            List<Scenario> allScenarios = new List<Scenario>();
            List<Scenario> updatedScenarios = new List<Scenario>();
            List<Scenario> newScenarios = new List<Scenario>();
            List<List<Pass>> allPassesByScenario = new List<List<Pass>>();
            List<Pass> updatedPasses = new List<Pass>();
            List<Pass> newPasses = new List<Pass>();
            foreach (var currentRunScenario in run.Scenarios)
            {
                if (currentRunScenario == runScenario)   // New scenario
                {
                    GetChangesForScenarioAndPassRepositories(runScenario, command, allScenarios, updatedScenarios, newScenarios, allPassesByScenario, updatedPasses, newPasses);
                }
                else    // Existing scenario, load from DB
                {
                    allScenarios.Add(_scenarioRepository.Get(currentRunScenario.Id));
                    allPassesByScenario.Add(_passRepository.FindByScenarioId(currentRunScenario.Id).ToList());
                }
            }

            // Validate
            _runsValidator.ValidateForSave(run, allScenarios, allPassesByScenario, _salesAreaRepository.GetAll().ToList());

            // Update Scenario & Pass repositories
            UpdateScenarioAndPassRepositories(run, updatedScenarios, newScenarios, updatedPasses, newPasses, null, null);

            _runRepository.Update(run);
            _runRepository.SaveChanges();   // Do not remove this, need to persist changes now so that we can return RunModel
            _scenarioRepository.SaveChanges();
            _passRepository.SaveChanges();

            newPasses.AddRange(updatedPasses);
            newScenarios.AddRange(updatedScenarios);
            var runModel = Mappings.MapToRunModel(run, _scenarioRepository, _passRepository, _tenantSettingsRepository,
                _mapper, _analysisGroupRepository, null, null, null, newPasses, newScenarios);

            return Ok(runModel);
        }

        /// <summary>
        /// Adds the list of deleted scenarios and passes in to
        /// deletedScenarioIds and deletedPassIds
        /// </summary>
        /// <param name="existingRunScenarios"></param>
        /// <param name="updatedRunScenarios"></param>
        /// <param name="deletedScenarioIds"></param>
        /// <param name="deletedPassIds"></param>
        private void GetDeletedScenariosAndPasses(List<RunScenario> existingRunScenarios, List<CreateRunScenarioModel> updatedRunScenarios,
                                                  List<Guid> deletedScenarioIds, List<int> deletedPassIds)
        {
            if (existingRunScenarios != null && updatedRunScenarios != null)
            {
                // Get old scenarios & passes
                var oldScenarioIds = existingRunScenarios.Select(s => s.Id);
                var oldPassIds = _scenarioRepository.GetScenariosWithPassId(oldScenarioIds).Select(swp => swp.PassId).Distinct().ToList();

                // Get new scenarios & passes
                var newScenarioIds = updatedRunScenarios.Select(s => s.Id);
                var newPassIds = updatedRunScenarios.SelectMany(s => s.Passes).Select(p => p.Id).Distinct().ToList();

                // Add deleted scenarios to list
                var oldScenarioIdsToDelete = oldScenarioIds.Except(newScenarioIds);
                if (oldScenarioIdsToDelete.Any())
                {
                    deletedScenarioIds = deletedScenarioIds == null ? new List<Guid>() : deletedScenarioIds;
                    deletedScenarioIds.AddRange(oldScenarioIdsToDelete);
                }

                // Add deleted passes to list
                var oldPassIdsToDelete = oldPassIds.Except(newPassIds);
                if (oldPassIdsToDelete.Any())
                {
                    deletedPassIds = deletedPassIds == null ? new List<int>() : deletedPassIds;
                    deletedPassIds.AddRange(oldPassIdsToDelete);
                }
            }
        }

        /// <summary>
        /// Update a run based on the supplied Run id and UpdateRunModel
        /// </summary>
        /// <param name="id">The Id <see cref="Guid"/> of the run to Update</param>
        /// <param name="updateRunModel">The Update Run Model <see cref="UpdateRunModel"/></param>
        /// <returns>The Updted Run <see cref="RunModel"/> if successfull.</returns>
        /// <response code="200">
        /// Returns the Updated Run <see cref="RunModel"/> with 200 OK Response
        /// </response>
        /// <response code="400">
        /// Returns 400 Bad Request, when the supplied updateRunModel is
        /// invalid/fails validation
        /// </response>
        /// <response code="500">
        /// Returns 500 internal server erros for any failures
        /// </response>
        [Route("{id}")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult Put([FromUri] Guid id, [FromBody] UpdateRunModel updateRunModel)
        {
            if (updateRunModel == null || !ModelState.IsValid || id != updateRunModel.Id)
            {
                return BadRequest(ModelState);
            }

            var existingRun = _runRepository.Find(id);
            if (existingRun == null)
            {
                return NotFound();
            }
            if (!updateRunModel.StartTime.HasValue)
            {
                updateRunModel.StartTime = existingRun.StartTime;
            }
            if (!updateRunModel.EndTime.HasValue)
            {
                updateRunModel.EndTime = existingRun.EndTime;
            }

            // get all the campaigns with product info. this will be used to
            // validate CampaignPassPriorities and to add defaults
            var campaignsResult = _campaignRepository.GetWithProduct(null);
            var allCampaigns = campaignsResult.Items != null && campaignsResult.Items.Any() ? campaignsResult.Items.ToList() : null;
            var allSalesAreas = _salesAreaRepository.GetAll().ToList();

            //validate the updateRunModel and send BadRequest if any validation fails
            var validationErrorMsg = ValidateForUpdate(updateRunModel, existingRun, allCampaigns, allSalesAreas);
            if (!string.IsNullOrWhiteSpace(validationErrorMsg))
            {
                return BadRequest(validationErrorMsg);
            }

            // Get deleted scenario Ids & pass Ids
            var deletedScenarioIds = new List<Guid>();
            var deletedPassIds = new List<int>();
            GetDeletedScenariosAndPasses(existingRun.Scenarios, updateRunModel.Scenarios, deletedScenarioIds, deletedPassIds);

            // sort out campaigns not included in run if needed (empty Campaigns
            // collection means all campaigns existing)
            if (allCampaigns != null && updateRunModel.Campaigns != null && updateRunModel.Campaigns.Any())
            {
                var runCampaignsExternalIds = updateRunModel.Campaigns.Select(x => x.ExternalId).ToImmutableHashSet();

                allCampaigns.RemoveAll(x => !runCampaignsExternalIds.Contains(x.ExternalId));
            }

            //Convert CreateRunModel To Run by processing scenarios, passes, campaign pass priorities and sales area priorities
            ProcessRunWithUpdatesInUpdateRunModel(existingRun, updateRunModel, allCampaigns, allSalesAreas);

            var allScenarios = new List<Scenario>();
            var updatedScenarios = new List<Scenario>();
            var newScenarios = new List<Scenario>();
            var allPassesByScenario = new List<List<Pass>>();
            var updatedPasses = new List<Pass>();
            var newPasses = new List<Pass>();

            // Get changes for Scenario & Pass repositories
            GetChangesForScenariosAndPasses(existingRun.Scenarios, updateRunModel.Scenarios, allScenarios, updatedScenarios,
                                            newScenarios, allPassesByScenario, updatedPasses, newPasses, allCampaigns);

            // Validate the Run before saving
            _runsValidator.ValidateForSave(existingRun, allScenarios, allPassesByScenario, allSalesAreas);

            // apply Run processes settings for Campaign Pass Priorities in all Scenarios
            // TODO: Applying Run processes Settings during the update should be done conditionally
            // only when the CampaignsProcessesSettings is changed in the Update
            // (Technical Debt)
            if (existingRun.CampaignsProcessesSettings != null && existingRun.CampaignsProcessesSettings.Any())
            {
                ApplyRunProcessesSettingsForCampaignPassPriorities(allScenarios, existingRun.CampaignsProcessesSettings);
            }

            _runRepository.Update(existingRun);

            if (newPasses != null && newPasses.Any())
            {
                _passRepository.Add(newPasses);
            }
            if (updatedPasses != null && updatedPasses.Any())
            {
                _passRepository.Update(updatedPasses);
            }
            if (deletedPassIds != null && deletedPassIds.Any())
            {
                _passRepository.Remove(deletedPassIds);
            }
            if (deletedScenarioIds != null && deletedScenarioIds.Any())
            {
                _scenarioRepository.Remove(deletedScenarioIds);
            }

            // commit run, pass and scenario delete in 1 transaction
            _runRepository.SaveChanges();

            // Bulk Add/Update scenarios and save to db
            if (newScenarios != null && newScenarios.Any())
            {
                _scenarioRepository.Add(newScenarios);
            }
            if (updatedScenarios != null && updatedScenarios.Any())
            {
                _scenarioRepository.Update(updatedScenarios);
            }

            _scenarioRepository.SaveChanges();

            newPasses.AddRange(updatedPasses);
            newScenarios.AddRange(updatedScenarios);
            var runModel = Mappings.MapToRunModel(existingRun, _scenarioRepository, _passRepository, _tenantSettingsRepository,
                _mapper, _analysisGroupRepository, null, null, null, newPasses, newScenarios);

            return Ok(runModel);
        }

        /// <summary>
        /// Executes a run
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/Execute")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(RunModel))]
        public IHttpActionResult Execute([FromUri] Guid id)
        {
            if (!ModelState.IsValid || id == null || id == Guid.Empty)
            {
                return this.Error().InvalidParameters("Invalid Run id parameter.");
            }

            // Prevent multiple attempts to execute the same run as scenario
            // status might not get updated for a few seconds due to
            // ValidateForStartRun taking time.
            using (MachineLock.Create($"xggameplan.RunController.{id}.execute", TimeSpan.FromSeconds(60)))
            {
                _auditEventRepository.Insert(
                    AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                        $"Entered MachineLock for Execute runid: {id}"));
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    if (!_synchronizationService.TryCapture(SynchronizedServiceType.RunExecution, id, out var token))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                            "Data synchronization is in progress"));
                    }

                    var isSyncLockReleaseNeeded = true;
                    try
                    {
                        // Use local IDocumentSession for updating Run because
                        // calling IDocumentSession.SaveChanges for
                        // _runRepository may temporarily revert the status.
                        var runRepository = scope.CreateRepository<IRunRepository>();
                        Run run = runRepository.Find(id);
                        if (run == null)
                        {
                            return NotFound();
                        }

                        // Determine that we can start run
                        var errors = new List<SystemMessage>();
                        var _httpCode = HttpStatusCode.InternalServerError; //default
                        try
                        {
                            var validateForStartResults = _runManager.ValidateForStartRun(run);
                            foreach (var warning in validateForStartResults.Where(x => x.SeverityLevel == SystemMessageSeverityLevel.Warning))
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunValidationWarning(
                                    0, 0, run.Id, warning.Description[Globals.SupportedLanguages.First()]));
                            }
                            if (validateForStartResults.Any(x => x.SeverityLevel == SystemMessageSeverityLevel.Error))
                            {
                                errors = validateForStartResults.Where(x => x.SeverityLevel == SystemMessageSeverityLevel.Error).ToList();
                                string validationErrorMessage =
                                    errors.First().Description[Globals.SupportedLanguages.First()];
                                _httpCode = errors.First().HttpCode;
                                throw new Exception(validationErrorMessage);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (errors != null && errors.Any())
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunValidationFailure(
                                    0, 0, run.Id,
                                    GetSystemMessageDescriptions(errors, Globals.SupportedLanguages.First())));
                            }
                            else
                            {
                                _auditEventRepository.Insert(
                                    AuditEventFactory.CreateAuditEventForRunValidationFailure(0, 0, run.Id,
                                        exception.Message));
                            }

                            return ResponseMessage(Request.CreateErrorResponse(_httpCode, exception.Message));
                        }

                        try
                        {
                            ProcessRunBeforeStartingTheRun(run);
                            runRepository.Update(run);

                            // Persist changes before we exit lock, don't wait
                            // for HTTP request handler to call SaveChanges
                            runRepository.SaveChanges();

                            try
                            {
                                StartRun(run);

                                isSyncLockReleaseNeeded = false;
                            }
                            catch
                            {
                                // Failed, reset status
                                using (var scope2 = scope.BeginRepositoryScope())
                                {
                                    runRepository = scope2.CreateRepository<IRunRepository>();
                                    run = runRepository.Find(id);
                                    run.ExecuteStartedDateTime = null;
                                    run.Scenarios.ForEach(s => s.ResetToPendingStatus());
                                    runRepository.Update(run);
                                    runRepository.SaveChanges();
                                }

                                throw;
                            }
                        }
                        catch (Exception exception)
                        {
                            // Generate notification for run start failed, most
                            // likely a rare config issue
                            _auditEventRepository.Insert(
                                AuditEventFactory.CreateAuditEventForRunStartFailed(0, 0, id, exception.Message));
                            throw;
                        }

                        // Generate notification for run started
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunStarted(0, 0, id, null));

                        var runModel = Mappings.MapToRunModel(run, _scenarioRepository, _passRepository,
                            _tenantSettingsRepository, _mapper, _analysisGroupRepository);

                        _auditEventRepository.Insert(
                            AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                                $"Finishing Execute runid: {id}"));
                        return Ok(runModel);
                    }
                    finally
                    {
                        if (isSyncLockReleaseNeeded)
                        {
                            _synchronizationService.Release(token);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Requests an Automated book run in Landmark
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{scenarioId}/execute-landmark")]
        [AuthorizeRequest("Runs")]
        [FeatureFilter(nameof(ProductFeature.LandmarkBooking))]
        public async Task<IHttpActionResult> ExecuteLandmark([FromUri] Guid scenarioId)
        {
            var command = new LandmarkRunTriggerModel
            {
                ScenarioId = scenarioId
            };

            if (!_landmarkRunTriggerModelValidator.IsValid(command))
            {
                return _landmarkRunTriggerModelValidator.BadRequest();
            }

            _backgroundJobManager.StartJob<LandmarkTriggerRunJob>(
                new BackgroundJobParameter<LandmarkRunTriggerModel>(command));

            return Ok();
        }

        /// <summary>
        /// Requests an book run in Landmark
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("execute-landmark/schedule")]
        [AuthorizeRequest("Runs")]
        [FeatureFilter(nameof(ProductFeature.LandmarkBooking))]
        public async Task<IHttpActionResult> ExecuteLandmarkSchedule(ScheduledRunSettingsModel model)
        {
            var command = new LandmarkRunTriggerModel
            {
                ScenarioId = model.ScenarioId
            };

            if (!_scheduledRunSettingsModelValidator.IsValid(model))
            {
                return _scheduledRunSettingsModelValidator.BadRequest();
            }

            if (!_landmarkRunTriggerModelValidator.IsValid(command))
            {
                return _landmarkRunTriggerModelValidator.BadRequest();
            }

            var scheduleRunSettings = _mapper.Map<xggameplan.model.Internal.Landmark.ScheduledRunSettingsModel>(model);
            scheduleRunSettings.CreatorId = HttpContext.Current.GetCurrentUser().Id;

            _backgroundJobManager.StartJob<LandmarkTriggerRunJob>(
                new BackgroundJobParameter<LandmarkRunTriggerModel>(command),
                new BackgroundJobParameter<xggameplan.model.Internal.Landmark.ScheduledRunSettingsModel>(
                    scheduleRunSettings));

            return Ok();
        }

        /// <summary>
        /// Cancels an Automated book run in Landmark
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{scenarioId}/CancelLandmark")]
        [AuthorizeRequest("Runs")]
        [FeatureFilter(nameof(ProductFeature.LandmarkBooking))]
        public async Task<IHttpActionResult> CancelLandmarkRun([FromUri] Guid scenarioId)
        {
            if (scenarioId == Guid.Empty)
            {
                return BadRequest();
            }

            var result = await _landmarkRunService.CancelRunAsync(scenarioId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// Concatenates the system message descriptions
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private static string GetSystemMessageDescriptions(List<SystemMessage> messages, string language)
        {
            System.Text.StringBuilder list = new System.Text.StringBuilder("");
            foreach (SystemMessage message in messages)
            {
                if (list.Length > 0)
                {
                    list.Append("; ");
                }
                list.Append(message.Description[language]);
            }
            return list.ToString();
        }

        /// <summary>
        /// Validates whether the run can be executed, returns a list of system
        /// messages with any issues. This check is also performed prior to
        /// executing the run.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/ValidateForExecuteV2")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(List<SystemMessageModel>))]
        public IHttpActionResult GetValidateForExecuteV2([FromUri] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid Run parameters");
            }

            Run run = _runRepository.Find(id);
            if (run is null)
            {
                return this.NotFound();
            }

            // Determine that we can start run
            var messages = _runManager.ValidateForStartRun(run);
            if (messages.Any())
            {
                _auditEventRepository.Insert(
                    AuditEventFactory.CreateAuditEventForRunValidationFailure(
                        0, 0, run.Id,
                        GetSystemMessageDescriptions(messages, Globals.SupportedLanguages.First())));
            }

            return Ok(_mapper.Map<List<SystemMessageModel>>(messages));
        }

        /// <summary>
        /// Asynchronously starts run
        /// </summary>
        /// <param name="run"></param>
        private void StartRun(Run run)
        {
            _backgroundJobManager.StartJob<RunCompletionBackgroundJob>(new BackgroundJobParameter<Guid>(run.Id));
            TaskInstance taskInstance = TaskInstanceFactory.CreateStartRunTaskInstance(_tenantIdentifier.Id, run.Id);
            TaskResult taskResult = new ProcessTaskExecutor(HostingEnvironment.MapPath("/"), _repositoryFactory, TimeSpan.FromMilliseconds(0), _auditEventRepository, _configuration).Execute(taskInstance);
            switch (taskResult.Status)
            {
                case TaskInstanceStatues.Starting: throw new Exception(taskResult.Exception == null ? "Unable to start task to start run" : string.Format("Unable to start task to start run: {0}", taskResult.Exception.Message));
                case TaskInstanceStatues.CompletedError: throw new Exception(taskResult.Exception == null ? "Unable to start task to start run" : string.Format("Unable to start task to start run: {0}", taskResult.Exception.Message));
            }
        }

        /// <summary>
        /// Deletes Run
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid Run parameters");
            }

            Run run = _runRepository.Find(id);
            if (run is null)
            {
                return NotFound();
            }

            Run.ValidateForDelete(run);

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Deleting run (RunID={run.Id})"));

            _runManager.DeleteRun(run.Id);

            // Log event
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForRunDeleted(0, 0, run.Id, null));
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Deleted run (RunID={run.Id})"));

            return this.NoContent();
        }

        /// <summary>
        /// Deletes scenario for run
        /// </summary>
        /// <param name="id"></param>
        /// ///
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [Route("{id}/scenarios/{scenarioId}")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult DeleteScenario([FromUri] Guid id,
                                        [FromUri] Guid scenarioId)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid Run parameters");
            }

            Run run = _runRepository.Find(id);
            if (run == null)
            {
                return this.NotFound();
            }
            RunScenario scenario = run.Scenarios.Where(x => x.Id == scenarioId).FirstOrDefault();
            if (scenario == null)
            {
                return this.NotFound();
            }

            // Validate
            Run.ValidateForDeleteScenario(run, scenario);

            // Delete scenario & pass
            _passRepository.RemoveByScenarioId(scenarioId);
            _scenarioRepository.Delete(scenarioId);

            // Delete scenario
            run.LastModifiedDateTime = DateTime.UtcNow;
            run.Scenarios.Remove(scenario);
            _runRepository.Update(run);

            return Ok();
        }

        /// <summary>
        /// Deletes all scenarios for run
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/scenarios")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult DeleteScenarios([FromUri] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid Run parameters");
            }

            Run run = _runRepository.Find(id);
            if (run == null)
            {
                return this.NotFound();
            }

            // Validate
            Run.ValidateForDeleteScenarios(run);

            // Delete scenarios & passes
            foreach (var scenario in run.Scenarios)
            {
                _passRepository.RemoveByScenarioId(scenario.Id);
                _scenarioRepository.Delete(scenario.Id);
            }

            // Delete all scenarios
            run.LastModifiedDateTime = DateTime.UtcNow;
            run.Scenarios.Clear();
            _runRepository.Update(run);

            return Ok();
        }

        /// <summary>
        /// Gets metrics for all scenarios in run.
        /// </summary>
        /// <param name="runId"></param>
        [Route("{runid}/metrics")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(IEnumerable<ScenarioMetricsResultModel>))]
        public IHttpActionResult GetMetrics([FromUri] Guid runId)
        {
            var run = _runRepository.Find(runId);
            if (run == null)
            {
                return NotFound();
            }

            var allScenarioResults = new List<ScenarioResult>();

            foreach (var item in run.Scenarios)
            {
                var result = _scenarioResultRepository.Find(item.Id);

                if (result != null)
                {
                    allScenarioResults.Add(result);
                }
            }

            var scenarioMetricResultModels = _mapper.Map<List<ScenarioMetricsResultModel>>(allScenarioResults);

            if (run.IsCompleted)
            {
                var comparisonConfigs = _kpiComparisonConfigRepository.GetAll();
                KPIProcessing.KPIRanking.PerformRanking(scenarioMetricResultModels, comparisonConfigs);
            }

            return Ok(scenarioMetricResultModels);
        }

        [Route("Search")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(PagedQueryResult<RunSearchResultModel>))]
        public IHttpActionResult GetSearch([FromUri(Name = "")] RunSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid || queryModel == null)
            {
                return this.Error().InvalidParameters("One or more of the required query parameters are invalid.");
            }

            var freeTextMatchRules = new StringMatchRules(StringMatchHowManyWordsToMatch.AllWords, StringMatchWordOrders.AnyOrder, StringMatchWordComparisons.ContainsWord, false, new Char[] { ' ', ',' }, null);
            var extendedRuns = _runRepository.Search(queryModel, freeTextMatchRules);
            var runs = _mapper.Map<List<Run>>(extendedRuns.Items);

            var runSearchResultModel = Mappings.MapToRunSearchResultModel(runs, _mapper);

            var results = new PagedQueryResult<RunSearchResultModel>(extendedRuns.TotalCount, runSearchResultModel);
            return Ok(results);
        }

        [Route("RunProcesses/{runId}")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PostApplyProcessesSettingsToSpecificRun([FromUri] Guid runId, [FromBody] IEnumerable<CampaignRunProcessesSettingsModel> campaignRunProcessesSettings)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid settings parameters");
            }

            try
            {
                campaignRunProcessesSettings.ForEach(c => c.Validate());
                _runsValidator.ValidateDeliveryCappingGroupIds(campaignRunProcessesSettings);
            }
            catch (ArgumentException exception)
            {
                return this.Error().InvalidParameters(exception.Message);
            }

            if (!_runManager.Exists(runId))
            {
                return NotFound();
            }

            _runManager.ApplyCampaignProcessesConfigurations(_mapper.Map<IEnumerable<CampaignRunProcessesSettings>>(campaignRunProcessesSettings), runId);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("RunProcesses/all")]
        [AuthorizeRequest("Runs")]
        public IHttpActionResult PostApplyProcessesSettingsToAllRuns([FromBody] IEnumerable<CampaignRunProcessesSettingsModel> campaignRunProcessesSettings)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid settings parameters");
            }

            try
            {
                campaignRunProcessesSettings.ForEach(c => c.Validate());
                _runsValidator.ValidateDeliveryCappingGroupIds(campaignRunProcessesSettings);
            }
            catch (ArgumentException exception)
            {
                return this.Error().InvalidParameters(exception.Message);
            }

            var campaignRunProcessesSettingsList =
                _mapper.Map<List<CampaignRunProcessesSettings>>(campaignRunProcessesSettings);

            _runManager.ApplyCampaignProcessesConfigurations(campaignRunProcessesSettingsList);

            var notStartedRuns = _runRepository.GetAll().Where(r => r.RunStatus == RunStatus.NotStarted && r.Scenarios.Any());

            var scenarioIds = notStartedRuns.SelectMany(x => x.Scenarios).Select(x => x.Id).Distinct();
            var scenarios = _scenarioRepository.FindByIds(scenarioIds).ToList();

            ApplyRunProcessesSettingsForCampaignPassPriorities(scenarios, campaignRunProcessesSettingsList);

            _scenarioRepository.Update(scenarios);
            _scenarioRepository.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Generates list of scenarios using autopilot logic
        /// </summary>
        /// <param name="command"></param>
        /// <returns>List of generated scenarios</returns>
        /// <remarks>Does not persist entities to the database</remarks>
        [Route("AutopilotScenarios")]
        [AuthorizeRequest("Runs")]
        [ResponseType(typeof(IEnumerable<AutopilotScenarioModel>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(IEnumerable<ErrorModel>))]
        public IHttpActionResult PostAutopilotScenarios([FromBody] AutopilotEngageModel command)
        {
            return !_autopilotEngageModelDataValidator.IsValid(command)
                ? _autopilotEngageModelDataValidator.BadRequest()
                : Ok(_autopilotManager.Engage(command));
        }

        // validations TODO these validations should be moved outside the
        // controller (Technical Debt)

        /// <summary>
        /// Validate the createRunModel For Creating a new Run. In sync with
        /// existing patterns of throwing exception when any validation fails
        /// </summary>
        /// <param name="createRunModel">The createRunModel <see cref="CreateRunModel"/></param>
        /// <param name="allCampaigns">The allCampaigns <see cref="List{CampaignWithProductFlatModel}"/></param>
        /// <param name="defaultScenario">The default Scenario <see cref="Scenario"/></param>
        /// <returns>The Error Message as string if any validation fails</returns>
        /// <remarks>
        /// NOTE: This is a temporary solution because of the tight delivery schedule
        /// TODO: Technical Debt. Change this to use a seperate validator
        /// </remarks>
        private string ValidateForCreate(CreateRunModel createRunModel, List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas, Scenario defaultScenario)
        {
            string validationError = null;

            try
            {
                // If RunID was specified then check that it doesn't exist
                if (createRunModel.Id != Guid.Empty)
                {
                    if (_runRepository.Exists(createRunModel.Id))
                    {
                        throw new Exception($"A Run already exists with Id: {createRunModel.Id.ToString()}");
                    }
                }

                // Validate passed Campaign Run Processes settings
                if (createRunModel.CampaignsProcessesSettings != null && createRunModel.CampaignsProcessesSettings.Any())
                {
                    createRunModel.CampaignsProcessesSettings.ForEach(c => c.Validate());
                }

                //check if Default Scenario Exists when AddDefaultScenario is true
                if (createRunModel.AddDefaultScenario)
                {
                    if (defaultScenario == null)
                    {
                        throw new Exception("Cannot add default scenario because there is no default scenario found.");
                    }
                }
                else
                {
                    if (createRunModel.Scenarios == null || !createRunModel.Scenarios.Any())
                    {
                        throw new Exception("Run contains no scenarios.");
                    }
                }

                //validate scenarios
                if (createRunModel.Scenarios != null && createRunModel.Scenarios.Any())
                {
                    ValidateScenarios(createRunModel.Scenarios, allCampaigns, allSalesAreas);
                }

                if (createRunModel.EfficiencyPeriod.HasValue
                    && createRunModel.EfficiencyPeriod == EfficiencyCalculationPeriod.NumberOfWeeks
                    && !createRunModel.NumberOfWeeks.HasValue)
                {
                    throw new ArgumentException(
                        "Number of weeks must be set when Efficiency Period is 'Number of weeks'");
                }

                ValidatePositionInProgrammeValue(createRunModel.PositionInProgramme);

                if (!HasAtleastOneProcessSelected(createRunModel))
                {
                    throw new Exception("At least one step must be selected");
                }
            }
            catch (Exception ex)
            {
                validationError = ex.Message;
            }

            return validationError;
        }

        private void ValidatePositionInProgrammeValue(string positionInProgramme)
        {
            var featureEnabled = _featureManager.IsEnabled(nameof(ProductFeature.UseBreakPositionInProgram));

            if (featureEnabled)
            {
                if (!acceptedPositionInProgrammeValues.Contains(positionInProgramme))
                {
                    throw new Exception("Position in Programme must be set to one of the following values: A, C, E");
                }
            }
            else
            {
                if (positionInProgramme != "A" && positionInProgramme != null)
                {
                    throw new Exception("Position in Programme must be set to one of the following values: A, null");
                }
            }
        }

        private bool HasAtleastOneProcessSelected(CreateRunModel createRunModel) =>
            createRunModel.Smooth
            || createRunModel.ISR
            || createRunModel.RightSizer
            || createRunModel.Optimisation;

        private void ValidateScenarios(List<CreateRunScenarioModel> scenarios, List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas)
        {
            if (scenarios == null || !scenarios.Any())
            {
                return;
            }

            var errorMsgBuilder = new StringBuilder(string.Empty);

            var scenarioDuplicateError = ValidateScenariosForUniqueNames(scenarios);
            if (!string.IsNullOrWhiteSpace(scenarioDuplicateError))
            {
                errorMsgBuilder.Append(scenarioDuplicateError);
            }

            var allCampaignsExternalIds = allCampaigns is null
                ? new HashSet<string>()
                : new HashSet<string>(allCampaigns.Select(c => c.ExternalId));

            foreach (var scenario in scenarios)
            {
                var scenarioError = ValidateScenario(scenario, allCampaignsExternalIds, allSalesAreas);
                if (!string.IsNullOrWhiteSpace(scenarioError))
                {
                    errorMsgBuilder.Append(scenarioError);
                }
            }

            if (errorMsgBuilder.Length > 0)
            {
                throw new Exception(errorMsgBuilder.ToString());
            }
        }

        private string ValidateScenariosForUniqueNames(List<CreateRunScenarioModel> scenarios)
        {
            var errorMsgBuilder = new StringBuilder(string.Empty);

            if (scenarios != null && scenarios.Any() &&
                scenarios.Where(s => s != null &&
                !string.IsNullOrWhiteSpace(s.Name)).Select(s => s.Name).Distinct().Count() != scenarios.Count)
            {
                errorMsgBuilder.AppendLine("Run contains Scenario(s) with invalid/duplicate Name.");
            }

            return errorMsgBuilder.ToString();
        }

        private string ValidateScenario(
            CreateRunScenarioModel scenario,
            HashSet<string> allCampaignsExternalIds,
            List<SalesArea> allSalesAreas)
        {
            var errorMsgBuilder = new StringBuilder(string.Empty);

            if (scenario is null)
            {
                return errorMsgBuilder.ToString();
            }

            if (string.IsNullOrWhiteSpace(scenario.Name))
            {
                errorMsgBuilder.Append("Scenario name is required.");
            }

            var scenarioName = !string.IsNullOrWhiteSpace(scenario.Name) ? scenario.Name : "with no Name.";
            if (scenario.Passes == null || !scenario.Passes.Any())
            {
                errorMsgBuilder.AppendLine($"Scenario: {scenarioName} contains no passes, One or more passes is required.");
            }
            else
            {
                var passNames = new HashSet<string>();
                foreach (var pass in scenario.Passes)
                {
                    passNames.Add(pass.Name);

                    if (_passInspectorService.InspectPassSalesAreaPriorities(
                        pass.PassSalesAreaPriorities,
                        out string passSalesAreaPrioritiesErrorMessage))
                    {
                        errorMsgBuilder.AppendLine($"Scenario: {scenarioName}, Pass: {pass.Name}, {passSalesAreaPrioritiesErrorMessage}");
                    }

                    if (BreakExclusionsValidations.DateTimeRangeIsValid(pass.BreakExclusions, allSalesAreas, out var breakExclusionsErrorMessages))
                    {
                        continue;
                    }

                    foreach (var breakExclusionsErrorMessage in breakExclusionsErrorMessages)
                    {
                        errorMsgBuilder.AppendLine($"Scenario: {scenarioName}, Pass: {pass.Name},  {breakExclusionsErrorMessage}");
                    }
                }

                if (passNames.Count != scenario.Passes.Count)
                {
                    errorMsgBuilder.AppendLine($"Scenario: {scenarioName} contains passes with duplicate names.");
                }
            }

            var cppErrors = ValidateCampaignPassPriorities(scenario.CampaignPassPriorities, scenario.Passes, allCampaignsExternalIds);
            if (!string.IsNullOrWhiteSpace(cppErrors))
            {
                errorMsgBuilder.AppendLine($"Scenario: {scenarioName} CampaignPassPriorities contains error: {cppErrors}");
            }

            ValidateCampaignPriorityRounds(scenario.CampaignPriorityRounds, errorMsgBuilder);

            return errorMsgBuilder.ToString();
        }

        private void ValidateCampaignPriorityRounds(CampaignPriorityRoundsModel campaignPriorityRounds, StringBuilder errorMsgBuilder)
        {
            campaignPriorityRounds?.PopulateRoundNumbers();

            var validationResult = CampaignPriorityRoundsModelValidations.ValidateCampaignPriorityRounds(campaignPriorityRounds);

            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                errorMsgBuilder.AppendLine($"Scenario {nameof(CampaignPriorityRoundsModel)} contains error: {validationResult}");
            }
        }

        private static string ValidateCampaignPassPriorities(List<CreateCampaignPassPriorityModel> campaignPassPriorities,
                                                      List<PassModel> scenarioPasses, HashSet<string> allCampaignsExternalIds)
        {
            var errorMsgBuilder = new StringBuilder(string.Empty);

            if (campaignPassPriorities == null || !campaignPassPriorities.Any() || scenarioPasses == null || !scenarioPasses.Any())
            {
                return errorMsgBuilder.ToString();
            }

            var noOfPasses = scenarioPasses.Count;
            var scenarioPassNames = scenarioPasses.Select(s => s.Name).ToList();

            foreach (var priority in campaignPassPriorities)
            {
                if (string.IsNullOrWhiteSpace(priority.CampaignExternalId))
                {
                    errorMsgBuilder.AppendLine("CampaignExternalId is required for each Campaign in CampaignPassPriorities");
                    continue;
                }

                if (!allCampaignsExternalIds.Contains(priority.CampaignExternalId))
                {
                    errorMsgBuilder.AppendLine($"CampaignExternalId: {priority.CampaignExternalId} is not valid.");
                }

                if (priority.PassPriorities.Where(p => !string.IsNullOrWhiteSpace(p.PassName)).Select(p => p.PassName).Distinct().Count() != noOfPasses)
                {
                    errorMsgBuilder.AppendLine($"CampaignExternalId: {priority.CampaignExternalId} contains PassPriorities with invalid/duplicate Pass Names.");
                }

                if (priority.PassPriorities.Count != noOfPasses)
                {
                    errorMsgBuilder.AppendLine($"CampaignExternalId: {priority.CampaignExternalId} should contain {noOfPasses.ToString()} PassPriorities.");
                }

                // check if campaignPassPriorities has any PassPriorities with
                // PassName that are not found in scenarioPasses Name this check
                // is important since we use the PassName in
                // campaignPassPriorities and Name in scenarioPasses to identify
                // and relate for which pass the campaignPassPriority is set.
                if (priority.PassPriorities.Any(p => !p.PassName.In(scenarioPassNames)))
                {
                    errorMsgBuilder.AppendLine($"CampaignExternalId: {priority.CampaignExternalId} contains PassPriorities with PassName, not found in Scenario Passes.");
                }
            }

            return errorMsgBuilder.ToString();
        }

        /// <summary>
        /// Validate the updateRunModel For Updating an existing Run. In sync
        /// with existing patterns of throwing exception when any validation fails
        /// </summary>
        /// <param name="updateRunModel">The updateRunModel <see cref="UpdateRunModel"/></param>
        /// <param name="existingRun">The existingRun <see cref="Run"/></param>
        /// <param name="allCampaigns">The allCampaigns <see cref="List{CampaignWithProductFlatModel}"/></param>
        /// <returns>The Error Message as string if any validation fails</returns>
        /// <remarks>
        /// NOTE: This is a temporary solution because of the tight delivery schedule
        /// TODO: Technical Debt. Change this to use a seperate validator
        /// </remarks>
        private string ValidateForUpdate(UpdateRunModel updateRunModel, Run existingRun,
            List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas)
        {
            string validationError = null;

            try
            {
                if (existingRun.IsTemplate && !string.IsNullOrWhiteSpace(updateRunModel.Description) &&
                    updateRunModel.Description.Trim().ToUpperInvariant() != existingRun.Description.Trim().ToUpperInvariant())
                {
                    throw new Exception("The run is a template hence the description cannot be changed");
                }

                ValidatePositionInProgrammeValue(updateRunModel.PositionInProgramme);

                // Validate passed Campaign Run Processes settings
                if (updateRunModel.CampaignsProcessesSettings != null && updateRunModel.CampaignsProcessesSettings.Any())
                {
                    updateRunModel.CampaignsProcessesSettings.ForEach(c => c.Validate());
                }

                // check if Run contains any Scenario
                if (updateRunModel.Scenarios == null || !updateRunModel.Scenarios.Any())
                {
                    throw new Exception("Run contains no scenarios.");
                }
                else // validate scenarios
                {
                    ValidateScenariosForUpdate(updateRunModel.Scenarios, existingRun, allCampaigns, allSalesAreas);
                }
            }
            catch (Exception ex)
            {
                validationError = ex.Message;
            }

            return validationError;
        }

        private void ValidateScenariosForUpdate(List<CreateRunScenarioModel> scenarios, Run existingRun,
            List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas)
        {
            if (scenarios == null || !scenarios.Any())
            {
                return;
            }

            var errorMsgBuilder = new StringBuilder(string.Empty);

            var scenarioDuplicateError = ValidateScenariosForUniqueNames(scenarios);
            if (!string.IsNullOrWhiteSpace(scenarioDuplicateError))
            {
                errorMsgBuilder.Append(scenarioDuplicateError);
            }

            var allCampaignsExternalIds = allCampaigns is null
                ? new HashSet<string>()
                : new HashSet<string>(allCampaigns.Select(c => c.ExternalId));

            foreach (var scenario in scenarios)
            {
                var scenarioError = ValidateScenarioForUpdate(scenario, existingRun, allCampaignsExternalIds, allSalesAreas);
                if (!string.IsNullOrWhiteSpace(scenarioError))
                {
                    errorMsgBuilder.Append(scenarioError);
                }
            }

            if (errorMsgBuilder.Length > 0)
            {
                throw new Exception(errorMsgBuilder.ToString());
            }
        }

        private string ValidateScenarioForUpdate(CreateRunScenarioModel scenarioModel, Run existingRun, HashSet<string> allCampaignsExternalIds, List<SalesArea> allSalesAreas)
        {
            var errorMsgBuilder = new StringBuilder(string.Empty);

            if (scenarioModel == null)
            {
                return errorMsgBuilder.ToString();
            }

            var scenarioError = ValidateScenario(scenarioModel, allCampaignsExternalIds, allSalesAreas);
            if (!string.IsNullOrWhiteSpace(scenarioError))
            {
                errorMsgBuilder.Append(scenarioError);
            }

            var existingScenario = existingRun.Scenarios.FirstOrDefault(s => s.Id == scenarioModel.Id);
            if (existingScenario == null)
            {
                return errorMsgBuilder.ToString();
            }

            var scenarioName = !string.IsNullOrWhiteSpace(scenarioModel.Name) ? scenarioModel.Name : "with no Name.";

            // Check whether the existing Scenario.StartedDateTime is cleared in update
            if (existingScenario.StartedDateTime != null && scenarioModel.StartedDateTime == null)
            {
                errorMsgBuilder.AppendLine($"Scenario: {scenarioName}, the StartedDateTime for this scenario cannot be cleared.");
            }

            // Check whether the existing Scenario.CompletedDateTime is cleared
            // in update
            if (existingScenario.CompletedDateTime != null && scenarioModel.CompletedDateTime == null)
            {
                errorMsgBuilder.AppendLine($"Scenario: {scenarioName}, the  CompletedDateTime for this scenario cannot be cleared.");
            }

            return errorMsgBuilder.ToString();
        }

        // TODO these payload processors should be moved outside controller if
        // possible (Technical Debt)

        private Run ProcessCreateRunModelAndConvertToRun(CreateRunModel createRunModel,
            List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas, Scenario defaultScenario)
        {
            AddDefaultsToCreateRunModel(createRunModel, allCampaigns, allSalesAreas, defaultScenario);

            ProcessScenarioPassesAndCampaignPassPriorities(createRunModel, allCampaigns);

            var addCampaignRefs = _featureManager.IsEnabled(nameof(ProductFeature.RunCampaignListOnCreation));

            if (addCampaignRefs)
            {
                AddCampaignRefsToCreateRunModel(createRunModel, allCampaigns);
            }

            // converts to a Run assigning default values and also Sets Id for
            // run and all scenarios in run
            return ConvertToRunWithDefaults(createRunModel);
        }

        private void AddCampaignRefsToCreateRunModel(CreateRunModel createRunModel, List<CampaignWithProductFlatModel> allCampaigns)
        {
            createRunModel.Campaigns = allCampaigns.Select(c => new CampaignReferenceModel { ExternalId = c.ExternalId }).ToList();
        }

        private void AddDefaultsToCreateRunModel(CreateRunModel createRunModel,
            List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas, Scenario defaultScenario)
        {
            // when AddDefaultScenario is true, then we add default scenario
            // which also adds default CampaignPassPriorities
            if (createRunModel.AddDefaultScenario)
            {
                AddDefaultScenarioToRunModel(createRunModel, defaultScenario, allCampaigns);
            }

            AddDefaultSalesAreaPriorities(createRunModel, allSalesAreas);
            EnsureRunEfficiencySettingsExist(createRunModel);
        }

        /// <summary>
        /// converts to a Run assigning default values and also Sets Id for run
        /// and all scenarios in run
        /// </summary>
        /// <param name="createRunModel">the CreateRunModel</param>
        /// <returns>A Run</returns>
        private Run ConvertToRunWithDefaults(CreateRunModel createRunModel)
        {
            // ISRDateRange,SmoothDateRange,OptimisationDateRange,RightSizerDateRange
            // Null handling performed in Mapping layer now
            var run = _mapper.Map<Run>(createRunModel);
            run.CreatedDateTime = DateTime.UtcNow;
            run.LastModifiedDateTime = run.CreatedDateTime;
            run.ExecuteStartedDateTime = null;
            run.IsLocked = false;
            if (!createRunModel.StartTime.HasValue)
            {
                run.StartTime = GetDefaultStartTime();
            }
            if (!createRunModel.EndTime.HasValue)
            {
                run.EndTime = GetDefaultEndTime();
            }
            // Clear IDs, need to ensure that all IDs are unique
            run.CustomId = 0;
            run.Id = createRunModel.Id; // Allow caller to set RunID but not other IDs
            // Set all scenarios status to Pending
            run.Scenarios.ForEach(scenario => scenario.ResetToPendingStatus());
            // Set IDs for run and for all scenarios in run
            IdUpdater.SetIds(run, _identityGeneratorResolver);

            return run;
        }

        private void AmendExistingRunWithUpdatesAndAddDefaults(Run existingRun, UpdateRunModel updateRunModel)
        {
            // Truncate DateTimes in StartDate and EndDate
            updateRunModel.StartDate = updateRunModel.StartDate.Date;
            updateRunModel.EndDate = updateRunModel.EndDate.Date;
            //NextGen.Core.Services.IMapper implementation currently doesn't support source and destination mapping hence this code
            _mapper.Map(updateRunModel, existingRun);
            // ISRDateRange,SmoothDateRange,OptimisationDateRange,RightSizerDateRange
            // Null handling performed in Mapping layer now
            existingRun.LastModifiedDateTime = DateTime.UtcNow;

            // Set IDs for run and for all scenarios in run
            IdUpdater.SetIds(existingRun, _identityGeneratorResolver);
        }

        /// <summary>
        /// ToDo: The StartTime EndTime default value should be fetch from Sales
        /// Area Broadcast day Duration, refer to Epic XGPT-1268
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetDefaultStartTime() => new TimeSpan(6, 0, 0);

        /// <summary>
        /// ToDo: The StartTime EndTime default value should be fetch from Sales
        /// Area Broadcast day Duration, refer to Epic XGPT-1268
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetDefaultEndTime() => GetDefaultStartTime().Subtract(new TimeSpan(0, 0, 1));

        private Scenario GetDefaultScenario()
        {
            Scenario scenario = null;

            var defaultScenarioId = _tenantSettingsRepository.GetDefaultScenarioId();
            if (defaultScenarioId != Guid.Empty)
            {
                scenario = _scenarioRepository.Get(defaultScenarioId);
            }

            return scenario;
        }

        /// <summary>
        /// Add Default Scenario To runModelBase
        /// </summary>
        /// <param name="runModelBase">
        /// The runModelBase to add the default scenario to
        /// </param>
        /// <param name="defaultScenario">The default Scenario</param>
        /// <param name="allCampaigns">The list of all Campaigns</param>
        private void AddDefaultScenarioToRunModel(RunModelBase runModelBase, Scenario defaultScenario, List<CampaignWithProductFlatModel> allCampaigns)
        {
            if (runModelBase == null || defaultScenario == null)
            {
                return;
            }

            // Create copy of default scenario also adds default CampaignPassPriorities
            var defaultCreateRunScenarioModel = CreateDefaultRunScenario(defaultScenario, allCampaigns, runModelBase);
            if (defaultCreateRunScenarioModel != null)
            {
                runModelBase.Scenarios = runModelBase.Scenarios == null ? new List<CreateRunScenarioModel>() : runModelBase.Scenarios;
                runModelBase.Scenarios.Add(defaultCreateRunScenarioModel);
            }
        }

        private CreateRunScenarioModel CreateDefaultRunScenario(Scenario defaultScenario, List<CampaignWithProductFlatModel> allCampaigns, RunModelBase runModelBase)
        {
            if (defaultScenario == null)
            {
                return null;
            }

            var passIds = defaultScenario.Passes.Select(a => a.Id).ToList();

            // Get passes for all the passids in the scenario
            var passes = _passRepository.FindByIds(passIds).ToList();

            //overwrite all PassSalesAreaPriorities dates and time by run when Adding Default Scenario flag is set
            if (runModelBase != null)
            {
                passes.ForEach(pass =>
                {
                    if (pass.PassSalesAreaPriorities == null)
                    {
                        return;
                    }

                    // XGGT-14730: Override start\end times and POM flags
                    // of library pass only if retain times switch is off
                    if (!pass.PassSalesAreaPriorities.AreTimesRetained)
                    {
                        pass.PassSalesAreaPriorities.IsPeakTime = false;
                        pass.PassSalesAreaPriorities.IsOffPeakTime = false;
                        pass.PassSalesAreaPriorities.IsMidnightTime = false;
                        pass.PassSalesAreaPriorities.StartTime = runModelBase.StartTime ?? GetDefaultStartTime();
                        pass.PassSalesAreaPriorities.EndTime = runModelBase.EndTime ?? GetDefaultEndTime();
                    }

                    // Override start\end dates of pass from library only if
                    // retain dates switch is off and stop dates processing
                    if (!pass.PassSalesAreaPriorities.AreDatesRetained)
                    {
                        pass.PassSalesAreaPriorities.StartDate = runModelBase.StartDate;
                        pass.PassSalesAreaPriorities.EndDate = runModelBase.EndDate;

                        return;
                    }

                    // XGGT-14464: Case when pass for nightly run is
                    // completely outside run's dates range should be an error
                    var arePassDatesOutsideRun = pass.PassSalesAreaPriorities.EndDate < runModelBase.StartDate
                                                 || pass.PassSalesAreaPriorities.StartDate > runModelBase.EndDate;

                    if (arePassDatesOutsideRun)
                    {
                        throw new ArgumentException($"Pass: {pass.Name} has StartDate/EndDate completely outside run StartDate/EndDate");
                    }

                    // XGGT-14464: Cut off pass dates if they partially
                    // outside run's dates range
                    if (pass.PassSalesAreaPriorities.StartDate < runModelBase.StartDate)
                    {
                        pass.PassSalesAreaPriorities.StartDate = runModelBase.StartDate;
                    }

                    if (pass.PassSalesAreaPriorities.EndDate > runModelBase.EndDate)
                    {
                        pass.PassSalesAreaPriorities.EndDate = runModelBase.EndDate;
                    }
                });
            }

            // Map to ScenarioModel, we don't care about read-only properties
            var createRunScenarioModel = _mapper.Map<CreateRunScenarioModel>(Tuple.Create(defaultScenario, passes));

            // Clear IDs, this is what external caller does when they post a new ScenarioModel
            createRunScenarioModel.Id = Guid.Empty;
            if (createRunScenarioModel.Passes != null && createRunScenarioModel.Passes.Any())
            {
                createRunScenarioModel.Passes.ForEach(pass => pass.Id = 0);
            }

            // if no default CampaignPassPriorities exists then add all
            // campaigns to CampaignPassPriorities using priority from campaign
            if (createRunScenarioModel.CampaignPassPriorities == null || !createRunScenarioModel.CampaignPassPriorities.Any())
            {
                createRunScenarioModel.CampaignPassPriorities = ProduceCreateCampaignPassPriorityModels(allCampaigns, createRunScenarioModel.Passes);
            }
            else
            {
                // Create the CampaignPassPriorities for the campaigns which are
                // not in the default scenario's CampaignPassPriorities
                var campaignPassPrioritiesForNewCampaigns = CreateCampaignPassPriorities(allCampaigns,
                                                                                         createRunScenarioModel.CampaignPassPriorities,
                                                                                         createRunScenarioModel.Passes);

                if (campaignPassPrioritiesForNewCampaigns != null && campaignPassPrioritiesForNewCampaigns.Any())
                {
                    createRunScenarioModel.CampaignPassPriorities.AddRange(campaignPassPrioritiesForNewCampaigns);
                }
            }

            // set pass ids to 0
            if (createRunScenarioModel.CampaignPassPriorities != null && createRunScenarioModel.CampaignPassPriorities.Any())
            {
                createRunScenarioModel.CampaignPassPriorities.ForEach(c => c.PassPriorities.ForEach(p => p.PassId = 0));
            }

            return createRunScenarioModel;
        }

        private List<CreateCampaignPassPriorityModel> CreateCampaignPassPriorities(List<CampaignWithProductFlatModel> usingAllCampaigns,
                                                                                   List<CreateCampaignPassPriorityModel> existingCampaignPassPriorities,
                                                                                   List<PassModel> forScenarioPasses)
        {
            List<CreateCampaignPassPriorityModel> campaignPassPrioritiesForNewCampaigns = null;

            // get the new campaigns which are not in the default scenario's CampaignPassPriorities
            List<CampaignWithProductFlatModel> campaignsWithoutPassPriorities;
            if (existingCampaignPassPriorities is null)
            {
                campaignsWithoutPassPriorities = usingAllCampaigns;
            }
            else
            {
                var indexedExistingPriorities = new HashSet<string>(existingCampaignPassPriorities.Select(p => p.CampaignExternalId));

                campaignsWithoutPassPriorities = usingAllCampaigns?.Where(c => !indexedExistingPriorities.Contains(c.ExternalId)).ToList();
            }

            // create CampaignPassPriorities with new campaigns for passes in
            // the scenario
            if (campaignsWithoutPassPriorities != null && campaignsWithoutPassPriorities.Any())
            {
                campaignPassPrioritiesForNewCampaigns = ProduceCreateCampaignPassPriorityModels(campaignsWithoutPassPriorities, forScenarioPasses);
            }

            return campaignPassPrioritiesForNewCampaigns;
        }

        private List<CampaignPassPriority> CreateCampaignPassPriorities(List<Pass> forScenarioPasses,
                                                                        List<CampaignWithProductFlatModel> usingAllCampaigns,
                                                                        List<string> excludingCampaignsWithExternalIds)
        {
            if (forScenarioPasses == null || !forScenarioPasses.Any())
            {
                return null;
            }

            HashSet<string> indexedCampaignsForExclude = null;
            List<CampaignWithProductFlatModel> campaigns;

            if (excludingCampaignsWithExternalIds != null)
            {
                indexedCampaignsForExclude = new HashSet<string>(excludingCampaignsWithExternalIds);
            }

            // get the Campaigns excluding the Campaigns with ExternalId in excludingCampaignExternalIds
            if (excludingCampaignsWithExternalIds is null || !excludingCampaignsWithExternalIds.Any())
            {
                campaigns = usingAllCampaigns;
            }
            else
            {
                campaigns = usingAllCampaigns?.Where(c => !indexedCampaignsForExclude.Contains(c.ExternalId)).ToList();
            }

            // create CampaignPassPriorities with new campaigns for passes in
            // the scenario
            List<CampaignPassPriority> campaignPassPrioritiesForNewCampaigns = null;
            if (campaigns != null && campaigns.Any())
            {
                var campaignPassPriorities =
                    CampaignPassPrioritiesServiceMapper.CreateCampaignPassPriorityModels(
                        campaigns,
                        forScenarioPasses,
                        _mapper);
                campaignPassPrioritiesForNewCampaigns = campaignPassPriorities.Any() ?
                                                        CampaignPassPrioritiesServiceMapper.
                                                        MapToCampaignPassPriorities(
                                                            campaignPassPriorities,
                                                            usingAllCampaigns,
                                                            _mapper) :
                                                        new List<CampaignPassPriority>();
            }

            return campaignPassPrioritiesForNewCampaigns;
        }

        private List<CreateCampaignPassPriorityModel> ProduceCreateCampaignPassPriorityModels(List<CampaignWithProductFlatModel> forCampaigns, List<PassModel> withPasses)
        {
            var result = new List<CreateCampaignPassPriorityModel>();

            if (forCampaigns != null && forCampaigns.Any() && withPasses != null && withPasses.Any())
            {
                result = CampaignPassPrioritiesServiceMapper.CreateCampaignPassPriorityModels(
                    forCampaigns,
                    withPasses,
                    _mapper);
            }

            return result;
        }

        /// <summary>
        /// Gets changes for Scenario and Pass repositories
        /// </summary>
        /// <param name="runScenarios">the runScenarios</param>
        /// <param name="createRunScenarioModels">The create Run ScenarioModels</param>
        /// <param name="allScenarios">The list to contain all Scenarios</param>
        /// <param name="updatedScenarios">The list to contain updated Scenarios</param>
        /// <param name="newScenarios">The list to contain new Scenarios</param>
        /// <param name="allPassesByScenario">
        /// The list to contain all Passes By Scenario
        /// </param>
        /// <param name="updatedPasses">The list to contain updated Passes</param>
        /// <param name="newPasses">The list to contain new Passes</param>
        /// <param name="campaigns">The list of campaigns</param>
        /// <param name="defaultScenario">The default Scenario</param>
        private void GetChangesForScenariosAndPasses(List<RunScenario> runScenarios, List<CreateRunScenarioModel> createRunScenarioModels,
                                                     List<Scenario> allScenarios, List<Scenario> updatedScenarios, List<Scenario> newScenarios,
                                                     List<List<Pass>> allPassesByScenario, List<Pass> updatedPasses, List<Pass> newPasses,
                                                     List<CampaignWithProductFlatModel> campaigns, Scenario defaultScenario = null)
        {
            // runScenarios != null && runScenarios.Any() &&
            // createRunScenarioModels != null && createRunScenarioModels.Any()
            // && runScenarios.Count == createRunScenarioModels.Count
            if (runScenarios == null || !runScenarios.Any() || createRunScenarioModels == null || !createRunScenarioModels.Any() || runScenarios.Count != createRunScenarioModels.Count)
            {
                return;
            }

            var scenarioIds = runScenarios.Where(x => x.Id != Guid.Empty).Select(x => x.Id).Distinct();
            var existingScenarios = _scenarioRepository.FindByIds(scenarioIds).ToList();
            var indexedPasses = _passRepository.FindByIds(existingScenarios.SelectMany(x => x.Passes).Select(p => p.Id).Distinct()).ToDictionary(x => x.Id, x => x);
            var indexedScenarios = existingScenarios.ToDictionary(x => x.Id, x => x);

            for (int scenarioIndex = 0; scenarioIndex < runScenarios.Count; scenarioIndex++)
            {
                GetChangesForScenariosAndPasses(runScenarios[scenarioIndex], createRunScenarioModels[scenarioIndex], allScenarios, updatedScenarios,
                    newScenarios, allPassesByScenario, updatedPasses, newPasses, campaigns, indexedPasses, indexedScenarios, defaultScenario);
            }
        }

        /// <summary>
        /// Gets changes for Scenario and Pass repositories
        /// </summary>
        /// <param name="runScenario">the runScenario</param>
        /// <param name="createRunScenarioModel">The create Run ScenarioModel</param>
        /// <param name="allScenarios">The list to contain all Scenarios</param>
        /// <param name="updatedScenarios">The list to contain updated Scenarios</param>
        /// <param name="newScenarios">The list to contain new Scenarios</param>
        /// <param name="allPassesByScenario">
        /// The list to contain all Passes By Scenario
        /// </param>
        /// <param name="updatedPasses">The list to contain updated Passes</param>
        /// <param name="newPasses">The list to contain new Passes</param>
        /// <param name="campaigns">The list of campaigns</param>
        /// <param name="defaultScenario">The default Scenario</param>
        private void GetChangesForScenariosAndPasses(RunScenario runScenario, CreateRunScenarioModel createRunScenarioModel,
                                                     List<Scenario> allScenarios, List<Scenario> updatedScenarios,
                                                     List<Scenario> newScenarios, List<List<Pass>> allPassesByScenario,
                                                     List<Pass> updatedPasses, List<Pass> newPasses, List<CampaignWithProductFlatModel> campaigns,
                                                     Dictionary<int, Pass> existingPasses, IReadOnlyDictionary<Guid, Scenario> existingScenarios,
                                                     Scenario defaultScenario = null)
        {
            // get the scenario by Id
            Scenario scenario = runScenario.Id == Guid.Empty || !existingScenarios.ContainsKey(runScenario.Id)
                ? null
                : existingScenarios[runScenario.Id];

            if (scenario == null)    // New scenario, add to newScenarios
            {
                // if using Default Scenario then include those Campaigns in
                // Default CampaignPassPriorities
                var campaignsIncludingDefaults = (campaigns != null) ? campaigns : new List<CampaignWithProductFlatModel>();
                if (defaultScenario != null && defaultScenario.CampaignPassPriorities != null)
                {
                    var defaultCampaigns = _mapper.Map<List<CampaignWithProductFlatModel>>(defaultScenario.CampaignPassPriorities
                                                                                                 .Select(c => c.Campaign));
                    campaignsIncludingDefaults = defaultCampaigns.Union(campaignsIncludingDefaults).ToList();
                }

                IdUpdater.SetIds(createRunScenarioModel, _identityGeneratorResolver);
                scenario = _mapper.Map<Scenario>(Tuple.Create(createRunScenarioModel, campaignsIncludingDefaults));
                if (scenario != null)
                {
                    IdUpdater.SetCustomId(scenario, _identityGeneratorResolver);
                    scenario.Id = runScenario.Id;
                    newScenarios = newScenarios != null ? newScenarios : new List<Scenario>();
                    newScenarios.Add(scenario);
                }
            }
            else    // Existing scenario, update it
            {
                Mappings.ApplyChangesToScenarioAndSetIds(scenario, createRunScenarioModel, campaigns, runScenario.Id,
                    _identityGeneratorResolver, _mapper);
                updatedScenarios = updatedScenarios != null ? updatedScenarios : new List<Scenario>();
                updatedScenarios.Add(scenario);
            }

            allScenarios = allScenarios != null ? allScenarios : new List<Scenario>();
            allScenarios.Add(scenario);

            // Check passes
            var allPasses = new List<Pass>();
            for (int passIndex = 0; passIndex < createRunScenarioModel.Passes.Count; passIndex++)
            {
                PassModel passModel = createRunScenarioModel.Passes[passIndex];

                PassReference passReference = scenario.Passes[passIndex];
                Pass pass = existingPasses.ContainsKey(passReference.Id) ? existingPasses[passReference.Id] : null;

                // New pass, add to repository
                if (pass == null)
                {
                    pass = _mapper.Map<Pass>(passModel);
                    if (pass != null)
                    {
                        pass.Id = passReference.Id;     // Id is Set in above call to ApplyChangesToScenarioAndSetIds
                        newPasses = newPasses != null ? newPasses : new List<Pass>();
                        newPasses.Add(pass);
                    }
                }
                else    // Existing pass, update it
                {
                    Mappings.ApplyToPass(pass, passModel, _mapper);
                    updatedPasses = updatedPasses != null ? updatedPasses : new List<Pass>();
                    updatedPasses.Add(pass);
                }
                allPasses.Add(pass);
            }

            allPassesByScenario = allPassesByScenario != null ? allPassesByScenario : new List<List<Pass>>();
            allPassesByScenario.Add(allPasses);
        }

        private void AddDefaultSalesAreaPriorities(RunModelBase runModelBase, List<SalesArea> allSalesAreas)
        {
            // if no SalesAreaPriorities then add all salesareapriorities as
            // default with Priority set to Priority3
            if (runModelBase.SalesAreaPriorities == null || !runModelBase.SalesAreaPriorities.Any())
            {
                runModelBase.SalesAreaPriorities = CreateDefaultListOfSalesAreasPriorities(allSalesAreas);
            }
        }

        private void EnsureRunEfficiencySettingsExist(RunModelBase runModelBase)
        {
            if (runModelBase.EfficiencyPeriod.HasValue)
            {
                return;
            }

            var defaultEfficiencySettings = _efficiencySettingsRepository.GetDefault();
            runModelBase.EfficiencyPeriod = defaultEfficiencySettings.EfficiencyCalculationPeriod;
            runModelBase.NumberOfWeeks = defaultEfficiencySettings.DefaultNumberOfWeeks;
        }

        /// <summary>
        /// Clear Pass IDs if referenced by other scenarios so that new ones are
        /// generated. And also Creates CampaignPassPriorities When None Exists
        /// for a Scenario OR create CampaignPassPriorities for the campaigns
        /// which are not in a Scenario's CampaignPassPriorities
        /// </summary>
        /// <param name="runModelBase">the updateRunModel</param>
        /// <param name="allCampaigns">the List of CampaignWithProductFlatModel</param>
        /// <remarks>
        /// Clear Pass IDs if referenced by other scenarios so that new ones are
        /// generated. This code can be removed when Mulesoft has been changed
        /// to upload PassIDs=0 or use the AddDefaultScenario property. Mulesoft
        /// should clear the PassIDs as per the UI.
        /// </remarks>
        private void ProcessScenarioPassesAndCampaignPassPriorities(RunModelBase runModelBase, List<CampaignWithProductFlatModel> allCampaigns)
        {
            var scenariosWithPassIds = runModelBase.Scenarios.SelectMany(s => s.Passes).Any(p => p != null && p.Id > 0)
                ? _scenarioRepository.GetScenariosWithPassId().ToList()
                : null;

            foreach (var createRunScenarioModel in runModelBase.Scenarios)
            {
                // if there are no passes then we can't create any
                // CampaignPassPriorities info.
                if (createRunScenarioModel.Passes == null || !createRunScenarioModel.Passes.Any())
                {
                    continue;
                }

                foreach (var passModel in createRunScenarioModel.Passes.Where(p => p.Id > 0))
                {
                    if (scenariosWithPassIds != null &&
                        scenariosWithPassIds.Any(swp => swp.PassId == passModel.Id && swp.ScenarioId != createRunScenarioModel.Id))
                    {
                        // Another scenario refs this pass
                        passModel.Id = 0;
                    }
                }

                // if contains any CampaignPassPriorities then reset the pass
                // ids with the pass ids from the createRunScenarioModel passes
                if (createRunScenarioModel.CampaignPassPriorities != null && createRunScenarioModel.CampaignPassPriorities.Any())
                {
                    // delete CampaignPassPriorities for those campaigns which
                    // are not in the run's Campaigns collection (deleted from run)
                    var runCampaignsExternalIds = allCampaigns.Select(x => x.ExternalId).ToImmutableHashSet();

                    createRunScenarioModel.CampaignPassPriorities.RemoveAll(x => !runCampaignsExternalIds.Contains(x.CampaignExternalId));

                    // create the CampaignPassPriorities for the campaigns which
                    // are not in the createRunScenarioModel i.e. the current
                    // scenario's CampaignPassPriorities
                    var campaignPassPrioritiesForMissingCampaigns = CreateCampaignPassPriorities(allCampaigns,
                        createRunScenarioModel.CampaignPassPriorities,
                        createRunScenarioModel.Passes);

                    if (campaignPassPrioritiesForMissingCampaigns != null && campaignPassPrioritiesForMissingCampaigns.Any())
                    {
                        createRunScenarioModel.CampaignPassPriorities.AddRange(campaignPassPrioritiesForMissingCampaigns);
                    }

                    // this assumes the pass names sent are unique to cross
                    // reference with passes in scenario
                    var indexedPassNames = createRunScenarioModel.Passes.ToDictionary(x => x.Name.Trim().ToUpperInvariant(), x => x.Id);

                    createRunScenarioModel.CampaignPassPriorities.ForEach(c =>
                    {
                        c.PassPriorities.ForEach(p =>
                        {
                            var passName = p.PassName.Trim().ToUpperInvariant();

                            p.PassId = indexedPassNames.ContainsKey(passName) ? indexedPassNames[passName] : 0;
                        });
                    });
                }
                else
                {
                    // there are no CampaignPassPriorities in the current
                    // createRunScenarioModel hence we need to create using all
                    // campaigns for all the passes in the current
                    // createRunScenarioModel(i.e. current object in loop)
                    var campaignPassPrioritiesForAllCampaigns = CreateCampaignPassPriorities(allCampaigns, null, createRunScenarioModel.Passes);

                    if (campaignPassPrioritiesForAllCampaigns != null && campaignPassPrioritiesForAllCampaigns.Any())
                    {
                        createRunScenarioModel.CampaignPassPriorities = campaignPassPrioritiesForAllCampaigns;
                    }
                }
            }
        }

        private void ProcessRunWithUpdatesInUpdateRunModel(Run existingRun, UpdateRunModel updateRunModel, List<CampaignWithProductFlatModel> allCampaigns, List<SalesArea> allSalesAreas)
        {
            AddDefaultSalesAreaPriorities(updateRunModel, allSalesAreas);

            ProcessScenarioPassesAndCampaignPassPriorities(updateRunModel, allCampaigns);

            //update existing Run with updates in the updateRunModel and assign default values
            //and also Sets Id for run and all scenarios in run if not set
            AmendExistingRunWithUpdatesAndAddDefaults(existingRun, updateRunModel);
        }

        private void ProcessRunBeforeStartingTheRun(Run run)
        {
            run.ExecuteStartedDateTime = DateTime.UtcNow;
            ProcessRunScenariosBeforeStartingTheRun(run.Scenarios, run.ExecuteStartedDateTime.Value, run.CampaignsProcessesSettings);
            run.FailureTypes = _functionalAreaRepository.GetSelectedFailureTypeIds().ToList();

            var featureEnabled = _featureManager.IsEnabled(nameof(ProductFeature.ScenarioPerformanceMeasurementKPIs));

            if (featureEnabled)
            {
                ProcessIncludeEfficiencyFactorBeforeStartingTheRun(run);
            }
        }

        /// <summary>
        /// Processes Run Scenarios and the actual Scenarios related to each Run
        /// Scenario based on the supplied parameters
        /// </summary>
        /// <param name="runScenarios">The list of Run Scenarios</param>
        /// <param name="startedDateTime">
        /// The startedDateTime to set for each Run Scenario in the supplied
        /// list of Run Scenarios
        /// </param>
        /// <param name="campaignsProcessesSettings">The Run campaignsProcessesSettings</param>
        private void ProcessRunScenariosBeforeStartingTheRun(IEnumerable<RunScenario> runScenarios,
                                                             DateTime startedDateTime,
                                                             List<CampaignRunProcessesSettings> campaignsProcessesSettings)
        {
            if (runScenarios != null && runScenarios.Any())
            {
                // Flag as scheduled and Set the time started
                foreach (var scenario in runScenarios)
                {
                    scenario.Status = ScenarioStatuses.Scheduled;
                    scenario.StartedDateTime = startedDateTime;  // Default, may be overriden for when scenario is actually started
                }

                // get Scenario Ids from Run Scenarios to process campaign pass
                // priorities for each Scenario
                ProcessScenariosAndSaveUpdatesBeforeStartingTheRun(runScenarios.Select(s => s.Id).ToList(), campaignsProcessesSettings);
            }
        }

        private void ProcessIncludeEfficiencyFactorBeforeStartingTheRun(Run run)
        {
            const string includeEfficiencyFactorFacility = "EFFFAC";

            try
            {
                var facility = _facilityRepository.GetByCode(includeEfficiencyFactorFacility);

                run.IncludeEfficiencyFactor = facility.Enabled;
            }
            catch (Exception ex)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error getting Efficiency Factor Facility", ex));
                throw;
            }
        }

        /// <summary>
        /// Processes Scenarios matching the supplied Scenario Ids to add/ammend
        /// missing/new Campaign Pass Priorities
        /// </summary>
        /// <param name="forScenarioIds">The list of Scenario Ids to process</param>
        /// <param name="campaignsProcessesSettings">The Run campaignsProcessesSettings</param>
        private void ProcessScenariosAndSaveUpdatesBeforeStartingTheRun(IEnumerable<Guid> forScenarioIds,
                                                                        List<CampaignRunProcessesSettings> campaignsProcessesSettings)
        {
            if (forScenarioIds != null && forScenarioIds.Any())
            {
                var scenarios = _scenarioRepository.FindByIds(forScenarioIds);
                if (scenarios != null && scenarios.Any())
                {
                    // get all the campaigns with product info. this will be
                    // used to Add/Ammend Campaign Pass Priorities in each Scenario
                    var campaignsResult = _campaignRepository.GetWithProduct(null);
                    var allCampaigns = (campaignsResult.Items != null && campaignsResult.Items.Any()) ? campaignsResult.Items.ToList() : null;

                    // get all pass ids from scenarios
                    var allPassIds = scenarios.SelectMany(s => s.Passes.Select(p => p.Id)).Distinct().ToList();
                    // get all passes with Id matching the allPassIds
                    var allPasses = _passRepository.FindByIds(allPassIds).ToList();

                    // Add/Ammend Campaign Pass Priorities for each Scenario
                    var scenariosToUpdate = new List<Scenario>();
                    foreach (var scenario in scenarios)
                    {
                        // get the pass details for the passes in scenario
                        var scenarioPassIds = scenario.Passes.Select(p => p.Id).ToList();
                        var scenarioPasses = allPasses.Where(p => scenarioPassIds.Contains(p.Id)).ToList();
                        // Add/Ammend Campaign Pass Priorities with any missing
                        // or new Campaigns for the scenarioPasses
                        var existingCampaignExternalIds = scenario.CampaignPassPriorities.Any() ?
                                                           scenario.CampaignPassPriorities.Select(c => c.Campaign.ExternalId).ToList() : null;
                        var newCampaignPassPriorities = CreateCampaignPassPriorities(scenarioPasses, allCampaigns, existingCampaignExternalIds);
                        if (newCampaignPassPriorities != null && newCampaignPassPriorities.Any())
                        {
                            scenario.CampaignPassPriorities.AddRange(newCampaignPassPriorities);
                            scenariosToUpdate.Add(scenario);
                        }
                    }

                    // update scenarios and save changes
                    if (scenariosToUpdate.Any())
                    {
                        // apply Run processes settings for Campaign Pass
                        // Priorities in all Scenarios
                        // TODO: Applying Run processes Settings during the update should be done conditionally
                        // only for new Campaigns are added and their ExternalId
                        // is in the CampaignsProcessesSettings (Technical Debt)
                        if (campaignsProcessesSettings != null && campaignsProcessesSettings.Any())
                        {
                            ApplyRunProcessesSettingsForCampaignPassPriorities(scenariosToUpdate, campaignsProcessesSettings);
                        }

                        _scenarioRepository.Update(scenariosToUpdate);
                        _scenarioRepository.SaveChanges();
                    }
                }
            }
        }

        private void ApplyRunProcessesSettingsForCampaignPassPriorities(List<Scenario> inScenarios, List<CampaignRunProcessesSettings> usingSettings)
        {
            if (inScenarios == null || !inScenarios.Any() || usingSettings == null || usingSettings.All(s => s == null))
            {
                return;
            }

            var indexedProcessesSettings = usingSettings
                .Where(s => s != null && !string.IsNullOrWhiteSpace(s.ExternalId))
                .ToDictionary(x => x.ExternalId, x => x);

            if (!indexedProcessesSettings.Any())
            {
                return;
            }

            foreach (var scenario in inScenarios.Where(x => x != null))
            {
                if (scenario.CampaignPassPriorities is null || !scenario.CampaignPassPriorities.Any())
                {
                    continue;
                }

                foreach (var priority in scenario.CampaignPassPriorities.Where(cpp => cpp?.Campaign != null && indexedProcessesSettings.ContainsKey(cpp.Campaign.ExternalId)))
                {
                    var settings = indexedProcessesSettings[priority.Campaign.ExternalId];

                    priority.Campaign.InefficientSpotRemoval = settings.InefficientSpotRemoval ?? false;
                    priority.Campaign.IncludeRightSizer = Mappings.MapToIncludeRightSizer(settings);
                }
            }
        }

        [Route("DefaultEfficiencySettings")]
        [ResponseType(typeof(EfficiencySettingsModel))]
        public IHttpActionResult GetDefaultEfficiencySettings()
        {
            var settings = _efficiencySettingsRepository.GetDefault();

            return settings is null
                ? this.Error().ResourceNotFound("Default efficiency settings not found")
                : Ok(_mapper.Map<EfficiencySettingsModel>(settings));
        }

        [Route("DefaultEfficiencySettings")]
        [ResponseType(typeof(EfficiencySettingsModel))]
        public IHttpActionResult PutDefaultEfficiencySettings([FromBody] EfficiencySettingsModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return this.Error().InvalidParameters("Passed model is invalid.");
            }

            var payload = _mapper.Map<EfficiencySettings>(model);
            var validationResult = payload.ValidateForSave();
            if (!validationResult.success)
            {
                return this.Error().InvalidParameters(validationResult.validationErrorMessage);
            }

            var settings = _efficiencySettingsRepository.UpdateDefault(payload);

            return Ok(_mapper.Map<EfficiencySettingsModel>(settings));
        }

        [HttpGet()]
        [Route("triggered-landmark-runs")]
        [ResponseType(typeof(IEnumerable<TriggeredLandmarkRunInfo>))]
        public IHttpActionResult GetTriggeredLandmarkRuns()
        {
            var runs = _runRepository.FindLandmarkRuns();
            if (runs is null || !runs.Any())
            {
                return this.Error().ResourceNotFound("No Landmark runs have been triggered");
            }

            var userIds = runs.SelectMany(x => x.Scenarios)
                .Where(x => x.ExternalRunInfo?.CreatorId != null)
                .Select(x => x.ExternalRunInfo.CreatorId.Value)
                .Distinct()
                .ToList();
            var runTypeIds = runs.Select(x => x.RunTypeId).Distinct().ToList();

            var scenarioIds = runs.SelectMany(x => x.Scenarios)
                .Select(x => x.ScenarioId)
                .Distinct()
                .ToList();

            var users = _usersRepository.GetByIds(userIds);
            var runTypes = _runTypeRepository.GetByIds(runTypeIds);
            var scenarios = _scenarioRepository.FindByIds(scenarioIds);

            return Ok(runs.SelectMany(x =>
                x.Scenarios
                    .Where(y => y.ExternalRunInfo != null)
                    .Select(y =>
                    {
                        var externalRunInfo = y.ExternalRunInfo;

                        return new TriggeredLandmarkRunInfo
                        {
                            QueueName = externalRunInfo.QueueName,
                            RunName = x.Description,
                            RunType = runTypes.FirstOrDefault(type => type.Id == x.RunTypeId)?.Name,
                            ScenarioId = y.ScenarioId,
                            ScenarioName = scenarios.FirstOrDefault(scenario => scenario.Id == y.ScenarioId)?.Name,
                            DaySelected = externalRunInfo.ScheduledDateTime?.DayOfWeek.ToString(),
                            ScheduledDateTime = externalRunInfo.ScheduledDateTime,
                            Priority = externalRunInfo.Priority,
                            LandmarkStatus = externalRunInfo.ExternalStatus,
                            Comment = externalRunInfo.Comment,
                            Creator = users.FirstOrDefault(user => user.Id == externalRunInfo.CreatorId)?.Email,
                            CreatedDateTime = externalRunInfo.CreatedDateTime,
                            RunId = x.Id,
                            ExternalRunId = externalRunInfo.ExternalRunId
                        };
                    })
            ));
        }

        private IHttpActionResult BinaryResponse(byte[] content)
        {
            var responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
            responseMsg.Content = new ByteArrayContent(content);
            responseMsg.Content.Headers.Add("IsCompressedContent", "true");
            return new ResponseMessageResult(responseMsg);
        }
    }
}
