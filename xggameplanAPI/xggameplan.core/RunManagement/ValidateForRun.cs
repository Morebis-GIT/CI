using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Services;
using xggameplan.Model;
using xggameplan.Repository;

namespace xggameplan.RunManagement
{
    /// <summary>
    /// Performs validation for starting the run
    /// </summary>
    internal class ValidateForRun
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IISRSettingsRepository _isrSettingsRepository;
        private readonly IRSSettingsRepository _rsSettingsRepository;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IUniverseRepository _universeRepository;
        private readonly ISpotRepository _spotRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IRatingsScheduleRepository _ratingsScheduleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IClashRepository _clashRepository;
        private readonly ISystemMessageRepository _systemMessageRepository;
        private readonly IAutoBooks _autoBooks;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IClearanceRepository _clearanceRepository;
        private readonly IFeatureManager _featureManager;
        private readonly ISystemLogicalDateService _systemLogicalDateService;
        private const string _placeholderSalesAreaName = "{sales_area_name}";
        private const string _placeholderRunDescription = "{run_description}";
        private const string _placeholderCampaignName = "{campaign_name}";
        private const string _placeholderInvalidData = "{invalid_data}";
        private const string _placeholderMissingRating = "{missing_rating}";

        public ValidateForRun(IRepositoryFactory repositoryFactory,
                        IISRSettingsRepository isrSettingsRepository, IRSSettingsRepository rsSettingsRepository,
                        IOutputFileRepository outputFileRepository, IUniverseRepository universeRepository,
                        ISpotRepository spotRepository, IScheduleRepository scheduleRepository,
                        IRatingsScheduleRepository ratingsScheduleRepository, IProductRepository productRepository,
                        IClashRepository clashRepository,
                        ISystemMessageRepository systemMessageRepository,
                        IAutoBooks autoBooks, ITenantSettingsRepository tenantSettingsRepository, IClearanceRepository clearanceRepository,
                        IFeatureManager featureManager, ISystemLogicalDateService systemLogicalDateService)
        {
            _repositoryFactory = repositoryFactory;
            _isrSettingsRepository = isrSettingsRepository;
            _rsSettingsRepository = rsSettingsRepository;
            _outputFileRepository = outputFileRepository;
            _universeRepository = universeRepository;
            _spotRepository = spotRepository;
            _scheduleRepository = scheduleRepository;
            _ratingsScheduleRepository = ratingsScheduleRepository;
            _productRepository = productRepository;
            _clashRepository = clashRepository;
            _systemMessageRepository = systemMessageRepository;
            _autoBooks = autoBooks;
            _tenantSettingsRepository = tenantSettingsRepository;
            _clearanceRepository = clearanceRepository;
            _featureManager = featureManager;
            _systemLogicalDateService = systemLogicalDateService;
        }

