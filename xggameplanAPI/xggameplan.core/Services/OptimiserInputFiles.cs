using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgCampaigns;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoGen.AgDataPopulation;
using xggameplan.core.Extensions;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Helpers;
using xggameplan.core.Services.OptimiserInputFilesSerialisers;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces;
using xggameplan.Extensions;
using xggameplan.model.AutoGen.AgInventoryLocks;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Services
{
    public class OptimiserInputFiles
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _applicationConfiguration;
        private readonly IClashExceptionSerializer _clashExceptionSerializer;
        private readonly ICampaignSerializer _campaignSerializer;
        private readonly IBreakSerializer _breakSerializer;
        private readonly IProgTxDetailSerializer _progTxDetailSerializer;
        private readonly IFeatureManager _featureManager;
        private readonly ISystemLogicalDateService _systemLogicalDateService;

        private void RaiseInfo(string message) =>
            _auditEventRepository.Insert(
                AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, message)
                );

        public OptimiserInputFiles(
            IRepositoryFactory repositoryFactory,
            IAuditEventRepository auditEventRepository,
            IConfiguration applicationConfiguration,
            IClashExceptionSerializer clashExceptionSerializer,
            ICampaignSerializer campaignSerializer,
            IBreakSerializer breakSerializer,
            IProgTxDetailSerializer progTxDetailSerializer,
            IFeatureManager featureManager,
            IMapper mapper,
            ISystemLogicalDateService systemLogicalDateService)
        {
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
            _applicationConfiguration = applicationConfiguration;
            _clashExceptionSerializer = clashExceptionSerializer;
            _campaignSerializer = campaignSerializer;
            _breakSerializer = breakSerializer;
            _progTxDetailSerializer = progTxDetailSerializer;
            _featureManager = featureManager;
            _mapper = mapper;
            _systemLogicalDateService = systemLogicalDateService;
        }

        public string RootFolder { get; set; } = "";

        /// <summary>
        /// Populate run data (i.e. Shared data) for dynamic xml generation
        /// </summary>
        /// <param name="run"></param>
        /// <returns>Compressed file path</returns>
        public string PopulateRunData(Run run)
        {
            var folderName = $@"{RootFolder}\Temp\Run\{run.Id}";
            var zipPath = folderName + ".zip";

            if (File.Exists(zipPath))
            {
                // If Run data available then skip the XML population. This
                // happens when multiple scenarios are used.
                return zipPath;
            }

            InitialiseFolder(folderName);

            List<Demographic> demographic = null;
            List<ProgrammeCategoryHierarchy> programmeCategories = null;
            List<SalesArea> allSalesAreas = null;
            List<Sponsorship> allSponsorships = null;
            List<ProgrammeEpisode> allProgrammesEpisodes = null;

            var prioritySalesAreas = new List<SalesArea>();

            IAutoBookDefaultParameters autoBookDefaultParameters = null;

            string errorMessage = "";

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var demographicRepository = scope.CreateRepository<IDemographicRepository>();
                var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();
                var clashRepository = scope.CreateRepository<IClashRepository>();
                var campaignRepository = scope.CreateRepository<ICampaignRepository>();
                var businessTypeRepository = scope.CreateRepository<IBusinessTypeRepository>();
                var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
                var tenantSettingsRepository = scope.CreateRepository<ITenantSettingsRepository>();
                var universeRepository = scope.CreateRepository<IUniverseRepository>();
                var restrictionRepository = scope.CreateRepository<IRestrictionRepository>();
                var productRepository = scope.CreateRepository<IProductRepository>();
                var isrSettingsRepository = scope.CreateRepository<IISRSettingsRepository>();
                var isrGlobalSettingsRepository = scope.CreateRepository<IISRGlobalSettingsRepository>();
                var rsSettingsRepository = scope.CreateRepository<IRSSettingsRepository>();
                var rsGlobalSettingsRepository = scope.CreateRepository<IRSGlobalSettingsRepository>();
                var sponsorshipRepository = scope.CreateRepository<ISponsorshipRepository>();
                var programmeRepository = scope.CreateRepository<IProgrammeRepository>();
                var programmeCategoryRepository = scope.CreateRepository<IProgrammeCategoryHierarchyRepository>();
                var autoBookDefaultParametersRepository = scope.CreateRepository<IAutoBookDefaultParametersRepository>();
                var programmeEpisodeRepository = scope.CreateRepository<IProgrammeEpisodeRepository>();
                var scenarioRepository = scope.CreateRepository<IScenarioRepository>();

                try
                {
                    #region Prerequisite

                    autoBookDefaultParameters = autoBookDefaultParametersRepository.Get();

                    demographic = demographicRepository.GetAll().ToList();
                    programmeCategories = programmeCategoryRepository.GetAll().ToList();
                    RaiseInfo($"{demographic.Count} demographics and {programmeCategories.Count} programme categories loaded. Current Time - {DateTime.UtcNow.ToString()}");

                    var programmes = programmeRepository.GetAll().ToList();
                    RaiseInfo($"{programmes.Count} programme loaded. Current Time - {DateTime.UtcNow.ToString()}");

                    var runStartDate = run.StartDate.Add(run.StartTime);
                    var runEndDate = DateHelper.ConvertBroadcastToStandard(run.EndDate, run.EndTime);

                    #endregion Prerequisite

                    errorMessage = "Error generating Sales Area List file";

                    RaiseInfo($"Getting the sales area for processing. Current Time - {DateTime.UtcNow.ToString()}");

                    allSalesAreas = salesAreaRepository.GetAll().ToList();
                    allSponsorships = sponsorshipRepository.GetAll().ToList();
                    allProgrammesEpisodes = programmeEpisodeRepository.GetAll().ToList();

                    if (run.SalesAreaPriorities.Count == 0) // All sales areas
                    {
                        prioritySalesAreas.AddRange(allSalesAreas);
                    }
                    else
                    {
                        foreach (var sap in run.SalesAreaPriorities.Where(sap => sap.Priority != SalesAreaPriorityType.Exclude))
                        {
                            SalesArea sa = allSalesAreas.Find(x => x.Name == sap.SalesArea);
                            if (sa is null)
                            {
                                continue;
                            }

                            prioritySalesAreas.Add(sa);
                        }
                    }

                    if (prioritySalesAreas.Count == 0)
                    {
                        throw new Exception("No sales areas mentioned in run data");
                    }

                    var prioritySalesAreaNames = prioritySalesAreas.Select(s => s.Name).ToList();

                    DayOfWeek startDayOfWeek = tenantSettingsRepository.GetStartDayOfWeek();

                    RaiseInfo($"Getting the programmes for processing. Current Time - {DateTime.UtcNow.ToString()}");
                    var programs = scheduleRepository.GetProgrammes(
                        prioritySalesAreaNames,
                        run.StartDate.Date.StartAndEndOfWeekDate(startDayOfWeek).startDate,
                        runEndDate.Date.StartAndEndOfWeekDate(startDayOfWeek).endDate);

                    var campaignExternalIds = run.Campaigns?.Where(s => !string.IsNullOrWhiteSpace(s.ExternalId))
                        .Select(s => s.ExternalId).ToList();

                    RaiseInfo($"Getting the campaigns for processing. Current Time - {DateTime.UtcNow.ToString()}");
                    List<Campaign> campaigns = campaignRepository.FindByRefs(campaignExternalIds)?.ToList();
                    if (campaigns?.Count == 0)
                    {
                        campaigns.AddRange(campaignRepository.GetAllActive().ToList());
                    }

                    campaigns = OptimiserInputFilesHelper.FilterOutExcludedCampaigns(scenarioRepository, run, campaigns);

                    var campaignsWithRestrictions = campaigns?
                        .Where(c => c.ProgrammeRestrictions != null && c.ProgrammeRestrictions.Any()).ToList();

                    errorMessage = "Error generating Break List file";

                    RaiseInfo($"Getting the breaks for processing. Current Time - {DateTime.UtcNow.ToString()}");

                    _breakSerializer.Serialize(
                        folderName,
                        run,
                        prioritySalesAreas,
                        demographic,
                        programmeCategories,
                        programs,
                        campaignsWithRestrictions,
                        autoBookDefaultParameters,
                        out var breaks,
                        out var programmeDictionary);

                    if (breaks == null || breaks.Count == 0)
                    {
                        throw new Exception("No breaks available to export");
                    }

                    var allClashes = clashRepository.GetAll().ToList();

                    var excludeBreakType = _applicationConfiguration["AutoBooks:ExcludeBreakType"];

                    var spots = GetSpots(
                            prioritySalesAreas.Select(sa => sa.Name).ToList().AsReadOnly(),
                            run.StartDate,
                            runEndDate,
                            breaks,
                            excludeBreakType,
                            _repositoryFactory)
                        .ToList();

                    var allProducts = productRepository
                        .GetAll()?
                        .ToList();

                    IList<string> campaignProductCodes = campaigns.ConvertAll(c => c.Product);
                    var filteredProducts = new List<Product>();
                    if (campaignProductCodes.Count > 0)
                    {
                        filteredProducts = allProducts
                            .Where(p => campaignProductCodes.Contains(p.Externalidentifier))
                            .ToList();
                    }

                    var businessTypes = businessTypeRepository.GetAll().ToList();

                    Parallel.Invoke(
                        () => SerialiseSalesAreaPointers(folderName, allSalesAreas, demographic, _mapper, RaiseInfo),
                        () => SerialiseSalesAreaReferences(folderName, allSalesAreas, _mapper, RaiseInfo),
                        () => SerialiseSponsorshipExclusivity(folderName, allSalesAreas, allSponsorships, programs, programmeDictionary, _mapper, RaiseInfo),
                        () => SerialiseSpots(folderName, prioritySalesAreas, autoBookDefaultParameters, allClashes, breaks, campaigns, demographic, businessTypes, spots, allProducts, _featureManager, RaiseInfo),
                        () => SerializeBusinessTypes(folderName, businessTypes, RaiseInfo)
                    );

                    #region Exposure Details Export

                    errorMessage = "Error generating Clash Exposure Details file";

                    bool exportExposureDetails = Convert.ToBoolean(_applicationConfiguration["AutoBooks:ExportExposureDetails"]);
                    if (exportExposureDetails)
                    {
                        if (allClashes.Count > 0)
                        {
                            try
                            {
                                allClashes.ForEach(c => Clash.Validation(c.Externalref, c.Description, c.DefaultPeakExposureCount, c.DefaultOffPeakExposureCount));
                                ClashHelper.ValidateClashes(allClashes);
                            }
                            catch
                            {
                                errorMessage = "Invalid clash details";
                                throw;
                            }

                            TenantSettings tenantSettings = tenantSettingsRepository.Get();

                            SerialiseClashExposureDetails(run, folderName, allSalesAreas, autoBookDefaultParameters, allClashes, tenantSettings, _mapper, RaiseInfo);
                        }
                        else
                        {
                            throw new Exception("No clashes/exposure details available to export");
                        }

                        #region Clash Exceptions

                        errorMessage = "Error generating Clash Exceptions List file";
                        _clashExceptionSerializer.Serialize(folderName, run, allClashes, filteredProducts);

                        #endregion Clash Exceptions
                    }
                    else
                    {
                        RaiseInfo($"ExportExposureDetails is turned off. So v_exposure_dtls.xml & {_clashExceptionSerializer.Filename} are not populated. Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion Exposure Details Export

                    #region Break Program Campaign and Restriction Export

                    errorMessage = "Error generating Programme List file";
                    if (programs != null && programs.Count > 0)
                    {
                        RaiseInfo($"Started populating v_rep_prgt_list.xml. Total programmes - {programs.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                        programs.ToAgProgramme(prioritySalesAreas, programmeDictionary, startDayOfWeek, _mapper)
                            .Serialize($"{folderName}//v_rep_prgt_list.xml");

                        RaiseInfo($"Finished populating v_rep_prgt_list.xml. Total programmes - {programs.Count}, Current Time - {DateTime.UtcNow.ToString()}");
                    }
                    else
                    {
                        throw new Exception("No programmes available to export");
                    }

                    IEnumerable<Restriction> allRestrictions = restrictionRepository.GetAll() ??
                        Enumerable.Empty<Restriction>();
                    if (allRestrictions.Any())
                    {
                        SerializerRestrictions(
                            folderName,
                            programmeCategories,
                            allSalesAreas,
                            autoBookDefaultParameters,
                            allRestrictions,
                            runStartDate.Date,
                            runEndDate,
                            ref programs,
                            programmeDictionary,
                            allClashes,
                            campaigns,
                            filteredProducts,
                            out errorMessage);
                    }
                    else
                    {
                        RaiseInfo($"No restrictions available to export, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    errorMessage = "Error generating Campaign List file";
                    IReadOnlyCollection<Tuple<int, int, SalesAreaGroup>> channelGroup = null;
                    List<AgCampaignInclusion> campaignIncludeFunctions = null;

                    RemoveInvalidCampaign(ref campaigns);
                    if (campaigns?.Count > 0)
                    {
                        _campaignSerializer.Serialize(
                            folderName,
                            run,
                            campaigns,
                            businessTypes,
                            demographic,
                            allSalesAreas,
                            programmeDictionary,
                            programmeCategories,
                            autoBookDefaultParameters,
                            out channelGroup,
                            out campaignIncludeFunctions);

                        RaiseInfo($"Started populating v_camp_incl_list.xml. Total campaigns - {campaigns.Count.ToString()}, Current Time - {DateTime.UtcNow.ToString()}");

                        campaignIncludeFunctions.ToAgCampaignInclusionsList()
                            .Serialize($"{folderName}//v_camp_incl_list.xml");

                        RaiseInfo($"Finished populating v_camp_incl_list.xml. Total campaigns - {campaigns.Count.ToString()}, Current Time - {DateTime.UtcNow.ToString()}");

                        RaiseInfo($"Started populating v_cend_list.xml. Total campaign break requirements - {campaigns.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                        var breakRequirements = campaigns.Select(x => x.BreakRequirement).Where(x => x != null);

                        var salesAreas = salesAreaRepository.FindByShortNames(
                            breakRequirements
                                .Select(x => x.SalesArea)
                                .Where(x => !string.IsNullOrEmpty(x))
                        );
                        campaigns
                            .ToAgCampaignBreakRequirement(salesAreas.ToList())?
                            .Serialize($"{folderName}//v_cend_list.xml");

                        RaiseInfo($"Finished populating v_cend_list.xml. Total campaign break requirements - {campaigns.Count}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    if (campaigns == null || !campaigns.Any())
                    {
                        // Log all campaigns for diagnostics
                        var allCampaigns = campaignRepository.GetAll().ToList();
                        allCampaigns.ForEach(campaign =>
                            RaiseInfo(string.Format(
                                    "Campaign: Id={0}, Name={1}, Status={2}, TargetRatings={3}, CampaignSalesAreaTarget={4}",
                                    campaign.Id, campaign.Name, campaign.Status, campaign.TargetRatings,
                                    campaign.SalesAreaCampaignTarget == null || !campaign.SalesAreaCampaignTarget.Any()
                                        ? "0"
                                        : campaign.SalesAreaCampaignTarget.Count.ToString())
                                )
                            );

                        throw new Exception("No campaigns available to export");
                    }

                    #endregion Break Program Campaign and Restriction Export

                    #region Campaign Channel Group Export

                    if (_featureManager.IsEnabled(nameof(ProductFeature.IncludeChannelGroupFileForOptimiser)))
                    {
                        if (channelGroup != null && channelGroup.Any())
                        {
                            RaiseInfo($"Started populating v_camp_channel_group_list.xml. Total channel group - {channelGroup.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                            channelGroup.ToAgChannelGroup(allSalesAreas, _mapper)
                                .Serialize($"{folderName}//v_camp_channel_group_list.xml");

                            RaiseInfo($"Finished populating v_camp_channel_group_list.xml. Total channel group - {channelGroup.Count}, Current Time - {DateTime.UtcNow.ToString()}");
                        }
                        else
                        {
                            throw new Exception("No campaign channel group available to export");
                        }
                    }

                    #endregion Campaign Channel Group Export

                    #region Campaign BreakTypes Export

                    errorMessage = "Error generating Campaign Break Types file";

                    RaiseInfo($"Getting the campaigns break types for processing. Current Time - {DateTime.UtcNow.ToString()}");

                    var agCampignBreakTypes =
                        campaigns.ToAgCampaignBreakTypes(_mapper, out int totalCampaignBreakTypes);
                    if (totalCampaignBreakTypes != 0 && agCampignBreakTypes != null)
                    {
                        RaiseInfo($"Started populating v_cmbt_list.xml. Total Campaign Break Types - {totalCampaignBreakTypes}, Current Time - {DateTime.UtcNow.ToString()}");

                        agCampignBreakTypes.Serialize($"{folderName}//v_cmbt_list.xml");

                        RaiseInfo($"Finished populating v_cmbt_list.xml. Total Campaign Break Types - {totalCampaignBreakTypes}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion Campaign BreakTypes Export

                    #region Time Restriction Export

                    errorMessage = "Error generating Campaign Time Restrictions file";

                    RaiseInfo($"Getting the campaigns time restriction for processing. Current Time - {DateTime.UtcNow.ToString()}");

                    var agTimeRestriction =
                        campaigns.ToAgTimeRestriction(allSalesAreas, _mapper, out int totalTimeRestrictions);
                    if (totalTimeRestrictions != 0 && agTimeRestriction != null)
                    {
                        RaiseInfo($"Started populating v_treq_list.xml. Total Time Restriction - {totalTimeRestrictions}, Current Time - {DateTime.UtcNow.ToString()}");

                        agTimeRestriction.Serialize($"{folderName}//v_treq_list.xml");

                        RaiseInfo($"Finished populating v_treq_list.xml. Total Time Restriction - {totalTimeRestrictions}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion Time Restriction Export

                    #region Programmme Restriction Export

                    errorMessage = "Error generating Programme Restrictions file";
                    if (campaignsWithRestrictions.Count > 0)
                    {
                        var totalProgrammeRestrictions =
                            campaignsWithRestrictions.Sum(c => c.ProgrammeRestrictions.Count);

                        RaiseInfo($"Started populating v_preq_list.xml. Total Programme Restriction - {totalProgrammeRestrictions}, Current Time - {DateTime.UtcNow.ToString()}");

                        var agProgramRestrictions = campaignsWithRestrictions.ToAgProgrammesSerialization(allSalesAreas,
                            programmeCategories, allProgrammesEpisodes, programmeDictionary, autoBookDefaultParameters.AgProgRestriction);

                        if (agProgramRestrictions != null)
                        {
                            agProgramRestrictions.Serialize($"{folderName}//v_preq_list.xml");
                        }

                        RaiseInfo($"Finished populating v_preq_list.xml. Total Programme Restriction - {totalProgrammeRestrictions}, Current Time - {DateTime.UtcNow.ToString()}");

                        campaignsWithRestrictions.DisposeAll();
                        campaignsWithRestrictions.Clear();
                    }

                    #endregion Programmme Restriction Export

                    #region Week Export

                    errorMessage = "Error generating Week List file";

                    RaiseInfo($"Getting the week details for processing. Current Time - {DateTime.UtcNow.ToString()}");

                    var weekset = GetWeekData(run.StartDate.Date, runEndDate.Date, startDayOfWeek);
                    if (weekset != null && weekset.Count > 0)
                    {
                        RaiseInfo($"Started populating v_week_list.xml. Total Weeks - {weekset.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                        weekset.ToAgWeek(_mapper).Serialize($"{folderName}//v_week_list.xml");

                        RaiseInfo($"Finished populating v_week_list.xml. Total Weeks - {weekset.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                        weekset.DisposeAll();
                    }
                    else
                    {
                        throw new Exception("No week information available to export");
                    }

                    #endregion Week Export

                    #region Universe Export

                    errorMessage = "Error generating Universe List file";

                    RaiseInfo($"Getting the universes for processing. Current Time - {DateTime.UtcNow.ToString()}");

                    // add universe
                    var universes = universeRepository.GetAll();
                    if (universes != null && universes.Any())
                    {
                        RaiseInfo($"Started populating v_univ_list.xml. Total universes - {universes.Count()}, Current Time - {DateTime.UtcNow.ToString()}");

                        allSalesAreas.ToAgUniverse(universes.ToList(), demographic, _mapper)
                            .Serialize($"{folderName}//v_univ_list.xml");

                        RaiseInfo($"Finished populating v_univ_list.xml. Total universes - {universes.Count()}, Current Time - {DateTime.UtcNow.ToString()}");

                        universes.DisposeAll();
                    }
                    else
                    {
                        throw new Exception("No universes available to export");
                    }

                    #endregion Universe Export

                    #region public Holiday Export

                    errorMessage = "Error generating Public Holiday List file";

                    RaiseInfo($"Started populating v_phol_list.xml. Total sales areas - {allSalesAreas.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                    // add public holiday
                    allSalesAreas.ToAgPublicHolidays(run.StartDate.Date, runEndDate.Date)
                        .Serialize($"{folderName}//v_phol_list.xml");

                    RaiseInfo($"Finished populating v_phol_list.xml. Total sales areas - {allSalesAreas.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                    #endregion public Holiday Export

                    #region School Holiday Export

                    errorMessage = "Error generating School Holiday List file";

                    RaiseInfo($"Started populating v_shol_list.xml. Total sales areas - {allSalesAreas.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                    // add school holiday
                    allSalesAreas.ToAgSchoolHolidays(run.StartDate.Date, runEndDate.Date)
                        .Serialize($"{folderName}//v_shol_list.xml");

                    RaiseInfo($"Finished populating v_shol_list.xml. Total sales areas - {allSalesAreas.Count}, Current Time - {DateTime.UtcNow.ToString()}");

                    #endregion School Holiday Export

                    SerializeFacilities(scope, folderName, out errorMessage);

                    SerializeCampaignBookingPositions(scope, folderName, campaigns, allSalesAreas, out errorMessage);

                    SerializeDeliveryCappingData(scope, folderName, run, campaigns, out errorMessage);

                    SerializeSalesAreaDemographics(scope, folderName, allSalesAreas, demographic, out errorMessage);

                    SerializeTotalRatings(scope, folderName, demographic, allSalesAreas, run.StartDate, run.EndDate, out errorMessage);

                    SerializeStandardDayParts(scope, allSalesAreas, folderName, out errorMessage);

                    SerializeStandardDayPartGroups(scope, allSalesAreas, demographic, folderName, out errorMessage);

                    SerializeSpotBookingRules(scope, folderName, allSalesAreas, out errorMessage);

                    SerializeInventoryStatuses(folderName, run.ExcludedInventoryStatuses, out errorMessage);

                    SerializeInventoryLocks(scope, folderName, allSalesAreas, out errorMessage);

                    SerializeLengthFactorData(scope, folderName, allSalesAreas, out errorMessage);

                    errorMessage = "Error generating Spot List file";

                    campaigns.DisposeAll();
                    spots.DisposeAll();
                    allProducts.DisposeAll();
                    allClashes.DisposeAll();

                    #region ISR Parameters

                    var salesAreaList = allSalesAreas.ToList();

                    errorMessage = "Error generating ISR Parameters file";
                    if (run.ISR)
                    {
                        var isrSettingsList = isrSettingsRepository
                            .GetAll()
                            .Where(s => salesAreaList.FindIndex(sa => sa.Name == s.SalesArea) != -1)
                            .ToList();

                        var isrGlobalSettings = isrGlobalSettingsRepository.Get();

                        RaiseInfo($"Started populating v_isr_list.xml. ISR Parameters, Current Time - {DateTime.UtcNow.ToString()}");

                        // add ISR params
                        allSalesAreas.ToAgISRParams(
                            run,
                            demographic,
                            excludeBreakType,
                            isrSettingsList,
                            isrGlobalSettings,
                            autoBookDefaultParameters.AgISRTimeBand)
                            .Serialize($"{folderName}//v_isr_list.xml");

                        RaiseInfo($"Finished populating v_isr_list.xml. ISR Parameters, Current Time - {DateTime.UtcNow.ToString()}");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_isr_list.xml because ISR is disabled for the run. ISR Parameters, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion ISR Parameters

                    #region Right Sizer Parameters

                    if (run.RightSizer)
                    {
                        var rightSizerSettingsList = rsSettingsRepository.GetAll()
                            .Where(s => salesAreaList.FindIndex(sa => sa.Name == s.SalesArea) != -1).ToList();

                        var rsGlobalSettings = rsGlobalSettingsRepository.Get();

                        RaiseInfo($"Started populating v_rs_list.xml. RS Parameters, Current Time - {DateTime.UtcNow.ToString()}");

                        // add ISR params
                        allSalesAreas.ToAgRSParams(demographic, rightSizerSettingsList, rsGlobalSettings).Serialize($"{folderName}//v_rs_list.xml");

                        RaiseInfo($"Finished populating v_rs_list.xml. RS Parameters, Current Time - {DateTime.UtcNow.ToString()}");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_rs_list.xml because RS is disabled for the run. Right Sizer Parameters, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion Right Sizer Parameters

                    #region Process Date Parameters List

                    var processesRanges = new List<AgProcessRange>();

                    if (run.Smooth)
                    {
                        processesRanges.Add(AgProcessRange.Create(run, RunStepEnum.Smooth));
                    }

                    if (run.ISR)
                    {
                        processesRanges.Add(AgProcessRange.Create(run, RunStepEnum.ISR));
                    }

                    if (run.RightSizer)
                    {
                        processesRanges.Add(AgProcessRange.Create(run, RunStepEnum.RightSizer));
                    }

                    if (run.Optimisation)
                    {
                        processesRanges.Add(AgProcessRange.Create(run, RunStepEnum.Optimiser));
                    }

                    if (processesRanges.Any())
                    {
                        RaiseInfo($"Started populating v_param_date_list.xml. Process Date Ranges, Current Time - {DateTime.UtcNow.ToString()}");

                        AgProcessesRangesSerialization.Create(processesRanges)
                            .Serialize($"{folderName}//v_param_date_list.xml");

                        RaiseInfo($"Finished populating v_param_date_list.xml. Process Date Ranges, Current Time - {DateTime.UtcNow.ToString()}");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_param_date_list.xml because no process is enabled for the run. Process Date Ranges, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion Process Date Parameters List

                    #region Failures

                    if (run.FailureTypes != null && run.FailureTypes.Any())
                    {
                        var agRunFailureTypes = run.FailureTypes
                            .Select(id => new AgFailureType { FailureTypeId = id })
                            .ToList();

                        RaiseInfo($"Started populating v_fail_type_list.xml. Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                        AgFailureTypeSerialization
                            .MapFrom(agRunFailureTypes)
                            .Serialize($"{folderName}//v_fail_type_list.xml");

                        RaiseInfo($"Finished populating v_fail_type_list.xml. Total Failure Types - {agRunFailureTypes.Count()}, Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_fail_type_list.xml because there is no Failure Types stored in the run. Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
                    }

                    #endregion Failures

                    #region Programme Programme Categories map

                    errorMessage = "Error generating Programme Programme Categories Map file";

                    if (programmes.Any())
                    {
                        RaiseInfo($"Started populating v_prog_prgc_list.xml. Total Programme - {programmes.Count()}, Current Time - {DateTime.UtcNow.ToString()}");

                        var agProgrammeProgrammeCategoryMapsSerialization =
                            programmes.ToAgProgrammeProgrammeCategoryMapSerialization(programmeDictionary, programmeCategories);
                        agProgrammeProgrammeCategoryMapsSerialization?.Serialize($"{folderName}//v_prog_prgc_list.xml");

                        RaiseInfo($"Finished populating v_prog_prgc_list.xml. Total Programme - {programmes.Count()}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    #endregion Programme Programme Categories map

                    errorMessage = "Error creating zip file";
                    ZipFile.CreateFromDirectory(folderName, zipPath);
                    return zipPath;
                }
                catch (Exception exception)
                {
                    // For Optimiser Report then try and create a user friendly message
                    if (String.IsNullOrEmpty(errorMessage))
                    {
                        throw exception;
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}: {1}", errorMessage, exception.Message), exception);
                    }
                }
                finally
                {
                    if (Directory.Exists(folderName))
                    {
                        Directory.Delete(folderName, true); // delete run xml folder
                    }

                    demographic.DisposeAll();
                    programmeCategories.DisposeAll();
                    allSalesAreas.DisposeAll();
                    prioritySalesAreas.DisposeAll();
                }
            }
        }

        private void SerializerRestrictions(
            string folderName,
            List<ProgrammeCategoryHierarchy> programmeCategories,
            List<SalesArea> allSalesAreas,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            IEnumerable<Restriction> allRestrictions,
            DateTime runStartDate,
            DateTime runEndDate,
            ref List<Programme> programs,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionary,
            List<Clash> allClashes,
            List<Campaign> campaigns,
            List<Product> products,
            out string errorMessage)
        {
            errorMessage = "Error generating Restriction List file";

            List<Restriction> restrictions = GetApplicableRestrictions(
                allRestrictions,
                campaigns,
                products);

            if (restrictions.Count == 0)
            {
                RaiseInfo($"No restrictions available to export, Current Time - {DateTime.UtcNow.ToString()}");

                return;
            }

            IEnumerable<Restriction> progTxBasedRestrictions = restrictions
                .Where(r =>
                    r.RestrictionType != RestrictionType.Index &&
                    r.RestrictionType != RestrictionType.Time);

            if (progTxBasedRestrictions.Any())
            {
                SerializerProgrammeTxRestrictions(
                    folderName,
                    programmeCategories,
                    allSalesAreas,
                    autoBookDefaultParameters,
                    runStartDate,
                    runEndDate,
                    ref programs,
                    programmeDictionary);
            }

            string restrictionCount = restrictions.Count.ToString();

            RaiseInfo($"Started populating v_rb_rest_list.xml. Total restrictions - {restrictionCount}, Current Time - {DateTime.UtcNow.ToString()}");

            restrictions.ToAgRestriction(
                programmeDictionary,
                programmeCategories,
                allSalesAreas,
                runEndDate.Date,
                allClashes,
                _mapper,
                autoBookDefaultParameters.AgRestriction)
                .Serialize($"{folderName}//v_rb_rest_list.xml");

            RaiseInfo($"Finished populating v_rb_rest_list.xml. Total restrictions - {restrictionCount}, Current Time - {DateTime.UtcNow.ToString()}");

            if (restrictions.Any(r => r.RestrictionType == RestrictionType.Index))
            {
                SerializerHfssDemos(
                    folderName,
                    autoBookDefaultParameters);
            }

            List<Restriction> GetApplicableRestrictions(
                IEnumerable<Restriction> allRestrictions,
                List<Campaign> campaigns,
                List<Product> products)
            {
                var filteredRestrictions = Enumerable.Empty<Restriction>().ToList();

                IList<string> campaignClearanceCodes = campaigns
                    .Where(c => !string.IsNullOrWhiteSpace(c.ExpectedClearanceCode))
                    .Select(c => c.ExpectedClearanceCode)
                    .ToList();

                if (campaignClearanceCodes.Count > 0)
                {
                    filteredRestrictions.AddRange(allRestrictions.Where(r => campaignClearanceCodes.Contains(r.ClearanceCode)));
                }

                IList<string> campaignProductCodes = campaigns.ConvertAll(c => c.Product);
                if (campaignProductCodes.Count > 0)
                {
                    filteredRestrictions.AddRange(allRestrictions.Where(r => campaignProductCodes.Contains(r.ProductCode.ToString())));
                }

                IList<string> campaignClashCodes = products.ConvertAll(p => p.ClashCode);

                if (campaignClashCodes.Count > 0)
                {
                    filteredRestrictions.AddRange(allRestrictions.Where(r => campaignClashCodes.Contains(r.ClashCode)));
                }

                return filteredRestrictions.Distinct().ToList();
            }
        }

        private void SerializerHfssDemos(
            string folderName,
            IAutoBookDefaultParameters autoBookDefaultParameters)
        {
            string hfssCount = autoBookDefaultParameters.AgHfssDemos.Count.ToString();

            RaiseInfo($"Started populating v_hfss_demo_list.xml. Total hfss demos - {hfssCount}, Current Time - {DateTime.UtcNow.ToString()}");

            //v_hfss_demo_list.xml is a static file for now.
            AgHfssDemosSerialisation agHfssDemosSerialisation = new AgHfssDemosSerialisation();
            agHfssDemosSerialisation.PopulateStaticHfssDemos(autoBookDefaultParameters.AgHfssDemos)
                .Serialize($"{folderName}//v_hfss_demo_list.xml");

            RaiseInfo($"Finished populating v_hfss_demo_list.xml. Total hfss demos - {hfssCount}, Current Time - {DateTime.UtcNow.ToString()}");
        }

        private void SerializerProgrammeTxRestrictions(
            string folderName,
            List<ProgrammeCategoryHierarchy> programmeCategories,
            List<SalesArea> allSalesAreas,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            DateTime runStartDate,
            DateTime runEndDate,
            ref List<Programme> programs,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionary)
        {
            if (programs is null)
            {
                NoProgrammesAvailable();
            }

            FilterProgrammesByDate(
                ref programs,
                runStartDate.AddDays(-1),
                runEndDate.AddDays(1));

            if (programs.Count == 0)
            {
                NoProgrammesAvailable();
            }

            _progTxDetailSerializer.Serialize(
                folderName,
                programs,
                programmeDictionary,
                programmeCategories,
                allSalesAreas,
                autoBookDefaultParameters);

            FilterProgrammesByDate(
                ref programs,
                runStartDate,
                runEndDate);

            if (programs.Count == 0)
            {
                NoProgrammesAvailable();
            }

            void NoProgrammesAvailable() =>
                throw new Exception("No programmes available to export");
        }

        private void SerializeStandardDayPartGroups(IRepositoryScope scope,
            List<SalesArea> allSalesAreas, List<Demographic> demographic, string folderName, out string errorMessage)
        {
            errorMessage = "Error generating StandardDayPartGroups List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.DayPartGroup)))
            {
                return;
            }

            RaiseInfo($"Getting the standard dayParts groups for processing. Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            var standardDayPartGroupRepository = scope.CreateRepository<IStandardDayPartGroupRepository>();
            var standardDaypartGroups = standardDayPartGroupRepository.GetAll().ToList();

            if (standardDaypartGroups != null && standardDaypartGroups.Any())
            {
                RaiseInfo($"Started populating v_sdpg_list.xml. Total standard dayPart groups - {standardDaypartGroups.Count}, Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                standardDaypartGroups.ToAgStandardDayPartGroups(allSalesAreas, demographic)
                    .Serialize($"{folderName}//v_sdpg_list.xml");

                RaiseInfo($"Finished populating v_sdpg_list.xml. Total standard dayParts groups - {standardDaypartGroups.Count}, Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                standardDaypartGroups.DisposeAll();
            }
        }

        private void SerializeStandardDayParts(IRepositoryScope scope, List<SalesArea> allSalesAreas,
            string folderName, out string errorMessage)
        {
            errorMessage = "Error generating StandardDayParts List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.DayPartGroup)))
            {
                return;
            }

            RaiseInfo($"Getting the standard dayParts for processing. Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            var standardDayPartRepository = scope.CreateRepository<IStandardDayPartRepository>();
            var standardDayParts = standardDayPartRepository.GetAll().ToList();

            if (standardDayParts != null && standardDayParts.Any())
            {
                RaiseInfo($"Started populating v_stdp_list.xml. Total standard dayParts - {standardDayParts.Count}, Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                standardDayParts.ToAgStandardDayParts(allSalesAreas)
                    .Serialize($"{folderName}//v_stdp_list.xml");

                RaiseInfo($"Finished populating v_stdp_list.xml. Total standard dayParts - {standardDayParts.Count}, Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                standardDayParts.DisposeAll();
            }
        }

        private static void InitialiseFolder(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                Directory.Delete(folderName, true);
            }

            _ = Directory.CreateDirectory(folderName);
        }

        private static void SerialiseClashExposureDetails(
            Run run,
            string folderName,
            List<SalesArea> allSalesAreas,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            List<Clash> allClashes,
            TenantSettings tenantSettings,
            in IMapper mapper,
            in Action<string> raiseInfo)
        {
            const string AgTimeFormat = "hhmmss";

            raiseInfo($"Getting the clashes for {ClashExposureDetailsSerialiser.Filename} processing. Current Time - {DateTime.UtcNow.ToString()}");
            raiseInfo($"Started populating c. Total clashes - {allClashes.Count}, Current Time - {DateTime.UtcNow.ToString()}");

            TimeSpan? peakStartTime = null;
            TimeSpan? peakEndTime = null;

            if (TimeSpan.TryParseExact(tenantSettings.PeakStartTime, AgTimeFormat,
                CultureInfo.InvariantCulture, out TimeSpan parsedPeakStartTime))
            {
                peakStartTime = parsedPeakStartTime;
            }

            if (TimeSpan.TryParseExact(tenantSettings.PeakEndTime, AgTimeFormat,
                CultureInfo.InvariantCulture, out TimeSpan parsedPeakEndTime))
            {
                peakEndTime = parsedPeakEndTime;
            }

            var serialiser = new ClashExposureDetailsSerialiser(folderName);
            serialiser.Serialise(
                allClashes,
                run.StartDate,
                run.EndDate,
                peakStartTime,
                peakEndTime,
                allSalesAreas,
                mapper,
                autoBookDefaultParameters.AgExposure
                );

            raiseInfo($"Finished populating {ClashExposureDetailsSerialiser.Filename}. Total clashes - {allClashes.Count}, Current Time - {DateTime.UtcNow.ToString()}");
        }

        private static void SerialiseSpots(
            in string folderName,
            List<SalesArea> prioritySalesAreas,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            List<Clash> allClashes,
            IReadOnlyCollection<BreakWithProgramme> breaks,
            List<Campaign> campaigns,
            List<Demographic> demographics,
            List<BusinessType> businessTypes,
            List<Spot> spots,
            List<Product> allProducts,
            IFeatureManager featureManager,
            in Action<string> raiseInfo)
        {
            raiseInfo($"Started populating {SpotSerialiser.Filename}. Spot, Current Time - {DateTime.UtcNow.ToString()}");

            var spotSerialiser = new SpotSerialiser(folderName);
            spotSerialiser.Serialise(
                spots,
                prioritySalesAreas,
                breaks,
                campaigns,
                demographics,
                businessTypes,
                allProducts,
                allClashes,
                autoBookDefaultParameters.AgSpot,
                featureManager
                );

            raiseInfo($"Finished populating {SpotSerialiser.Filename}. Spot, Current Time - {DateTime.UtcNow.ToString()}");
        }

        private static void SerializeBusinessTypes(in string folderName, List<BusinessType> businessTypes, in Action<string> raiseInfo)
        {
            raiseInfo($"Started populating {BusinessTypeSerializer.Filename}. BusinessType, Current Time - {DateTime.UtcNow.ToString()}");

            var serializer = new BusinessTypeSerializer(folderName);
            serializer.Serialise(businessTypes);

            raiseInfo($"Finished populating {BusinessTypeSerializer.Filename}. BusinessType, Current Time - {DateTime.UtcNow.ToString()}");
        }

        private static void SerialiseSponsorshipExclusivity(
            in string folderName,
            List<SalesArea> allSalesAreas,
            List<Sponsorship> allSponsorships,
            IReadOnlyCollection<Programme> programs,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionary,
            in IMapper mapper,
            in Action<string> raiseInfo)
        {
            raiseInfo($"Started populating {SponsorshipExclusivitySerialiser.Filename}. Total Sponsorships - {allSponsorships.Count}, Current Time - {DateTime.UtcNow.ToString()}");

            var sponsorshipExclusivitySerialiser = new SponsorshipExclusivitySerialiser(folderName);
            sponsorshipExclusivitySerialiser.Serialise(
                allSponsorships,
                programs,
                programmeDictionary,
                allSalesAreas,
                mapper
                );

            raiseInfo($"Finished populating {SponsorshipExclusivitySerialiser.Filename}. Total Sponsorships - {allSponsorships.Count}, Current Time - {DateTime.UtcNow.ToString()}");
        }

        private static void SerialiseSalesAreaPointers(
            in string folderName,
            in IReadOnlyCollection<SalesArea> allSalesAreas,
            in IReadOnlyCollection<Demographic> demographic,
            in IMapper mapper,
            in Action<string> raiseInfo
            )
        {
            raiseInfo(
                $"Started populating {SalesAreaPointersSerialiser.Filename}. Total sales areas - {allSalesAreas.Count}, "
                + $"Current Time - {DateTime.UtcNow.ToString()}"
                );

            var salesAreaPtrs = new SalesAreaPointersSerialiser(folderName);
            salesAreaPtrs.Serialise(allSalesAreas, demographic, mapper);

            raiseInfo(
                $"Finished populating {SalesAreaPointersSerialiser.Filename}. Total sales areas - {allSalesAreas.Count}, "
                + $"Current Time - {DateTime.UtcNow.ToString()}"
                );
        }

        private static void SerialiseSalesAreaReferences(
            in string folderName,
            in IReadOnlyCollection<SalesArea> allSalesAreas,
            in IMapper mapper,
            in Action<string> raiseInfo
            )
        {
            raiseInfo(
                $"Started populating {SalesAreaHierarchySerialiser.Filename}. Total sales areas - {allSalesAreas.Count}, "
                + $"Current Time - {DateTime.UtcNow.ToString()}"
                );

            var salesAreaReferences = new SalesAreaHierarchySerialiser(folderName);
            salesAreaReferences.Serialise(allSalesAreas, mapper);

            raiseInfo(
                $"Finished populating {SalesAreaHierarchySerialiser.Filename}. Total sales areas - {allSalesAreas.Count}, "
                + $"Current Time - {DateTime.UtcNow.ToString()}"
                );
        }

        /// <summary>
        /// Populate scenario data for dynamic xml generation
        /// </summary>
        public string PopulateScenarioData(Run run, Guid scenarioId)
        {
            DateTime systemDate = _systemLogicalDateService.GetSystemLogicalDate();
            string errorMessage = "";
            var folderName = string.Format(@"{0}\Temp\Scenario\{1}", RootFolder, scenarioId);
            var zipPath = folderName + ".zip";
            IAutoBookDefaultParameters autoBookDefaultParameters = null;

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var demographicRepository = scope.CreateRepository<IDemographicRepository>();
                var scenarioRepository = scope.CreateRepository<IScenarioRepository>();
                var passRepository = scope.CreateRepository<IPassRepository>();
                var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();
                var tenantSettingsRepository = scope.CreateRepository<ITenantSettingsRepository>();
                var autoBookDefaultParametersRepository = scope.CreateRepository<IAutoBookDefaultParametersRepository>();

                try
                {
                    autoBookDefaultParameters = autoBookDefaultParametersRepository.Get();

                    var allSalesAreas = salesAreaRepository.GetAll().ToList();

                    errorMessage = "Error creating local folder for scenario zip file";
                    if (File.Exists(zipPath))
                    {
                        return zipPath; // if already Scenario data available then skip the XML population
                    }

                    if (Directory.Exists(folderName))
                    {
                        Directory.Delete(folderName, true);
                    }

                    Directory.CreateDirectory(folderName);
                    // Run
                    var runScenario = run.Scenarios.Where(x => x.Id == scenarioId).FirstOrDefault();
                    var scenario = scenarioRepository.Get(scenarioId);

                    var tenantSettings = tenantSettingsRepository.Get();

                    var passes = new List<Pass>();
                    if (scenario.Passes != null)
                    {
                        scenario.Passes.ForEach(p => passes.Add(passRepository.Get(p.Id)));
                    }

                    // Add weighting
                    errorMessage = "Error generating Scenario Weighting List file";
                    passes.ToAgWeighting(scenario).Serialize($"{folderName}//v_abwe_list.xml");

                    // Add tolerance
                    errorMessage = "Error generating Scenario Tolerance List file";
                    passes.ToAgTolerance(scenario, _featureManager).Serialize($"{folderName}//v_atol_list.xml");

                    // Add rule
                    errorMessage = "Error generating Scenario Rule List file";
                    passes.ToAgRule(scenario).Serialize($"{folderName}//v_agru_list.xml");

                    // Add pass default
                    errorMessage = "Error generating Scenario Pass Default List file";
                    passes.ToAgPassDefault(scenario, run, tenantSettings).Serialize($"{folderName}//v_abps_list.xml");

                    // add default
                    errorMessage = "Error generating Scenario Default List file";
                    passes.ToAgDefault(scenario, _featureManager).Serialize($"{folderName}//v_agde_list.xml");

                    // Add Rating Points
                    errorMessage = "Error generating Rating Points List file";
                    passes.ToAgRatingPoints(allSalesAreas, tenantSettings).Serialize($"{folderName}//v_min_tarp_list.xml");

                    // add Scenario's Campaign Priority Rounds
                    errorMessage = "Error generating Campaign Priority Round List file";
                    scenario.ToAgCampaignPriorityRound().Serialize($"{folderName}//v_mode_list.xml");

                    // add params
                    errorMessage = "Error generating Scenario Parameter List file";
                    scenario
                        .ToAgParams(run, tenantSettings.AutoBookTargetedZeroRatedBreaks, systemDate, _featureManager, tenantSettings.OpenAirtimeFactor)
                        .Serialize($"{folderName}//v_param_list.xml");

                    // Add Scenario's Campaign Passes
                    errorMessage = "Error generating Scenario's Campaign Pass List file";
                    scenario.ToAgScenarioCampaignPass().Serialize($"{folderName}//v_camp_reqs_list.xml");

                    //add Programme Repetition
                    errorMessage = "Error generating Programme Repetition List file";
                    if (passes != null && passes.Any() &&
                        passes.Any(p => p.ProgrammeRepetitions != null && p.ProgrammeRepetitions.Any()))
                    {
                        scenario.ToAgProgrammeRepetitions(passes).Serialize($"{folderName}//v_abpr_list.xml");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_abpr_list.xml because programme repetitions is null or empty in scenario id - {scenarioId}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    // Add pass default

                    errorMessage = "Error generating Sales Area Pass Priority List file";
                    passes.ToAgSalesAreaPassPriority(scenario, allSalesAreas, run, _mapper)
                        .Serialize($"{folderName}//v_sare_reqs_list.xml");

                    //add Peak start and end time

                    var peakStartTime = tenantSettings.PeakStartTime;
                    var peakEndTime = tenantSettings.PeakEndTime;

                    errorMessage = "Error Peak start and end time file";

                    scenario.ToPeakStartAndEndTime(
                        allSalesAreas,
                        peakStartTime,
                        peakEndTime,
                        autoBookDefaultParameters.AgPeakStartEndTime)
                        .Serialize($"{folderName}//v_agdt_list.xml");

                    //add Break Exclusions
                    errorMessage = "Error generating Break exclusion List file";
                    if (passes != null && passes.Any() &&
                        passes.Any(p => p.BreakExclusions != null && p.BreakExclusions.Any()))
                    {
                        scenario.ToAgBreakExclusions(passes, allSalesAreas)
                            .Serialize($"{folderName}//v_brek_excl_list.xml");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_brek_excl_list.xml because Break exclusion is null or empty in scenario id - {scenarioId}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    allSalesAreas.DisposeAll();

                    //add slotting Limit by demograph
                    errorMessage = "Error generating slotting Limit by demograph List file";
                    if (passes != null && passes.Any() &&
                        passes.Any(p => p.SlottingLimits != null && p.SlottingLimits.Any()))
                    {
                        var demographs = demographicRepository.GetAll().ToList();
                        scenario.ToAgSlottingLimits(passes, demographs).Serialize($"{folderName}//v_abdm_list.xml");
                    }
                    else
                    {
                        RaiseInfo($"Not generating v_abdm_list.xml because slotting Limit by demographs is null or empty in scenario id - {scenarioId}, Current Time - {DateTime.UtcNow.ToString()}");
                    }

                    errorMessage = "Error creating zip file";
                    ZipFile.CreateFromDirectory(folderName, zipPath);
                    Directory.Delete(folderName, true); // Delete the unzip folder
                }
                catch (System.Exception exception)
                {
                    // For Optimiser Report then try and create a user friendly message
                    if (String.IsNullOrEmpty(errorMessage))
                    {
                        throw exception;
                    }
                    else
                    {
                        throw new Exception(String.Format("{0}: {1}", errorMessage, exception.Message), exception);
                    }
                }
            }

            return zipPath;
        }

        private void FilterProgrammesByDate(
            ref List<Programme> programs,
            DateTime runStartDateTime,
            DateTime runEndDateTime)
        {
            programs = programs
                .Where(p =>
                    p.StartDateTime >= runStartDateTime &&
                    p.StartDateTime <= runEndDateTime)
                .ToList();
        }

        private void RemoveInvalidCampaign(ref List<Campaign> campaigns)
        {
            var allowMissingLengths = _featureManager.IsEnabled(nameof(ProductFeature.StrikeWeightDayPartsMerge));

            campaigns?.RemoveAll(c =>
            {
                var saleAreaTargets = c.SalesAreaCampaignTarget;
                var invalidMultiPart = saleAreaTargets == null || !saleAreaTargets.Any() ||
                              saleAreaTargets.Any(s => s.Multiparts == null || s.Multiparts.Count == 0);
                if (invalidMultiPart)
                {
                    RaiseInfo($"Excluded campaign with external id - {c.ExternalId} for processing. Multiparts are missing. Current Time - {DateTime.UtcNow.ToString()}");

                    return true;
                }

                var invalidLength = !allowMissingLengths && saleAreaTargets.Any(s => s.CampaignTargets?
                                                              .Any(ct => ct?.StrikeWeights?
                                                              .Any(sw => sw?.Lengths == null || sw.Lengths.Count == 0)
                                                              ?? true)
                                                              ?? true);
                if (invalidLength)
                {
                    RaiseInfo($"Excluded campaign with external id - {c.ExternalId} for processing. Lengths are missing. Current Time - {DateTime.UtcNow.ToString()}");

                    return true;
                }

                return false;
            });
        }

        private IEnumerable<Spot> GetSpots(
            IReadOnlyCollection<string> salesAreasNames,
            DateTime fromDateTime,
            DateTime toDateTime,
            IReadOnlyCollection<BreakWithProgramme> breaks,
            string excludeBreakType,
            IRepositoryFactory repositoryFactory)
        {
            RaiseInfo($"Getting spots for {salesAreasNames.Count} sales areas ({fromDateTime} - {toDateTime})");

            var salesAreaSpotCollections = new ConcurrentBag<List<Spot>>();

            _ = Parallel.ForEach(salesAreasNames, salesAreaName =>
            {
                RaiseInfo($"Gettings spots for sales area {salesAreaName} ({fromDateTime} - {toDateTime})");

                var salesAreaSpots = new List<Spot>();

                using (var scope = repositoryFactory.BeginRepositoryScope())
                {
                    var spotRepository = scope.CreateRepository<ISpotRepository>();

                    foreach (DateTime[] dateRange in DateHelper.GetDateRanges(fromDateTime, toDateTime, 30))
                    {
                        RaiseInfo(
                            $"Gettings batch of spots for sales area {salesAreaName} ({dateRange[0]} - {dateRange[1]})"
                            );

                        var rangeSalesAreaSpots = spotRepository
                            .Search(dateRange[0], dateRange[1], salesAreaName)
                            .ToList();

                        RaiseInfo(
                            $"Got batch of {rangeSalesAreaSpots.Count} spots for sales area {salesAreaName} ({dateRange[0]} - {dateRange[1]})"
                            );

                        salesAreaSpots.AddRange(rangeSalesAreaSpots);
                    }
                }

                // Filter spots that we need to upload. We exclude spots for
                // Premium break types if configured
                var externalBreakNos = breaks
                    .Select(b => b.Break.ExternalBreakRef)
                    .Distinct()
                    .ToList();

                // Exclude Spot when
                // 1) Empty external break ref in spot
                // 2) Spot has the excluded break type (Configured to exclude)
                // 3) Invalid external break ref in spot
                _ = salesAreaSpots.RemoveAll(
                    s =>
                        String.IsNullOrWhiteSpace(s.ExternalBreakNo)
                        || (!String.IsNullOrWhiteSpace(s.BreakType)
                        && !String.IsNullOrWhiteSpace(excludeBreakType)
                        && s.BreakType.Equals(excludeBreakType, StringComparison.OrdinalIgnoreCase))
                        || (!String.IsNullOrWhiteSpace(s.ExternalBreakNo)
                        && !externalBreakNos.Contains(s.ExternalBreakNo))
                    );

                RaiseInfo(
                    $"After exclusion, Got {salesAreaSpots.Count} spots for sales area {salesAreaName} ({fromDateTime} - {toDateTime})"
                    );

                salesAreaSpotCollections.Add(salesAreaSpots);
            });

            var spots = new List<Spot>();

            foreach (var spotCollection in salesAreaSpotCollections)
            {
                spots.AddRange(spotCollection);
            }

            RaiseInfo($"Got {spots.Count} spots for {salesAreasNames.Count} sales areas ({fromDateTime} - {toDateTime})");

            return spots;
        }

        private Dictionary<DateTime, DateTime> GetWeekData(DateTime fromDate, DateTime toDate,
            DayOfWeek startDayOfWeek = DayOfWeek.Monday)
        {
            var weekset = new Dictionary<DateTime, DateTime>();
            while (fromDate <= toDate)
            {
                var weekrange = fromDate.StartAndEndOfWeekDate(startDayOfWeek);
                weekset.Add(weekrange.startDate, weekrange.endDate);

                fromDate = weekrange.startDate.AddDays(7); // iterate to next week
            }

            return weekset;
        }

        private void SerializeFacilities(IRepositoryScope scope, string folderName, out string errorMessage)
        {
            errorMessage = "Error generating Facility List file";
            RaiseInfo($"Getting the facilities for processing. {CurrentTimeMessage()}");

            var facilityRepository = scope.CreateRepository<IFacilityRepository>();
            var facilities = facilityRepository.GetAll().ToList();
            if (facilities != null && facilities.Any())
            {
                var enabledFacilities = facilities.Where(f => f.Enabled).ToList();
                if (enabledFacilities.Any())
                {
                    RaiseInfo($"Started populating v_fac_list.xml. Total facilities - {enabledFacilities.Count}, {CurrentTimeMessage()}");

                    AgFacilitySerialization
                        .MapFrom(enabledFacilities.Select(f => new AgFacility { Code = f.Code }).ToList())
                        .Serialize($"{folderName}//v_fac_list.xml");

                    RaiseInfo($"Finished populating v_fac_list.xml. Total facilities - {enabledFacilities.Count}, {CurrentTimeMessage()}");
                }
                else
                {
                    RaiseInfo($"Not generating v_fac_list.xml because all the facilities are disabled. {CurrentTimeMessage()}");
                }
                facilities.DisposeAll();
            }
            else
            {
                throw new Exception("No facilities available to export");
            }
        }

        private void SerializeCampaignBookingPositions(IRepositoryScope scope, string folderName, List<Campaign> campaigns, List<SalesArea> allSalesAreas, out string errorMessage)
        {
            errorMessage = "Error generating Campaign Booking Position Groups file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.PositionInBreak)))
            {
                return;
            }

            RaiseInfo($"Getting the booking position groups for processing. {CurrentTimeMessage()}");

            var bookingPositionsRepository = scope.CreateRepository<IBookingPositionRepository>();
            var bookingPositionGroupsRepository = scope.CreateRepository<IBookingPositionGroupRepository>();

            var allBookingPositions = bookingPositionsRepository.GetAll().ToList();
            var allBookingPositionGroups = bookingPositionGroupsRepository.GetAll().ToList();
            var campaignPositionGroups = campaigns.ToAgBookingPositionGroups(allBookingPositionGroups, allBookingPositions, allSalesAreas, _mapper, out var totalPositionGroups);

            if (campaignPositionGroups != null && totalPositionGroups > 0)
            {
                RaiseInfo($"Started populating v_cmbp_list.xml. Total Booking Position Groups - {totalPositionGroups}, {CurrentTimeMessage()}");

                campaignPositionGroups.Serialize($"{folderName}//v_cmbp_list.xml");

                RaiseInfo($"Finished populating v_cmbp_list.xml. Total Booking Position Groups - {totalPositionGroups}, {CurrentTimeMessage()}");
            }
            else
            {
                RaiseInfo($"Not generating v_cmbp_list.xml because there is no Booking Position Group stored in the run. {CurrentTimeMessage()}");
            }

            allBookingPositions.DisposeAll();
            allBookingPositionGroups.DisposeAll();
        }

        private void SerializeDeliveryCappingData(IRepositoryScope scope, string folderName, Run run, IReadOnlyCollection<Campaign> campaigns, out string errorMessage)
        {
            errorMessage = "Error generating Delivery Capping Groups List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.DeliveryCappingGroup)))
            {
                return;
            }

            var deliveryCappingGroupRepository = scope.CreateRepository<IDeliveryCappingGroupRepository>();
            var deliveryCappingGroups = deliveryCappingGroupRepository.GetAll().ToList().AsReadOnly();

            ExportDeliveryCappingGroup(deliveryCappingGroups, folderName, "v_abdg_list.xml");

            errorMessage = "Error generating Campaigns with Delivery Capping Groups List file";
            ExportCampaignDeliveryCappingGroup(run, deliveryCappingGroups, campaigns, folderName, "v_camp_abdg_list.xml");
        }

        private void SerializeLengthFactorData(IRepositoryScope scope, string folderName, IEnumerable<SalesArea> allSalesAreas, out string errorMessage)
        {
            errorMessage = "Error generating Length Factors List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.LengthFactor)))
            {
                return;
            }

            var fileName = "v_lefa_list.xml";
            var lengthFactorRepository = scope.CreateRepository<ILengthFactorRepository>();
            var lengthFactors = lengthFactorRepository.GetAll().ToList();

            if (lengthFactors.Any())
            {
                RaisePopulatingMessage(lengthFactors, fileName, isStartPopulating: true);

                var salesAreaNames = lengthFactors.Select(x => x.SalesArea).ToList();
                var salesAreas = allSalesAreas.Where(x => salesAreaNames.Contains(x.Name));

                lengthFactors.ToAgLengthFactorGroups(salesAreas)?.Serialize($"{folderName}//{fileName}");

                RaisePopulatingMessage(lengthFactors, fileName, isStartPopulating: false);

                lengthFactors.DisposeAll();
            }
        }

        private void ExportDeliveryCappingGroup(IReadOnlyCollection<DeliveryCappingGroup> deliveryCappingGroups, string folderName, string fileName)
        {
            RaiseInfo($"Getting the delivery capping groups for processing. {CurrentTimeMessage()}");

            if (deliveryCappingGroups.Any())
            {
                RaisePopulatingMessage(deliveryCappingGroups, fileName, isStartPopulating: true);

                AgDeliveryCappingGroupSerialization
                    .MapFrom(_mapper.Map<IEnumerable<AgDeliveryCappingGroup>>(deliveryCappingGroups).ToList())
                    .Serialize($"{folderName}//{fileName}");

                RaisePopulatingMessage(deliveryCappingGroups, fileName, isStartPopulating: false);

                deliveryCappingGroups.DisposeAll();
            }
            else
            {
                RaiseInfo($"No campaigns with delivery capping groups available to export. {CurrentTimeMessage()}");
            }
        }

        private void ExportCampaignDeliveryCappingGroup(Run run, IReadOnlyCollection<DeliveryCappingGroup> deliveryCappingGroups, IReadOnlyCollection<Campaign> campaigns, string folderName, string fileName)
        {
            RaiseInfo($"Getting the delivery capping groups for processing. {CurrentTimeMessage()}");

            var settings = run.CampaignsProcessesSettings
                .Where(x => deliveryCappingGroups.Any(y => y.Id == x.DeliveryCappingGroupId));

            var agCampaignsDeliveryCappingGroups = campaigns.Join(
                settings,
                x => x.ExternalId,
                y => y.ExternalId,
                (campaign, deliveryCappingGroup) => new AgCampaignDeliveryCappingGroup
                {
                    CampaignNo = campaign.CustomId,
                    DeliveryCappingGroupId = deliveryCappingGroup.DeliveryCappingGroupId
                }
            ).ToList();

            if (agCampaignsDeliveryCappingGroups.Any())
            {
                RaisePopulatingMessage(agCampaignsDeliveryCappingGroups, fileName, isStartPopulating: true);

                AgCampaignDeliveryCappingGroupSerialization
                    .MapFrom(agCampaignsDeliveryCappingGroups)
                    .Serialize($"{folderName}//{fileName}");

                RaisePopulatingMessage(agCampaignsDeliveryCappingGroups, fileName, isStartPopulating: false);

                agCampaignsDeliveryCappingGroups.DisposeAll();
            }
            else
            {
                RaiseInfo($"No campaigns with delivery capping groups available to export. {CurrentTimeMessage()}");
            }
        }

        private void SerializeSalesAreaDemographics(IRepositoryScope scope, string folderName,
            IReadOnlyCollection<SalesArea> salesAreas, IReadOnlyCollection<Demographic> demographics, out string errorMessage)
        {
            errorMessage = "Error generating Sales Area Demographics List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.EfficiencyFactor)))
            {
                return;
            }

            var fileName = "v_rdem_list.xml";
            var salesAreaDemographicsRepository = scope.CreateRepository<ISalesAreaDemographicRepository>();
            var salesAreaDemographics = salesAreaDemographicsRepository.GetAll().ToList().AsReadOnly();

            RaiseInfo($"Getting the sales area demographics for processing. {CurrentTimeMessage()}");

            var entities = salesAreas.Join(salesAreaDemographics, x => x.Name, y => y.SalesArea, (salesArea, demographic) => new AgSalesAreaDemographic
            {
                SalesAreaNo = salesArea.CustomId,
                DemographicNo = demographics.First(x => x.ExternalRef == demographic.ExternalRef).Id,
                Exclude = Convert.ToInt32(demographic.Exclude),
                SupplierCode = demographic.SupplierCode
            }).ToList();

            if (entities.Any())
            {
                RaisePopulatingMessage(entities, fileName, isStartPopulating: true);

                AgSalesAreaDemographicsSerialization
                    .MapFrom(entities)
                    .Serialize($"{folderName}//{fileName}");

                RaisePopulatingMessage(entities, fileName, isStartPopulating: false);

                entities.DisposeAll();
            }
            else
            {
                RaiseInfo($"No sales area demographics available to export. {CurrentTimeMessage()}");
            }
        }

        private void SerializeTotalRatings(IRepositoryScope scope, string folderName, List<Demographic> demographic, List<SalesArea> salesAreas, DateTime runStartDate, DateTime runEndDate, out string errorMessage)
        {
            errorMessage = "Error generating TotalRatings List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.TotalRating)))
            {
                return;
            }

            RaiseInfo($"Getting the total ratings for processing. {CurrentTimeMessage()}");

            var totalRatingsRepository = scope.CreateRepository<ITotalRatingRepository>();
            var totalRatings = totalRatingsRepository.SearchByMonths(runStartDate, runEndDate).ToList();

            if (totalRatings != null && totalRatings.Any())
            {
                RaiseInfo($"Started populating v_trdp_day_list.xml. Total total ratings - {totalRatings.Count}, {CurrentTimeMessage()}");

                totalRatings.ToAgTotalRatings(demographic, salesAreas)
                    .Serialize($"{folderName}//v_trdp_day_list.xml");

                RaiseInfo($"Finished populating v_trdp_day_list.xml. Total total ratings - {totalRatings.Count}, {CurrentTimeMessage()}");

                totalRatings.DisposeAll();
            }
        }

        private void SerializeInventoryStatuses(string folderName, List<InventoryStatus> inventoryStatuses, out string errorMessage)
        {
            errorMessage = "Error generating Inventory Status file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.InventoryStatus)))
            {
                return;
            }

            if (inventoryStatuses != null && inventoryStatuses.Any())
            {
                RaiseInfo($"Started populating v_invs_excl_list.xml. Inventory statuses - {inventoryStatuses.Count.ToString()}, {CurrentTimeMessage()}");

                inventoryStatuses.ToAgInventoryStatuses()
                    .Serialize($"{folderName}//v_invs_excl_list.xml");

                RaiseInfo($"Finished populating v_invs_excl_list.xml. Inventory statuses - {inventoryStatuses.Count.ToString()}, {CurrentTimeMessage()}");

                inventoryStatuses.DisposeAll();
            }
        }

        private void SerializeInventoryLocks(IRepositoryScope scope, string folderName, List<SalesArea> salesAreas, out string errorMessage)
        {
            errorMessage = "";
            var systemLockType = "Y";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.InventoryStatus)))
            {
                return;
            }

            errorMessage = "Error generating InventoryLocks List file";

            RaiseInfo($"Getting the inventory locks for processing. {CurrentTimeMessage()}");

            var fileName = "v_invs_list.xml";
            var inventoryLockRepository = scope.CreateRepository<IInventoryLockRepository>();
            var inventoryTypeRepository = scope.CreateRepository<IInventoryTypeRepository>();

            var inventoryLocks = inventoryLockRepository.GetAll().ToList().AsReadOnly();
            var inventoryTypes = inventoryTypeRepository.GetAll().ToList().AsReadOnly();
            var salesAreasIndex = salesAreas.ToDictionary(x => x.Name);

            var entities = (from iLock in inventoryLocks
                            join iType in inventoryTypes.DefaultIfEmpty() on iLock.InventoryCode equals iType.InventoryCode into lockJoined
                            from iType in lockJoined.DefaultIfEmpty()
                            from lkTypeId in iType.LockTypes.DefaultIfEmpty()
                            where iType.System?.Trim() == systemLockType || (iType.System?.Trim() != systemLockType && lkTypeId != 0)
                            select new AgInventoryLock
                            {
                                SalesArea = salesAreasIndex.ContainsKey(iLock.SalesArea) ? salesAreasIndex[iLock.SalesArea].CustomId : 0,
                                InventoryCode = iLock.InventoryCode.Trim(),
                                LockType = iType.System?.Trim() == systemLockType ? 0 : lkTypeId,
                                StartDate = AgConversions.ToAgDateYYYYMMDDAsString(iLock.StartDate),
                                EndDate = AgConversions.ToAgDateYYYYMMDDAsString(iLock.EndDate),
                                StartTime = AgConversions.ToAgTimeAsHHMMSS(iLock.StartTime),
                                EndTime = AgConversions.ToAgTimeAsHHMMSS(iLock.EndTime)
                            }).ToList();

            if (entities.Any())
            {
                RaisePopulatingMessage(entities, fileName, isStartPopulating: true);

                AgInventoryLockSerialization
                    .MapFrom(entities)
                    .Serialize($"{folderName}//{fileName}");

                RaisePopulatingMessage(entities, fileName, isStartPopulating: false);

                entities.DisposeAll();
            }
            else
            {
                RaiseInfo($"No inventory locks available to export. {CurrentTimeMessage()}");
            }
        }

        private void SerializeSpotBookingRules(IRepositoryScope scope, string folderName, List<SalesArea> salesAreas, out string errorMessage)
        {
            errorMessage = "Error generating spot booking rules List file";

            if (!_featureManager.IsEnabled(nameof(ProductFeature.SpotBookingRule)))
            {
                return;
            }

            RaiseInfo($"Getting the spot booking rules for processing. {CurrentTimeMessage()}");

            var spotBookingRuleRepository = scope.CreateRepository<ISpotBookingRuleRepository>();
            var spotBookingRules = spotBookingRuleRepository.GetAll().ToList();
            if (spotBookingRules != null && spotBookingRules.Any())
            {
                RaiseInfo($"Started populating v_sbru_list.xml. Spot booking rules - {spotBookingRules.Count}, {CurrentTimeMessage()}");

                spotBookingRules.ToAgSpotBookingRules(salesAreas)
                    .Serialize($"{folderName}//v_sbru_list.xml");

                RaiseInfo($"Finished populating v_sbru_list.xml. Spot booking rules - {spotBookingRules.Count}, {CurrentTimeMessage()}");

                spotBookingRules.DisposeAll();
            }
        }

        private void RaisePopulatingMessage<T>(IEnumerable<T> entities, string fileName, bool isStartPopulating)
        {
            var msg = isStartPopulating ? "Started" : "Finished";
            RaiseInfo($"{msg} populating {fileName}. Total {typeof(T).Name} - {entities.Count()}, {CurrentTimeMessage()}");
        }

        private string CurrentTimeMessage() => $"Current Time - {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}";
    }
}
