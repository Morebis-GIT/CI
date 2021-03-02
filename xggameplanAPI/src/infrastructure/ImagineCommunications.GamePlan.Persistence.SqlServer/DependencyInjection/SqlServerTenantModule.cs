using System;
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
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.CampaignCleaning;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.RunCleaning;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection
{
    public class SqlServerTenantModule : SqlServerTenantModule<SqlServerTenantDbContext, SqlServerLongRunningTenantDbContext>
    {
        public SqlServerTenantModule(string connectionString) : base(connectionString)
        {
        }

        public SqlServerTenantModule(string connectionString, int longRunningCommandsTimeout)
            : base(connectionString, new SqlServerDbContextRegistrationFeatures(),
                new SqlServerDbContextRegistrationFeatures { CommandTimeout = longRunningCommandsTimeout })
        {
        }

        public SqlServerTenantModule(string connectionString,
            SqlServerDbContextRegistrationFeatures tenantContextFeatures,
            SqlServerDbContextRegistrationFeatures longRunningContextFeatures)
            : base(connectionString, tenantContextFeatures, longRunningContextFeatures)
        {
        }
    }

    public class SqlServerTenantModule<TDbContext, TLongRunningDbContext> : SqlServerAutofacModuleBase
        where TDbContext : SqlServerTenantDbContext, ISqlServerTenantDbContext
        where TLongRunningDbContext : SqlServerLongRunningTenantDbContext, ISqlServerLongRunningTenantDbContext
    {
        private readonly string _connectionString;
        private readonly bool _registerOnlyLongRunningDbContext;
        private readonly SqlServerDbContextRegistrationFeatures _tenantContextFeatures;
        private readonly SqlServerDbContextRegistrationFeatures _longRunningContextFeatures;

        public SqlServerTenantModule(string connectionString) :
            this(connectionString, new SqlServerDbContextRegistrationFeatures(), new SqlServerDbContextRegistrationFeatures())
        {
        }

        public SqlServerTenantModule(string connectionString, SqlServerDbContextRegistrationFeatures tenantContextFeatures)
        {
            _connectionString = connectionString;
            _registerOnlyLongRunningDbContext = true;
            _longRunningContextFeatures = tenantContextFeatures ?? throw new ArgumentNullException(nameof(tenantContextFeatures));
        }

        public SqlServerTenantModule(string connectionString, SqlServerDbContextRegistrationFeatures tenantContextFeatures, SqlServerDbContextRegistrationFeatures longRunningContextFeatures)
        {
            _connectionString = connectionString;
            _registerOnlyLongRunningDbContext = false;
            _tenantContextFeatures = tenantContextFeatures ?? throw new ArgumentNullException(nameof(tenantContextFeatures));
            _longRunningContextFeatures = longRunningContextFeatures ?? throw new ArgumentNullException(nameof(longRunningContextFeatures));
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_registerOnlyLongRunningDbContext)
            {
                RegisterDbContext<ISqlServerLongRunningTenantDbContext, TLongRunningDbContext>(builder, _longRunningContextFeatures);
                _ = builder.Register(x => x.Resolve<ISqlServerLongRunningTenantDbContext>())
                    .As<ISqlServerTenantDbContext>()
                    .WithSqlServerMetadata()
                    .InstancePerLifetimeScope();
            }
            else
            {
                RegisterDbContext<ISqlServerTenantDbContext, TDbContext>(builder, _tenantContextFeatures);
                RegisterDbContext<ISqlServerLongRunningTenantDbContext, TLongRunningDbContext>(builder, _longRunningContextFeatures);
            }

            _ = builder.RegisterRepository<AnalysisGroupRepository, IAnalysisGroupRepository>();
            _ = builder.RegisterRepository<AWSInstanceConfigurationRepository, IAWSInstanceConfigurationRepository>();
            _ = builder.RegisterRepository<AutoBookDefaultParametersRepository, IAutoBookDefaultParametersRepository>();
            _ = builder.RegisterRepository<AutoBookInstanceConfigurationRepository, IAutoBookInstanceConfigurationRepository>();
            _ = builder.RegisterRepository<AutoBookSettingsRepository, IAutoBookSettingsRepository>();
            _ = builder.RegisterRepository<AutoBookRepository, IAutoBookRepository>();
            _ = builder.RegisterRepository<BookingPositionRepository, IBookingPositionRepository>();
            _ = builder.RegisterRepository<BookingPositionGroupRepository, IBookingPositionGroupRepository>();
            _ = builder.RegisterRepository<BreakRepository, IBreakRepository>();
            _ = builder.RegisterRepository<BusinessTypeRepository, IBusinessTypeRepository>();
            _ = builder.RegisterRepository<CampaignRepository, ICampaignRepository>();
            _ = builder.RegisterRepository<CampaignSettingsRepository, ICampaignSettingsRepository>();
            _ = builder.RegisterRepository<ChannelRepository, IChannelsRepository>();
            _ = builder.RegisterRepository<ClashExceptionRepository, IClashExceptionRepository>();
            _ = builder.RegisterRepository<ClashRepository, IClashRepository>();
            _ = builder.RegisterRepository<ClearanceRepository, IClearanceRepository>();
            _ = builder.RegisterRepository<DeliveryCappingGroupRepository, IDeliveryCappingGroupRepository>();
            _ = builder.RegisterRepository<DemographicRepository, IDemographicRepository>();
            _ = builder.RegisterRepository<EfficiencySettingsRepository, IEfficiencySettingsRepository>();
            _ = builder.RegisterRepository<EmailAuditEventSettingsRepository, IEmailAuditEventSettingsRepository>();
            _ = builder.RegisterRepository<FunctionalAreaRepository, IFunctionalAreaRepository>();
            _ = builder.RegisterRepository<FailuresRepository, IFailuresRepository>();
            _ = builder.RegisterRepository<IndexTypeRepository, IIndexTypeRepository>();
            _ = builder.RegisterRepository<ISRSettingsRepository, IISRSettingsRepository>();
            _ = builder.RegisterRepository<KPIComparisonConfigRepository, IKPIComparisonConfigRepository>();
            _ = builder.RegisterRepository<LanguageRepository, ILanguageRepository>();
            _ = builder.RegisterRepository<LibrarySalesAreaPassPrioritiesRepository, ILibrarySalesAreaPassPrioritiesRepository>();
            _ = builder.RegisterRepository<LengthFactorRepository, ILengthFactorRepository>();
            _ = builder.RegisterRepository<MetadataRepository, IMetadataRepository>();
            _ = builder.RegisterRepository<MSTeamsAuditEventSettingsRepository, IMSTeamsAuditEventSettingsRepository>();
            _ = builder.RegisterRepository<OutputFileRepository, IOutputFileRepository>();
            _ = builder.RegisterRepository<PassRepository, IPassRepository>();
            _ = builder.RegisterRepository<ProductRepository, IProductRepository>();
            _ = builder.RegisterRepository<ProgrammeClassificationRepository, IProgrammeClassificationRepository>();
            _ = builder.RegisterRepository<ProgrammeDictionaryRepository, IProgrammeDictionaryRepository>();
            _ = builder.RegisterRepository<ProgrammeRepository, IProgrammeRepository>();
            _ = builder.RegisterRepository<RatingsScheduleRepository, IRatingsScheduleRepository>();
            _ = builder.RegisterRepository<RecommendationRepository, IRecommendationRepository>();
            _ = builder.RegisterRepository<RestrictionRepository, IRestrictionRepository>();
            _ = builder.RegisterRepository<ResultsFileRepository, IResultsFileRepository>();
            _ = builder.RegisterRepository<RSSettingsRepository, IRSSettingsRepository>();
            _ = builder.RegisterRepository<RunRepository, IRunRepository>();
            _ = builder.RegisterRepository<SalesAreaRepository, ISalesAreaRepository>();
            _ = builder.RegisterRepository<ScenarioCampaignFailureRepository, IScenarioCampaignFailureRepository>();
            _ = builder.RegisterRepository<ScenarioRepository, IScenarioRepository>();
            _ = builder.RegisterRepository<ScenarioResultRepository, IScenarioResultRepository>();
            _ = builder.RegisterRepository<ScenarioCampaignResultRepository, IScenarioCampaignResultRepository>();
            _ = builder.RegisterRepository<ScenarioCampaignMetricRepository, IScenarioCampaignMetricRepository>();
            _ = builder.RegisterRepository<ScheduleRepository, IScheduleRepository>();
            _ = builder.RegisterRepository<SmoothConfigurationRepository, ISmoothConfigurationRepository>();
            _ = builder.RegisterRepository<SmoothFailureMessageRepository, ISmoothFailureMessageRepository>();
            _ = builder.RegisterRepository<SmoothFailureRepository, ISmoothFailureRepository>();
            _ = builder.RegisterRepository<SponsorshipRepository, ISponsorshipRepository>();
            _ = builder.RegisterRepository<SpotRepository, ISpotRepository>();
            _ = builder.RegisterRepository<SpotPlacementRepository, ISpotPlacementRepository>();
            _ = builder.RegisterRepository<TenantSettingsRepository, ITenantSettingsRepository>();
            _ = builder.RegisterRepository<UniverseRepository, IUniverseRepository>();
            _ = builder.RegisterRepository<ISRGlobalSettingsRepository, IISRGlobalSettingsRepository>();
            _ = builder.RegisterRepository<RSGlobalSettingsRepository, IRSGlobalSettingsRepository>();
            _ = builder.RegisterType<AutopilotSettingsRepository>().As<IAutopilotSettingsRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<AutopilotRuleRepository>().As<IAutopilotRuleRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<RuleRepository>().As<IRuleRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<RuleTypeRepository>().As<IRuleTypeRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<FlexibilityLevelRepository>().As<IFlexibilityLevelRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterRepository<InventoryLockRepository, IInventoryLockRepository>();
            _ = builder.RegisterRepository<InventoryTypeRepository, IInventoryTypeRepository>();
            _ = builder.RegisterRepository<LockTypeRepository, ILockTypeRepository>();
            _ = builder.RegisterRepository<ProgrammeCategoryHierarchyRepository, IProgrammeCategoryHierarchyRepository>();
            _ = builder.RegisterRepository<TotalRatingRepository, ITotalRatingRepository>();
            _ = builder.RegisterRepository<SalesAreaDemographicRepository, ISalesAreaDemographicRepository>();
            _ = builder.RegisterRepository<FacilityRepository, IFacilityRepository>();
            _ = builder.RegisterRepository<StandardDayPartRepository, IStandardDayPartRepository>();
            _ = builder.RegisterRepository<StandardDayPartGroupRepository, IStandardDayPartGroupRepository>();
            _ = builder.RegisterRepository<ProgrammeEpisodeRepository, IProgrammeEpisodeRepository>();
            _ = builder.RegisterRepository<PipelineAuditEventRepository, IPipelineAuditEventRepository>();
            _ = builder.RegisterRepository<AutoBookTaskReportRepository, IAutoBookTaskReportRepository>();
            _ = builder.RegisterRepository<BRSConfigurationTemplateRepository, IBRSConfigurationTemplateRepository>();
            _ = builder.RegisterRepository<KPIPriorityRepository, IKPIPriorityRepository>();
            _ = builder.RegisterRepository<RunTypeRepository, IRunTypeRepository>();
            _ = builder.RegisterRepository<SpotBookingRuleRepository, ISpotBookingRuleRepository>();
            _ = builder.RegisterRepository<LandmarkRunQueueRepository, ILandmarkRunQueueRepository>();

            _ = builder.RegisterType<SqlServerTenantIdentifierSequence>()
                .As<ISqlServerTenantIdentifierSequence>()
                .As<ISqlServerIdentifierSequence>()
                .As<ITenantIdentifierSequence>()
                .As<IIdentifierSequence>()
                .WithSqlServerMetadata()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SqlServerIdentityGenerator>()
                .Keyed<IIdentityGenerator>(typeof(IRunRepository))
                .As<ISqlServerIdentityGenerator>()
                .As<IIdentityGenerator>()
                .WithSqlServerMetadata()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ResultsFileTenantDbStorage>()
                .As<IResultsFileStorage>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SqlServerSalesAreaCleanupDeleteCommand>()
                .As<ISalesAreaCleanupDeleteCommand>()
                .WithSqlServerMetadata()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RunCleaner>().As<IRunCleaner>().InstancePerLifetimeScope();
            _ = builder.RegisterType<SpotCleaner>().As<ISpotCleaner>().InstancePerLifetimeScope();
            _ = builder.RegisterType<CampaignCleaner>().As<ICampaignCleaner>().InstancePerLifetimeScope();

            _ = builder.RegisterType<SalesAreaCacheAccessor>()
                .As<ISqlServerSalesAreaByIdCacheAccessor>()
                .As<ISqlServerSalesAreaByNullableIdCacheAccessor>()
                .As<ISqlServerSalesAreaByNameCacheAccessor>()
                .InstancePerLifetimeScope();
        }

        private void RegisterDbContext<TContextAbstraction, TContextImplementation>(ContainerBuilder builder, SqlServerDbContextRegistrationFeatures features)
            where TContextAbstraction : ISqlServerDbContext
            where TContextImplementation : SqlServerTenantDbContext, TContextAbstraction
        {
            _ = builder.RegisterModule(
                new SqlServerDbContextOptionsModule<TContextImplementation>(_connectionString, features));

            _ = builder.Register(context =>
                      new SqlServerDbContextFactory<TContextAbstraction, TContextImplementation>(
                          context.Resolve<DbContextOptions<TContextImplementation>>()))
                .As<ISqlServerDbContextFactory<TContextAbstraction>>()
                .InstancePerLifetimeScope();

            _ = builder.Register(context => context.Resolve<ISqlServerDbContextFactory<TContextAbstraction>>().Create())
                .As<TContextAbstraction>()
                .As<ISqlServerDbContext>()
                .As<ITenantDbContext>()
                .As<IDbContext>()
                .WithSqlServerMetadata()
                .InstancePerLifetimeScope();

            _ = builder.Register(context => new SqlServerDatabaseIndexAwaiter(context.Resolve<TContextAbstraction>()))
                .As<IDatabaseIndexAwaiter>()
                .WithSqlServerMetadata()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RecalculateBreakAvailabilityService>().As<IRecalculateBreakAvailabilityService>()
                .InstancePerLifetimeScope();
        }
    }
}