        public List<SystemMessage> Validate(Run run)
        {
            var messages = new List<SystemMessage>();

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var repositories = scope.CreateRepositories(
                    typeof(ISalesAreaRepository),
                    typeof(ICampaignRepository),
                    typeof(IAutoBookInstanceConfigurationRepository),
                    typeof(IDemographicRepository),
                    typeof(IScheduleRepository),
                    typeof(ISmoothConfigurationRepository)
                );
                var salesAreaRepository = repositories.Get<ISalesAreaRepository>();
                var campaignRepository = repositories.Get<ICampaignRepository>();
                var autoBookInstanceConfigurationRepository = repositories.Get<IAutoBookInstanceConfigurationRepository>();
                var demographicRepository = repositories.Get<IDemographicRepository>();
                var scheduleRepository = repositories.Get<IScheduleRepository>();
                var smoothConfigurationRepository = repositories.Get<ISmoothConfigurationRepository>();

                // Validate run details, first check that run is well formed and
                // carry on if it pass this first method check
                messages.AddRange(ValidateRunDetailsForStartRun(run));
                if (messages.Count != 0)
                {
                    return messages;
                }

                // Get Sales areas for run and list of sales area names from
                // run.SalesAreaPriorities NOT set to 'Exclude'
                var runSalesAreas = new List<SalesArea>();
                var runSalesAreaNames = new List<string>();
                if (run.SalesAreaPriorities.Any())
                {
                    runSalesAreas = salesAreaRepository.FindByNames(run.SalesAreaPriorities.Where(s => s.Priority != SalesAreaPriorityType.Exclude).Select(s => s.SalesArea).ToList());
                    runSalesAreaNames = salesAreaRepository.GetListOfNames(runSalesAreas);
                }
                else
                {
                    // If there are NO entries in run.SalesAreaPriorities then
                    // get all sales are names from the repository of sales areas
                    runSalesAreaNames = salesAreaRepository.GetListOfNames(salesAreaRepository.GetAll().ToList());
                }

                // check that there are some sales areas from one of the two
                // sources above
                if (!runSalesAreaNames.Any())
                {
                    messages.Add(FormatSystemMessage(SystemMessage.SalesAreaDataMissing));

                    // only carry on with the run validation if there are some
                    // sales areas
                    return messages;
                }

                bool isOptimiserNeeded = run.Optimisation || run.RightSizer || run.ISR;

                var allAutoBookInstanceConfigurations = autoBookInstanceConfigurationRepository.GetAll().ToList();
                if (!allAutoBookInstanceConfigurations.Any())
                {
                    allAutoBookInstanceConfigurations = _autoBooks.CreateDefaultInstanceConfigurations();
                }

                var allCampaigns = campaignRepository.GetAllFlat().ToList();
                var runCampaigns = RunManager.GetRunCampaigns(run, allCampaigns);
                var demographics = demographicRepository.GetAll().ToList();

                // Validate RatingsData
                if (_featureManager.IsEnabled(nameof(ProductFeature.NineValidationRatingPredictions)))
                {
                    var demographicsGameplan = demographics.Where(g => g.Gameplan).Select(x => x.ExternalRef).ToList();

                    var noOfRatingPredictionsPerScheduleDayAreaDemo = _tenantSettingsRepository.Get()?.NoOfRatingsPerSalesDayDemo ?? 96;
                    messages.AddRange(ValidateMissingRatingsForStartRun(run.StartDate, run.EndDate, runSalesAreaNames, demographicsGameplan, noOfRatingPredictionsPerScheduleDayAreaDemo));
                }

                if (isOptimiserNeeded && allAutoBookInstanceConfigurations.Count > 1)   //single instance?
                {
                    // Get number of breaks
                    int breaks = RunManager.GetBreakCountEstimate(run, runSalesAreas, _repositoryFactory);

                    messages.AddRange(ValidateAutoBookResourcesForStartRun(run, allAutoBookInstanceConfigurations, runSalesAreas.Count, runCampaigns.Count, demographics.Count, breaks));
                }

                // Validate ISR settings
                messages.AddRange(ValidateISRSettingsForStartRun(run, runSalesAreas));

                // Validate RS settings
                messages.AddRange(ValidateRSSettingsForStartRun(run, runSalesAreas));

                // Validate Optimisation settings
                messages.AddRange(ValidateOptimisationSettingsForStartRun(run));

                // Validate Smooth configuration
                messages.AddRange(ValidateSmoothConfiguration(run, smoothConfigurationRepository));

                // Validate against existing runs
                messages.AddRange(ValidateExistingRunsForStartRun());

                // Validate schedule data (Breaks, spots, campaigns etc)
                messages.AddRange(ValidateScheduleDataForStartRun(run, runCampaigns, runSalesAreas, demographics.Count));

                // Validate demographics
                messages.AddRange(ValidateDemographicsForStartRun(demographics));

                // Validate output files, can't process output files if data is missing
                messages.AddRange(ValidateOutputFilesForStartRun(run));

                // Validate universes
                messages.AddRange(ValidateUniversesForStartRun(run, runSalesAreas));
            }

