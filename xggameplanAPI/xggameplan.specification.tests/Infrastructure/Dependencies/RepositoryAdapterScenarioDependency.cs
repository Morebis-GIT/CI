using BoDi;
using xggameplan.specification.tests.Interfaces;
using xggameplan.specification.tests.RepositoryAdapters;

namespace xggameplan.specification.tests.Infrastructure.Dependencies
{
    public class RepositoryAdapterScenarioDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterTypeAs<AccessTokenRepositoryAdapter, IRepositoryAdapter>("AccessTokens");
            objectContainer.RegisterTypeAs<AnalysisGroupRepositoryAdapter, IRepositoryAdapter>("AnalysisGroups");
            objectContainer.RegisterTypeAs<AutoBookDefaultParametersRepositoryAdapter, IRepositoryAdapter>("AutoBookDefaultParameters");
            objectContainer.RegisterTypeAs<AutoBookRepositoryAdapter, IRepositoryAdapter>("AutoBooks");
            objectContainer.RegisterTypeAs<AutoBookInstanceConfigurationRepositoryAdapter, IRepositoryAdapter>("AutoBookInstanceConfiguration");
            objectContainer.RegisterTypeAs<AutoBookSettingsRepositoryAdapter, IRepositoryAdapter>("AutoBookSettings");
            objectContainer.RegisterTypeAs<AWSInstanceConfigurationRepositoryAdapter, IRepositoryAdapter>("AWSInstanceConfiguration");
            objectContainer.RegisterTypeAs<AutopilotRuleRepositoryAdapter, IRepositoryAdapter>("AutopilotRules");
            objectContainer.RegisterTypeAs<AutopilotSettingsRepositoryAdapter, IRepositoryAdapter>("AutopilotSettings");
            objectContainer.RegisterTypeAs<BookingPositionRepositoryAdapter, IRepositoryAdapter>("BookingPositions");
            objectContainer.RegisterTypeAs<BookingPositionGroupRepositoryAdapter, IRepositoryAdapter>("BookingPositionGroups");
            objectContainer.RegisterTypeAs<BreakRepositoryAdapter, IRepositoryAdapter>("Breaks");
            objectContainer.RegisterTypeAs<BusinessTypeRepositoryAdapter, IRepositoryAdapter>("BusinessTypes");
            objectContainer.RegisterTypeAs<BRSConfigurationTemplateRepositoryAdapter, IRepositoryAdapter>("BRSConfigurationTemplate");
            objectContainer.RegisterTypeAs<CampaignRepositoryAdapter, IRepositoryAdapter>("Campaigns");
            objectContainer.RegisterTypeAs<CampaignSettingsRepositoryAdapter, IRepositoryAdapter>("CampaignSettings");
            objectContainer.RegisterTypeAs<ClashRepositoryAdapter, IRepositoryAdapter>("Clashes");
            objectContainer.RegisterTypeAs<ClashExceptionRepositoryAdapter, IRepositoryAdapter>("ClashException");
            objectContainer.RegisterTypeAs<ChannelsRepositoryAdapter, IRepositoryAdapter>("Channels");
            objectContainer.RegisterTypeAs<ClearanceRepositoryAdapter, IRepositoryAdapter>("Clearance");
            objectContainer.RegisterTypeAs<DemographicRepositoryAdapter, IRepositoryAdapter>("Demographics");
            objectContainer.RegisterTypeAs<EfficiencySettingsRepositoryAdapter, IRepositoryAdapter>("EfficiencySettings");
            objectContainer.RegisterTypeAs<EmailAuditEventSettingsRepositoryAdapter, IRepositoryAdapter>("EmailAuditEventSettings");
            objectContainer.RegisterTypeAs<FacilityRepositoryAdapter, IRepositoryAdapter>("Facilities");
            objectContainer.RegisterTypeAs<FailuresRepositoryAdapter, IRepositoryAdapter>("Failures");
            objectContainer.RegisterTypeAs<FlexibilityLevelRepositoryAdapter, IRepositoryAdapter>("FlexibilityLevels");
            objectContainer.RegisterTypeAs<FunctionalAreaRepositoryAdapter, IRepositoryAdapter>("FunctionalAreas");
            objectContainer.RegisterTypeAs<IndexTypeRepositoryAdapter, IRepositoryAdapter>("IndexTypes");
            objectContainer.RegisterTypeAs<InventoryLockRepositoryAdapter, IRepositoryAdapter>("InventoryLocks");
            objectContainer.RegisterTypeAs<InventoryTypeRepositoryAdapter, IRepositoryAdapter>("InventoryTypes");
            objectContainer.RegisterTypeAs<ISRSettingsRepositoryAdapter, IRepositoryAdapter>("ISRSettings");
            objectContainer.RegisterTypeAs<ISRGlobalSettingsRepositoryAdapter, IRepositoryAdapter>("ISRGlobalSettings");
            objectContainer.RegisterTypeAs<KPIComparisonConfigRepositoryAdapter, IRepositoryAdapter>("KPIComparisonConfig");
            objectContainer.RegisterTypeAs<LanguageRepositoryAdapter, IRepositoryAdapter>("Languages");
            objectContainer.RegisterTypeAs<LibrarySalesAreaPassPrioritiesRepositoryAdapter, IRepositoryAdapter>("LibrarySalesAreaPassPriorities");
            objectContainer.RegisterTypeAs<LockTypeRepositoryAdapter, IRepositoryAdapter>("LockTypes");
            objectContainer.RegisterTypeAs<MetadataRepositoryAdapter, IRepositoryAdapter>("Metadata");
            objectContainer.RegisterTypeAs<MSTeamsAuditEventSettingsAdapter, IRepositoryAdapter>("MSTeamsAuditEventSettings");
            objectContainer.RegisterTypeAs<OutputFilesRepositoryAdapter, IRepositoryAdapter>("OutputFiles");
            objectContainer.RegisterTypeAs<PassRepositoryAdapter, IRepositoryAdapter>("Passes");
            objectContainer.RegisterTypeAs<ProductRepositoryAdapter, IRepositoryAdapter>("Products");
            objectContainer.RegisterTypeAs<ProgrammeRepositoryAdapter, IRepositoryAdapter>("Programmes");
            objectContainer.RegisterTypeAs<ProgrammeClassificationRepositoryAdapter, IRepositoryAdapter>("ProgrammeClassifications");
            objectContainer.RegisterTypeAs<ProgrammeDictionaryAdapter, IRepositoryAdapter>("ProgrammeDictionaries");
            objectContainer.RegisterTypeAs<RatingsScheduleRepositoryAdapter, IRepositoryAdapter>("RatingsSchedule");
            objectContainer.RegisterTypeAs<RecommendationsRepositoryAdapter, IRepositoryAdapter>("Recommendations");
            objectContainer.RegisterTypeAs<RestrictionsRepositoryAdapter, IRepositoryAdapter>("Restrictions");
            objectContainer.RegisterTypeAs<ResultsFileRepositoryAdapter, IRepositoryAdapter>("ResultFiles");
            objectContainer.RegisterTypeAs<RSSettingsRepositoryAdapter, IRepositoryAdapter>("RSSettings");
            objectContainer.RegisterTypeAs<RSGlobalSettingsRepositoryAdapter, IRepositoryAdapter>("RSGlobalSettings");
            objectContainer.RegisterTypeAs<RunRepositoryAdapter, IRepositoryAdapter>("Runs");
            objectContainer.RegisterTypeAs<RuleRepositoryAdapter, IRepositoryAdapter>("Rules");
            objectContainer.RegisterTypeAs<RuleTypeRepositoryAdapter, IRepositoryAdapter>("RuleTypes");
            objectContainer.RegisterTypeAs<SalesAreaRepositoryAdapter, IRepositoryAdapter>("SalesArea");
            objectContainer.RegisterTypeAs<ScenariosRepositoryAdapter, IRepositoryAdapter>("Scenarios");
            objectContainer.RegisterTypeAs<ScenarioResultsRepositoryAdapter, IRepositoryAdapter>("ScenarioResults");
            objectContainer.RegisterTypeAs<ScheduleRepositoryAdapter, IRepositoryAdapter>("Schedules");
            objectContainer.RegisterTypeAs<SmoothConfigurationRepositoryAdapter, IRepositoryAdapter>("SmoothConfiguration");
            objectContainer.RegisterTypeAs<SmoothFailuresRepositoryAdapter, IRepositoryAdapter>("SmoothFailures");
            objectContainer.RegisterTypeAs<SpotPlacementRepositoryAdapter, IRepositoryAdapter>("SpotPlacement");
            objectContainer.RegisterTypeAs<SpotRepositoryAdapter, IRepositoryAdapter>("Spots");
            objectContainer.RegisterTypeAs<TaskInstanceRepositoryAdapter, IRepositoryAdapter>("TaskInstance");
            objectContainer.RegisterTypeAs<TenantsRepositoryAdapter, IRepositoryAdapter>("Tenants");
            objectContainer.RegisterTypeAs<TenantSettingsRepositoryAdapter, IRepositoryAdapter>("TenantSettings");
            objectContainer.RegisterTypeAs<UniverseRepositoryAdapter, IRepositoryAdapter>("Universes");
            objectContainer.RegisterTypeAs<UpdateDetailsRepositoryAdapter, IRepositoryAdapter>("UpdateDetails");
            objectContainer.RegisterTypeAs<UsersRepositoryAdapter, IRepositoryAdapter>("Users");
            objectContainer.RegisterTypeAs<SponsorshipsRepositoryAdapter, IRepositoryAdapter>("Sponsorships");
            objectContainer.RegisterTypeAs<ScenarioCampaignResultRepositoryAdapter, IRepositoryAdapter>("ScenarioCampaignResults");
            objectContainer.RegisterTypeAs<ScenarioCampaignFailureRepositoryAdapter, IRepositoryAdapter>("ScenarioCampaignFailures");
            objectContainer.RegisterTypeAs<ScenarioCampaignMetricRepositoryAdapter, IRepositoryAdapter>("ScenarioCampaignMetrics");
            objectContainer.RegisterTypeAs<DeliveryCappingGroupRepositoryAdapter, IRepositoryAdapter>("DeliveryCappingGroup");
            objectContainer.RegisterTypeAs<SalesAreaDemographicRepositoryAdapter, IRepositoryAdapter>("SalesAreaDemographic");
            objectContainer.RegisterTypeAs<ProgrammeCategoryRepositoryAdapter, IRepositoryAdapter>("ProgrammeCategories");
            objectContainer.RegisterTypeAs<TotalRatingRepositoryAdapter, IRepositoryAdapter>("TotalRatings");
            objectContainer.RegisterTypeAs<StandardDayPartRepositoryAdapter, IRepositoryAdapter>("StandardDayParts");
            objectContainer.RegisterTypeAs<StandardDayPartGroupRepositoryAdapter, IRepositoryAdapter>("StandardDayPartGroups");
            objectContainer.RegisterTypeAs<ProgrammeEpisodeRepositoryAdapter, IRepositoryAdapter>("ProgrammeEpisodes");
            objectContainer.RegisterTypeAs<RunTypeRepositoryAdapter, IRepositoryAdapter>("RunTypes");
            objectContainer.RegisterTypeAs<SpotBookingRuleRepositoryAdapter, IRepositoryAdapter>("SpotBookingRules");
            objectContainer.RegisterTypeAs<LegnthFactorRepositoryAdapter, IRepositoryAdapter>("LengthFactor");
        }
    }
}
