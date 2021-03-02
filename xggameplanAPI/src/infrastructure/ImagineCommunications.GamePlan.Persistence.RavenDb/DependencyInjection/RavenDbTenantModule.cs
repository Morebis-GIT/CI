using System.Reflection;
using Autofac;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;
using Raven.Client;
using Raven.Client.FileSystem;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection
{
    public class RavenDbTenantModule : Autofac.Module
    {
        private readonly string _connectionString;

        public RavenDbTenantModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.Register(context =>
                      DocumentStoreFactory.CreateStore(_connectionString, Assembly.GetExecutingAssembly()))
                .As<IDocumentStore>()
                .SingleInstance();

            _ = builder.Register(context => context.Resolve<IDocumentStore>())
                .Keyed<IDocumentStore>(DatabaseType.Tenant)
                .SingleInstance();

            _ = builder.Register(context => FileStoreFactory.CreateStore(_connectionString))
                .As<IFilesStore>()
                .SingleInstance();

            _ = builder.Register(context => context.Resolve<IFilesStore>())
                .Keyed<IFilesStore>(DatabaseType.Tenant)
                .SingleInstance();

            _ = builder.Register(x => x.Resolve<IDocumentStore>().OpenSession())
                .As<IDocumentSession>()
                .InstancePerLifetimeScope();

            _ = builder.Register(x => x.Resolve<IDocumentSession>())
                .Keyed<IDocumentSession>(DatabaseType.Tenant)
                .InstancePerLifetimeScope();

            _ = builder.Register(x => x.Resolve<IDocumentStore>().OpenAsyncSession())
                .As<IAsyncDocumentSession>()
                .InstancePerLifetimeScope();

            _ = builder.Register(x => x.Resolve<IAsyncDocumentSession>())
                .Keyed<IAsyncDocumentSession>(DatabaseType.Tenant)
                .InstancePerLifetimeScope();

            _ = builder.Register(x => x.Resolve<IFilesStore>().OpenAsyncSession())
                .As<IAsyncFilesSession>()
                .InstancePerLifetimeScope();

            _ = builder.Register(x => x.Resolve<IAsyncFilesSession>())
                .Keyed<IAsyncFilesSession>(DatabaseType.Tenant)
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<TenantRavenDbContext>()
                .As<IRavenTenantDbContext>()
                .As<IRavenDbContext>()
                .As<ITenantDbContext>()
                .As<IDbContext>()
                .WithRavenDbMetadata()
                .InstancePerLifetimeScope();

            // Register Raven repositories
            _ = builder.RegisterRepository<RavenCampaignRepository, ICampaignRepository>();
            _ = builder.RegisterRepository<RavenCampaignSettingsRepository, ICampaignSettingsRepository>();
            _ = builder.RegisterRepository<RavenBreakRepository, IBreakRepository>();
            _ = builder.RegisterRepository<RavenBusinessTypeRepository, IBusinessTypeRepository>();
            _ = builder.RegisterRepository<RavenScheduleRepository, IScheduleRepository>();
            _ = builder.RegisterRepository<RavenProgrammeRepository, IProgrammeRepository>();
            _ = builder.RegisterRepository<RavenProgrammeDictionaryRepository, IProgrammeDictionaryRepository>();
            _ = builder.RegisterRepository<RavenAutoBookRepository, IAutoBookRepository>();
            _ = builder.RegisterRepository<RavenRunRepository, IRunRepository>();
            _ = builder.RegisterRepository<RavenSpotRepository, ISpotRepository>();
            _ = builder.RegisterRepository<RavenClashRepository, IClashRepository>();
            _ = builder.RegisterRepository<RavenClashExceptionRepository, IClashExceptionRepository>();
            _ = builder.RegisterRepository<RavenChannelRepository, IChannelsRepository>();
            _ = builder.RegisterRepository<RavenSalesAreaRepository, ISalesAreaRepository>();
            _ = builder.RegisterRepository<RavenScenarioResultRepository, IScenarioResultRepository>();
            _ = builder.RegisterRepository<RavenMetadataRepository, IMetadataRepository>();
            _ = builder.RegisterRepository<RavenTenantSettingsRepository, ITenantSettingsRepository>();
            _ = builder.RegisterRepository<RavenProductRepository, IProductRepository>();
            _ = builder.RegisterRepository<RavenFunctionalAreaRepository, IFunctionalAreaRepository>();
            _ = builder.RegisterRepository<RavenLanguageRepository, ILanguageRepository>();
            _ = builder.RegisterRepository<RavenRestrictionRepository, IRestrictionRepository>();
            _ = builder.RegisterRepository<RavenRatingsScheduleRepository, IRatingsScheduleRepository>();
            _ = builder.RegisterRepository<RavenResultsFileRepository, IResultsFileRepository>();
            _ = builder.RegisterRepository<RavenOutputFileRepository, IOutputFileRepository>();
            _ = builder.RegisterRepository<RavenUniverseRepository, IUniverseRepository>();
            _ = builder.RegisterRepository<RavenFailuresRepository, IFailuresRepository>();
            _ = builder.RegisterRepository<RavenScenarioCampaignFailureRepository, IScenarioCampaignFailureRepository>();
            _ = builder.RegisterRepository<RavenRecommendationRepository, IRecommendationRepository>();
            _ = builder.RegisterRepository<RavenISRSettingsRepository, IISRSettingsRepository>();
            _ = builder.RegisterRepository<RavenRSSettingsRepository, IRSSettingsRepository>();
            _ = builder.RegisterRepository<RavenISRGlobalSettingsRepository, IISRGlobalSettingsRepository>();
            _ = builder.RegisterRepository<RavenRSGlobalSettingsRepository, IRSGlobalSettingsRepository>();
            _ = builder.RegisterRepository<RavenDemographicRepository, IDemographicRepository>();
            _ = builder.RegisterRepository<RavenSmoothFailureMessageRepository, ISmoothFailureMessageRepository>();
            _ = builder.RegisterRepository<RavenSmoothFailureRepository, ISmoothFailureRepository>();
            _ = builder.RegisterRepository<RavenAutoBookSettingsRepository, IAutoBookSettingsRepository>();
            _ = builder.RegisterRepository<RavenAutoBookDefaultParametersRepository, IAutoBookDefaultParametersRepository>();
            _ = builder.RegisterRepository<RavenAutoBookInstanceConfigurationRepository, IAutoBookInstanceConfigurationRepository>();
            _ = builder.RegisterRepository<RavenAWSInstanceConfigurationRepository, IAWSInstanceConfigurationRepository>();
            _ = builder.RegisterRepository<RavenEmailAuditEventSettingsRepository, IEmailAuditEventSettingsRepository>();
            _ = builder.RegisterRepository<RavenMSTeamsAuditEventSettingsRepository, IMSTeamsAuditEventSettingsRepository>();
            _ = builder.RegisterRepository<RavenProgrammeClassificationRepository, IProgrammeClassificationRepository>();
            _ = builder.RegisterRepository<RavenClearanceRepository, IClearanceRepository>();
            _ = builder.RegisterRepository<RavenKPIComparisonConfigRepository, IKPIComparisonConfigRepository>();
            _ = builder.RegisterRepository<RavenScenarioRepository, IScenarioRepository>();
            _ = builder.RegisterRepository<RavenPassRepository, IPassRepository>();
            _ = builder.RegisterRepository<RavenSmoothConfigurationRepository, ISmoothConfigurationRepository>();
            _ = builder.RegisterRepository<RavenIndexTypeRepository, IIndexTypeRepository>();
            _ = builder.RegisterRepository<RavenLibrarySalesAreaPassPrioritiesRepository, ILibrarySalesAreaPassPrioritiesRepository>();
            _ = builder.RegisterRepository<RavenEfficiencySettingsRepository, IEfficiencySettingsRepository>();
            _ = builder.RegisterRepository<RavenSponsorshipRepository, ISponsorshipRepository>();
            _ = builder.RegisterRepository<RavenSpotPlacementRepository, ISpotPlacementRepository>();
            _ = builder.RegisterRepository<RavenAnalysisGroupRepository, IAnalysisGroupRepository>();
            _ = builder.RegisterRepository<RavenAutopilotRuleRepository, IAutopilotRuleRepository>();
            _ = builder.RegisterRepository<RavenAutopilotSettingsRepository, IAutopilotSettingsRepository>();
            _ = builder.RegisterRepository<RavenFlexibilityLevelRepository, IFlexibilityLevelRepository>();
            _ = builder.RegisterRepository<RavenRuleRepository, IRuleRepository>();
            _ = builder.RegisterRepository<RavenRuleTypeRepository, IRuleTypeRepository>();
            _ = builder.RegisterRepository<RavenScenarioCampaignResultsRepository, IScenarioCampaignResultRepository>();
            _ = builder.RegisterRepository<RavenScenarioCampaignMetricRepository, IScenarioCampaignMetricRepository>();
            _ = builder.RegisterRepository<RavenBookingPositionRepository, IBookingPositionRepository>();
            _ = builder.RegisterRepository<RavenBookingPositionGroupRepository, IBookingPositionGroupRepository>();
            _ = builder.RegisterRepository<RavenFacilityRepository, IFacilityRepository>();
            _ = builder.RegisterRepository<RavenDeliveryCappingGroupRepository, IDeliveryCappingGroupRepository>();
            _ = builder.RegisterRepository<RavenSalesAreaDemographicRepository, ISalesAreaDemographicRepository>();
            _ = builder.RegisterRepository<RavenProgrammeCategoryHierarchyRepository, IProgrammeCategoryHierarchyRepository>();
            _ = builder.RegisterRepository<RavenStandardDayPartRepository, IStandardDayPartRepository>();
            _ = builder.RegisterRepository<RavenStandardDayPartGroupRepository, IStandardDayPartGroupRepository>();
            _ = builder.RegisterRepository<RavenProgrammeEpisodeRepository, IProgrammeEpisodeRepository>();
            _ = builder.RegisterRepository<RavenInventoryTypeRepository, IInventoryTypeRepository>();
            _ = builder.RegisterRepository<RavenInventoryLockRepository, IInventoryLockRepository>();
            _ = builder.RegisterRepository<RavenLockTypeRepository, ILockTypeRepository>();
            _ = builder.RegisterRepository<RavenTotalRatingRepository, ITotalRatingRepository>();
            _ = builder.RegisterRepository<RavenPipelineAuditEventRepository, IPipelineAuditEventRepository>();
            _ = builder.RegisterRepository<RavenAutoBookTaskReportRepository, IAutoBookTaskReportRepository>();
            _ = builder.RegisterRepository<RavenBRSConfigurationTemplateRepository, IBRSConfigurationTemplateRepository>();
            _ = builder.RegisterRepository<RavenKPIPriorityRepository, IKPIPriorityRepository>();
            _ = builder.RegisterRepository<RavenRunTypeRepository, IRunTypeRepository>();
            _ = builder.RegisterRepository<RavenSpotBookingRuleRepository, ISpotBookingRuleRepository>();
            _ = builder.RegisterRepository<RavenLengthFactorRepository, ILengthFactorRepository>();
            _ = builder.RegisterRepository<RavenLandmarkRunQueueRepository, ILandmarkRunQueueRepository>();

            _ = builder.RegisterType<RavenTenantIdentifierSequence>()
                .As<IRavenTenantIdentifierSequence>()
                .As<IRavenIdentifierSequence>()
                .As<ITenantIdentifierSequence>()
                .As<IIdentifierSequence>()
                .WithRavenDbMetadata()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RavenIdentityGenerator>()
                .As<IRavenIdentityGenerator>()
                .As<IIdentityGenerator>()
                .WithRavenDbMetadata()
                .InstancePerLifetimeScope();

            _ = builder.Register(context => new RavenDatabaseIndexAwaiter(context.Resolve<IRavenTenantDbContext>()))
                .As<IDatabaseIndexAwaiter>()
                .WithRavenDbMetadata()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RavenSalesAreaCleanupDeleteCommand>()
                .As<ISalesAreaCleanupDeleteCommand>()
                .WithRavenDbMetadata()
                .InstancePerLifetimeScope();
        }
    }
}