            return messages;
        }

        /// <summary>
        /// Returns formatted system message.
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        private SystemMessage FormatSystemMessage(int messageId) => FormatSystemMessage(messageId, null);

        /// <summary>
        /// Returns formatted system message with any placeholders replaced.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="placeholders"></param>
        /// <returns></returns>
        private SystemMessage FormatSystemMessage(int messageId, Dictionary<string, string> placeholders)
        {
            SystemMessage validateRunMessage = _systemMessageRepository.Get(messageId);

            var newValidateRunMessage = new SystemMessage(
                validateRunMessage.Id,
                validateRunMessage.MessageGroups,
                new Dictionary<string, string>(),
                validateRunMessage.Link,
                validateRunMessage.HttpCode);

            foreach (var language in validateRunMessage.Description.Keys)
            {
                newValidateRunMessage.Description.Add(language, validateRunMessage.Description[language]);
            }

            if (!(placeholders is null))
            {
                foreach (string placeholder in placeholders.Keys)
                {
                    foreach (var language in validateRunMessage.Description.Keys)
                    {
                        newValidateRunMessage.Description[language] = newValidateRunMessage
                            .Description[language]
                            .Replace(placeholder, placeholders[placeholder]);
                    }
                }
            }

            return newValidateRunMessage;
        }

        /// <summary>
        /// Validates resources needed for starting run and check there are some
        /// working autobooks of suitable size if auto provisioning is disabled
        /// </summary>
        /// <param name="run"></param>
        private List<SystemMessage> ValidateAutoBookResourcesForStartRun(Run run, List<AutoBookInstanceConfiguration> allAutoBookInstanceConfigurations, int salesAreas, int campaigns, int demographics, int breaks)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            var autoBookInstanceConfigurations = _autoBooks.GetInstanceConfigurationsAscByCost(run, allAutoBookInstanceConfigurations, salesAreas, campaigns, demographics, breaks);
            var autobookType = autoBookInstanceConfigurations.FirstOrDefault();  //cheapest, smallest by cost, the required type for this run

            if (!autoBookInstanceConfigurations.Any())
            {
                messages.Add(FormatSystemMessage(SystemMessage.NoAutoBooksBigEnoughForRun));
            }

            if (String.IsNullOrEmpty(_autoBooks.Settings.BinariesVersion))
            {
                messages.Add(FormatSystemMessage(SystemMessage.AutoBookSettingsBinariesVersionotSet));
            }

            if (!_autoBooks.Settings.AutoDistributed)
            {
                if (!_autoBooks.Settings.AutoProvisioning)   // Manual provisioning - autoprovisioning is off and autoDistributed is off
                {
                    // Check that there are some working AutoBooks, even if they're
                    // currently busy then run will be queued
                    if (!_autoBooks.WorkingAutoBooks.Any())
                    {
                        messages.Add(FormatSystemMessage(SystemMessage.NoWorkingAutoBooks));
                    }

                    else if (autobookType != null)
                    {
                        var workingAutoBooks = _autoBooks.WorkingAutoBooks.OrderBy(x => x.InstanceConfigurationId); //existing autobooks in ascending order
                        var workingOfCorrectType = workingAutoBooks.Where(w => w.InstanceConfigurationId >= autobookType.Id); //existing autobooks of required type
                        if (!workingOfCorrectType.Any())
                        {
                            messages.Add(FormatSystemMessage(SystemMessage.NoAutoBooksOfCorrectSizeAndNotAutoProvisioning));
                        }
                    }
                }
            }

            return messages;
        }

        /// <summary>
        /// Validate Smooth configuration
        /// </summary>
        /// <param name="run"></param>
        /// <param name="smoothConfigurationRepository"></param>
        /// <returns></returns>
        private List<SystemMessage> ValidateSmoothConfiguration(Run run, ISmoothConfigurationRepository smoothConfigurationRepository)
        {
            List<SystemMessage> messages = new List<SystemMessage>();

            if (run.Smooth)
            {
                // Try loading configuration
                SmoothConfiguration smoothConfiguration = null;
                try
                {
                    smoothConfiguration = smoothConfigurationRepository.GetById(SmoothConfiguration.DefaultId);
                    if (smoothConfiguration == null)    // Not found
                    {
                        messages.Add(FormatSystemMessage(SystemMessage.SmoothConfigurationMissing));
                    }
                }
                catch (System.Exception exception)   // Possible corruption caused by manual modification
                {
                    messages.Add(FormatSystemMessage(SystemMessage.SmoothConfigurationInvalid, new Dictionary<string, string>() { { _placeholderInvalidData, string.Format("Error loading Smooth configuration: {0}", exception.Message) } }));
                }

                // Validate Smooth configuration
                if (smoothConfiguration != null)
                {
                    SmoothConfigurationValidator smoothConfigurationValidator = new SmoothConfigurationValidator();
                    var validationResults = smoothConfigurationValidator.Validate(smoothConfiguration);
                    validationResults.ForEach(validationResult => messages.Add(FormatSystemMessage(SystemMessage.SmoothConfigurationInvalid, new Dictionary<string, string>() { { _placeholderInvalidData, validationResult } })));
                }
            }
            return messages;
        }

        /// <summary>
        /// Validates run details for starting run, checks that run is well formed
        /// </summary>
        /// <param name="run"></param>
        private List<SystemMessage> ValidateRunDetailsForStartRun(Run run)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            bool DEBUG = _tenantSettingsRepository.Get().Debug;    //boolean default value is false, so if not set in tenant setting then will be false

            if (run.IsTemplate)    // Cannot execute a template run
            {
                messages.Add(FormatSystemMessage(SystemMessage.NotAllowedToExecuteRun));

                return messages;
            }

            if (!DEBUG && run.RunStatus == RunStatus.NotStarted && run.StartDate.Date < _systemLogicalDateService.GetSystemLogicalDate().Date)
            {
                messages.Add(FormatSystemMessage(SystemMessage.RunDateRangeInvalid));
            }
            if (run.Scenarios.Count == 0)
            {
                messages.Add(FormatSystemMessage(SystemMessage.RunHasNoScenarios));
            }
            if (!run.Scenarios.Where(s => s.Status == ScenarioStatuses.Pending).Any())    // Scenarios should all be in Pending state
            {
                messages.Add(FormatSystemMessage(SystemMessage.RunAlreadyStarted));
            }
            if (!run.Optimisation && !run.Smooth && !run.ISR && !run.RightSizer)
            {
                messages.Add(FormatSystemMessage(SystemMessage.NoProcessingSpecified));
            }

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var scenarioRepository = scope.CreateRepository<IScenarioRepository>();
                var scenarioPassIds = scenarioRepository
                    .GetScenariosWithPassId(run.Scenarios.Select(c => c.Id))
                    .GroupBy(c => c.ScenarioId)
                    .ToDictionary(g => g.Key, g => g.Count());

                messages.AddRange(run.Scenarios
                    .Where(scenario => !scenarioPassIds.ContainsKey(scenario.Id) || scenarioPassIds[scenario.Id] == 0)
                    .Select(scenario => FormatSystemMessage(SystemMessage.RunScenarioHasNoPasses)));
            }

            return messages;
        }

        /// <summary>
        /// Validates ISR settings needed for starting run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        private List<SystemMessage> ValidateISRSettingsForStartRun(Run run, IEnumerable<SalesArea> salesAreas)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            if (!run.ISR)
            {
                return messages;
            }

            var isrSettingsSalesAreas = new HashSet<string>(_isrSettingsRepository
                .FindBySalesAreas(salesAreas.Select(x => x.Name))
                .Select(x => x.SalesArea));

            // Check settings exist
            messages.AddRange(salesAreas.Where(salesArea => !isrSettingsSalesAreas.Contains(salesArea.Name))
                .Select(salesArea => FormatSystemMessage(SystemMessage.NoISRSettingsForSalesArea, new Dictionary<string, string>() { { _placeholderSalesAreaName, salesArea.Name } })));

            return messages;
        }

        /// <summary>
        /// Validates Optimisation settings
        /// </summary>
        /// <param name="run"></param>
        private List<SystemMessage> ValidateOptimisationSettingsForStartRun(Run run)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            if (!run.Smooth && !run.Optimisation && !run.ISR && !run.RightSizer)
            {
                messages.Add(FormatSystemMessage(SystemMessage.NoProcessingSpecified));
            }
            return messages;
        }

        /// <summary>
        /// Validates RS settings
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        private List<SystemMessage> ValidateRSSettingsForStartRun(Run run, IEnumerable<SalesArea> salesAreas)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            if (!run.RightSizer)
            {
                return messages;
            }

            var rsSettingsSalesAreas = new HashSet<string>(_rsSettingsRepository
                .FindBySalesAreas(salesAreas.Select(x => x.Name))
                .Select(x => x.SalesArea));

            // Check settings exist
            messages.AddRange(salesAreas.Where(salesArea => !rsSettingsSalesAreas.Contains(salesArea.Name))
                .Select(salesArea => FormatSystemMessage(SystemMessage.NoRSSettingsForSalesArea, new Dictionary<string, string>() { { _placeholderSalesAreaName, salesArea.Name } })));

            return messages;
        }

        /// <summary>
        /// Validate existing runs for starting run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        private List<SystemMessage> ValidateExistingRunsForStartRun()
        {
            var messages = new List<SystemMessage>();

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var runRepository = scope.CreateRepository<IRunRepository>();

                if (runRepository.GetAllActive().Any())
                {
                    messages.Add(FormatSystemMessage(SystemMessage.AnotherRunIsScheduledOrActive));
                }
            }

            return messages;
        }

        /// <summary>
        /// Validate demograpghics for starting run
        /// </summary>
        /// <param name="run"></param>
        private List<SystemMessage> ValidateDemographicsForStartRun(List<Demographic> demographics)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            if (demographics == null || !demographics.Any())
            {
                messages.Add(FormatSystemMessage(SystemMessage.DemographicDataMissing));
            }
            return messages;
        }

        /// <summary>
        /// Validate output files for starting run. Only files defined in
        /// IOutputFilesRepository are processed.
        /// </summary>
        /// <param name="run"></param>
        private List<SystemMessage> ValidateOutputFilesForStartRun(Run run)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            var outputFiles = _outputFileRepository.GetAll();
            if (outputFiles == null || !outputFiles.Any())
            {
                messages.Add(FormatSystemMessage(SystemMessage.OutputFileDataMissing));
            }
            return messages;
        }

        /// <summary>
        /// Validate universes for starting run. There must be sufficient
        /// universe data for the full run date range.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        private List<SystemMessage> ValidateUniversesForStartRun(Run run, IEnumerable<SalesArea> salesAreas)
        {
            List<SystemMessage> messages = new List<SystemMessage>();
            var universes = _universeRepository.GetAll();
            foreach (var salesArea in salesAreas)
            {
                var universesForSalesArea = universes.Where(u => u.SalesArea == salesArea.Name).ToList();
                var universesForSalesAreaStart = universesForSalesArea.Where(u => u.SalesArea == salesArea.Name && run.StartDate.Date >= u.StartDate.Date && run.StartDate.Date <= u.EndDate.Date).ToList();
                var universesForSalesAreaEnd = universesForSalesArea.Where(u => u.SalesArea == salesArea.Name && run.EndDate.Date >= u.StartDate.Date && run.EndDate.Date <= u.EndDate.Date).ToList();
                if (universesForSalesArea.Count == 0)    // No universes for sales areas
                {
                    messages.Add(FormatSystemMessage(SystemMessage.UniverseDataMissingForSalesArea, new Dictionary<string, string>() { { _placeholderSalesAreaName, salesArea.Name } }));
                }
                else if (!universesForSalesAreaStart.Any() || !universesForSalesAreaEnd.Any())   // Some universes for sales area but some missing for this run
                {
                    messages.Add(FormatSystemMessage(SystemMessage.UniverseDataMissingForDateRangeAndSalesArea, new Dictionary<string, string>() { { _placeholderSalesAreaName, salesArea.Name } }));
                }

                messages.ForEach(x => x.SeverityLevel = SystemMessageSeverityLevel.Warning);
            }
            return messages;
        }

        /// <summary>
        /// Validates the schedule data needed (i.e. Breaks, spots etc) for
        /// starting run.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="campaigns"></param>
        /// <param name="salesAreas"></param>
        /// <param name="demographicsCount"></param>
        private List<SystemMessage> ValidateScheduleDataForStartRun(Run run, IReadOnlyCollection<CampaignReducedModel> campaigns, IEnumerable<SalesArea> salesAreas, int demographicsCount)
        {
            List<SystemMessage> messages = new List<SystemMessage>();

            var tenantSettings = _tenantSettingsRepository.Get();
            var minDocumentRestriction = tenantSettings?.RunRestrictions?.MinDocRestriction;
            var minRunSizeDocumentRestriction = tenantSettings?.RunRestrictions?.MinRunSizeDocRestriction;
            if (minDocumentRestriction == null || minRunSizeDocumentRestriction == null)
            {
                messages.Add(FormatSystemMessage(SystemMessage.RunRestrictionsMissing, null));
                return messages;
            }

            var daysCount = (run.EndDate - run.StartDate).Days + 1;

            if (_featureManager.IsEnabled(nameof(ProductFeature.NineValidationMinSpot)))
            {
                var minSpotDocument = minRunSizeDocumentRestriction.Spots * daysCount * salesAreas.Count();
                if (_spotRepository.Count(x => x.StartDateTime >= run.StartDate && x.StartDateTime <= run.EndDate) < minSpotDocument)
                {
                    messages.Add(FormatSystemMessage(SystemMessage.SpotDataMissing, null));
                }
            }

            var countBreaksAndProgrammes = _scheduleRepository.CountBreaksAndProgrammes(run.StartDate, run.EndDate);

            var minBreakDocument = minRunSizeDocumentRestriction.Breaks * daysCount * salesAreas.Count();
            if (countBreaksAndProgrammes.breaksCount < minBreakDocument)
            {
                messages.Add(FormatSystemMessage(SystemMessage.BreakDataMissing, null));
            }

            var minProgrammeDocuments = minRunSizeDocumentRestriction.Programmes * daysCount * salesAreas.Count();
            if (countBreaksAndProgrammes.programmesCount < minProgrammeDocuments)
            {
                messages.Add(FormatSystemMessage(SystemMessage.ProgrammeDataMissing, null));
            }

            if (campaigns.Count < minDocumentRestriction.Campaigns)
            {
                messages.Add(FormatSystemMessage(SystemMessage.CampaignDataMissing, null));
            }
            else
            {
                // Check that campaign CustomId is set
                var campaignsWithInvalidCustomId = campaigns.Where(c => c.CustomId == 0);
                foreach (var campaignWithInvalidCustomId in campaignsWithInvalidCustomId)
                {
                    messages.Add(FormatSystemMessage(SystemMessage.CampaignDataInvalid, new Dictionary<string, string>() { { _placeholderCampaignName, campaignWithInvalidCustomId.Name }, { _placeholderInvalidData, "Custom ID is not set" } }));
                }

                // Check that campaign CustomId is unique
                var campaignsWithCustomId = campaigns.Where(c => c.CustomId > 0).ToList();
                var campaignCustomIdsGrouped = campaignsWithCustomId
                    .GroupBy(c => c.CustomId)
                    .ToDictionary(x => x.Key, x => x.Count());

                foreach (var campaign in campaignsWithCustomId)
                {
                    if (!campaignCustomIdsGrouped.ContainsKey(campaign.CustomId) || campaignCustomIdsGrouped[campaign.CustomId] > 1)
                    {
                        messages.Add(FormatSystemMessage(SystemMessage.CampaignDataInvalid, new Dictionary<string, string>() { { _placeholderCampaignName, campaign.Name }, { _placeholderInvalidData, "Custom ID is not unique" } }));
                    }
                }
            }

            if (_clearanceRepository.CountAll < minDocumentRestriction.ClearanceCodes)
            {
                messages.Add(FormatSystemMessage(SystemMessage.ClearanceDataMissing, null));
            }

            if (_clashRepository.CountAll < minDocumentRestriction.Clashes)
            {
                messages.Add(FormatSystemMessage(SystemMessage.ClashDataMissing, null));
            }

            if (demographicsCount < minDocumentRestriction.Demographics)
            {
                messages.Add(FormatSystemMessage(SystemMessage.DemographicDataMissing, null));
            }

            if (_productRepository.CountAll < minDocumentRestriction.Products)
            {
                messages.Add(FormatSystemMessage(SystemMessage.ProductDataMissing, null));
            }

            var campaignProductRefs = campaigns
                .Where(c => !string.IsNullOrEmpty(c.Product))
                .Select(c => c.Product)
                .Distinct()
                .ToList();

            var existingProducts = _productRepository
                .FindByExternal(campaignProductRefs)
                .ToDictionary(x => x.Externalidentifier, x => x);

            var productClashCodes = existingProducts
                .Where(p => !string.IsNullOrEmpty(p.Value.ClashCode))
                .Select(p => p.Value.ClashCode)
                .Distinct()
                .ToList();

            var existingClashCodes = new HashSet<string>(_clashRepository
                .FindByExternal(productClashCodes)
                .Select(c => c.Externalref));

            foreach (var campaign in campaigns)
            {
                if (string.IsNullOrEmpty(campaign.Product))
                {
                    messages.Add(FormatSystemMessage(SystemMessage.CampaignDataInvalid,
                        new Dictionary<string, string>
                        {
                            {_placeholderCampaignName, campaign.ExternalId},
                            {_placeholderInvalidData, "Campaign is missing a Product"}
                        }));

                    continue;
                }

                var product = existingProducts.ContainsKey(campaign.Product) ? existingProducts[campaign.Product] : null;
                if (product is null)
                {
                    messages.Add(FormatSystemMessage(SystemMessage.CampaignDataInvalid,
                        new Dictionary<string, string>
                        {
                            {_placeholderCampaignName, campaign.ExternalId},
                            {_placeholderInvalidData, $"Product: {campaign.Product} is missing"}
                        }));
                }
                else if (!string.IsNullOrEmpty(product.ClashCode) && !existingClashCodes.Contains(product.ClashCode))
                {
                    messages.Add(FormatSystemMessage(SystemMessage.CampaignDataInvalid,
                        new Dictionary<string, string>
                        {
                            {_placeholderCampaignName, campaign.ExternalId},
                            {_placeholderInvalidData, $"Product: {product.Externalidentifier}, Clash Code: {product.ClashCode} is missing"}
                        }));
                }
            }

            return messages;
        }

        /// <summary>
        /// Validates missing ratings for starting run.
        /// </summary>
        /// <param></param>
        private List<SystemMessage> ValidateMissingRatingsForStartRun(DateTime fromDateTime, DateTime toDateTime,
            IEnumerable<string> salesAreaNames, IEnumerable<string> demographics,
            int noOfRatingPredictionsPerScheduleDayAreaDemo)
        {
            var messages = new List<SystemMessage>();

            var results = _ratingsScheduleRepository.Validate_RatingsPredictionSchedules(fromDateTime, toDateTime, salesAreaNames, demographics, noOfRatingPredictionsPerScheduleDayAreaDemo);

            foreach (var error in results.Where(x => x.SeverityLevel == ValidationSeverityLevel.Error))
            {
                messages.Add(FormatSystemMessage(SystemMessage.RatingsPredictionDataMissing, new Dictionary<string, string>() { { _placeholderMissingRating, error.Message } }));
            }

            foreach (var warning in results.Where(x => x.SeverityLevel == ValidationSeverityLevel.Warning))
            {
                var systemMessage = FormatSystemMessage(SystemMessage.RatingsPredictionDataMissing, new Dictionary<string, string>() { { _placeholderMissingRating, warning.Message } });

                systemMessage.SeverityLevel = SystemMessageSeverityLevel.Warning;

                messages.Add(systemMessage);
            }

            return messages;
        }

        /// <summary>
        /// Validates run's stard and end dates for starting run.
        /// </summary>
        /// <param name="run"></param>
        private List<SystemMessage> ValidateRunStartAndEndDateForStartRun(Run run)
        {
            List<SystemMessage> messages = new List<SystemMessage>();

            if (run.RunStatus == RunStatus.NotStarted && run.StartDate < _systemLogicalDateService.GetSystemLogicalDate().Date)
            {
                messages.Add(FormatSystemMessage(SystemMessage.RunDateRangeInvalid));
            }

            return messages;
        }
    }
}
