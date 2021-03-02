using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AnalysisGroups;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BookingPositionGroups;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BRS;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.BusinessTypes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ClashExceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayPartGroups;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayParts;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Failures;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.FunctionalAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.InventoryStatuses;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ISRSettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.OutputFiles;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Restrictions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.RSSettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Sponsorships;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SpotBookingRules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.TenantSettings;
using Microsoft.EntityFrameworkCore;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerTestDbContext : SqlServerDbContext, ISqlServerTestDbContext
    {
        public SqlServerTestDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public override void Dispose()
        {
            if (CreatedByFactoryCounter > 0)
            {
                CreatedByFactoryCounter--;
            }
            else
            {
                base.Dispose();
            }
        }

        internal int CreatedByFactoryCounter { get; set; }

        protected override SqlServerSpecificDbAdapter CreateSpecificDbAdapter() => new SqlServerTestSpecificDbAdapter(this);

        protected override ISqlServerBulkInsertEngine CreateBulkInsertEngine() => new SqlServerTestBulkInsertEngine(this);

        protected override bool UseSqlCommandForTruncate => false;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // master db entities
            _ = modelBuilder.ApplyConfiguration(new AccessTokenEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PreviewFileEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProductSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProductSettingFeatureEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TaskInstanceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TaskInstanceParameterEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantProductFeatureEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantProductFeatureReferenceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UpdateDetailsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UserSettingEntityConfiguration());

            // tenant db entities
            _ = modelBuilder.ApplyConfiguration(new AdvertiserEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgencyEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgencyGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgAvalEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgCampaignProgrammeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgCampaignProgrammeProgrammeCategoryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgCampaignSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgDayPartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgDayPartLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgHfssDemoEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgMultiPartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgPartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgPartLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgPredictionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgRegionalBreakEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgStrikeWeightEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgStrikeWeightLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgTimeBandEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AgTimeSliceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AnalysisGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AnalysisGroupTargetMetricEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutoBookDefaultParametersEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutoBookEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutoBookInstanceConfigurationEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutoBookInstanceConfigurationCriteriaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutoBookSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutoBookTaskEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutopilotSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AutopilotRuleEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new AWSInstanceConfigurationEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BestBreakDefaultFactorEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BestBreakFactorGroupEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BestBreakFactorGroupItemEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BestBreakFactorGroupRecordEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BestBreakFactorGroupRecordPassSequenceItemEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BestBreakFilterFactorEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BookingPositionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BookingPositionGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BreakEfficiencyConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BRSConfigurationForKPIEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BRSConfigurationTemplateEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BreakEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new BusinessTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignBookingPositionGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignBookingPositionGroupSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignBreakTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignBreakRequirementEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignBreakRequirementItemEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignProgrammeRestrictionCategoryOrProgrammeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignProgrammeRestrictionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignProgrammeRestrictionSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignSalesAreaTargetEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignSalesAreaTargetGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignSalesAreaTargetGroupSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignSalesAreaTargetMultipartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignSalesAreaTargetMultipartLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTargetEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTargetStrikeWeightDayPartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTargetStrikeWeightDayPartLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTargetStrikeWeightDayPartTimeSliceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTargetStrikeWeightEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTargetStrikeWeightLengthEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTimeRestrictionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new CampaignTimeRestrictionSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ChannelEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ClashEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ClashDifferenceEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ClashDifferenceTimeAndDowEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ClashExceptionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ClashExceptionsTimeAndDowEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ClearanceCodeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new DeliveryCappingGroupConfiguration());
            _ = modelBuilder.ApplyConfiguration(new DemographicEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new EfficiencySettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new EmailAuditEventSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ExternalRunInfoEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FacilityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FaultTypeDescriptionEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FaultTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FlexibilityLevelEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FunctionalAreaDescriptionEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FunctionalAreaEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FunctionalAreaFaultTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FailureEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new FailureItemEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new IndexTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new InventoryTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new InventoryTypeLockTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new InventoryLockEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ISRDemographicSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ISRGlobalSettingsEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ISRSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new KPIComparisonConfigEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new KPIPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new LanguageEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new LibrarySalesAreaPassPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new MetadataCategoryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new LockTypeItemEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new MetadataCategoryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new LengthFactorEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new MetadataValueEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new MSTeamsAuditEventSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new OutputFileColumnEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new OutputFileEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PositionGroupAssociationEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PredictionScheduleEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PredictionScheduleRatingEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassBreakExclusionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassProgrammeRepetitionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassRatingPointEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassRuleEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassSalesAreaPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PassSlottingLimitEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PersonEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProductAdvertiserEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProductAgencyEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProductPersonEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammeCategoryHierarchyEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammeEpisodeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammesClassificationEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammesDictionaryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammeCategoryEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ProgrammeCategoryLinkEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RecommendationEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RestrictionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RestrictionSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ResultsFileEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RSDemographicSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RSGlobalSettingsEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RSSettingsDefaultDeliverySettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RSSettingsDemographicsDeliverySettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RSSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RuleEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RuleTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunAnalysisGroupTargetEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunAuthorEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunCampaignProcessesSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunCampaignReferenceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunInventoryLockEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunSalesAreaPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunScenarioEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunTypeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunTypeAnalysisGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SalesAreaDemographicEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SalesAreaPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SameBreakGroupScoreFactorEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCampaignPassPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCampaignResultEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCampaignMetricEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCampaignFailureEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCampaignPriorityRoundCollectionEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCampaignPriorityRoundEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCompactCampaignEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioCompactCampaignPaybackEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioPassReferenceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioPassPriorityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioResultEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScenarioResultMetricEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScheduleEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScheduleBreakEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScheduleBreakEfficiencyEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new ScheduleProgrammeEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothConfigurationEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothDiagnosticConfigurationEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothFailureEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothFailureMessageDescriptionEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothFailureMessageEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothFailuresSmoothFailureMessagesEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassDefaultEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassDefaultIterationEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassBookedEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassUnplacedEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassUnplacedIterationEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassIterationRecordEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SmoothPassIterationRecordPassSequenceItemEntityTypeConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SponsoredDayPartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SponsoredItemEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SponsorshipAdvertiserExclusivityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SponsorshipClashExclusivityEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SponsorshipEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SponsorshipItemEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SpotEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SpotPlacementEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new StandardDayPartEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new StandardDayPartTimesliceEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new StandardDayPartGroupEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new StandardDayPartSplitEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantRunEventSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantFeatureSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TenantWebhookSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new TotalRatingEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UniverseEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SpotBookingRuleEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new SpotBookingRuleSalesAreaEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunLandmarkScheduleSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RunScheduleSettingsEntityConfiguration());
            _ = modelBuilder.ApplyConfiguration(new LandmarkRunQueueEntityConfiguration());
        }
    }
}
