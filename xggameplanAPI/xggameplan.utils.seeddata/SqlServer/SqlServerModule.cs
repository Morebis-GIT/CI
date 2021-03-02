using System;
using Autofac;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using xggameplan.AuditEvents;
using xggameplan.utils.seeddata.Migration.RavenToSql;
using xggameplan.utils.seeddata.SqlServer.DomainModelHandlers;

namespace xggameplan.utils.seeddata.SqlServer
{
    public class SqlServerModule : SqlServerModule<SqlServerMasterDbContext, SqlServerLongRunningTenantDbContext>
    {
        public SqlServerModule(DatabaseType databaseType, string connectionString) :
            base(databaseType, connectionString)
        {
        }

        public SqlServerModule(
            DatabaseType databaseType,
            string connectionString,
            SqlServerDbContextRegistrationFeatures features) :
            base(databaseType, connectionString, features)
        {
        }
    }

    public class SqlServerModule<TMasterDbContext, TTenantDbContext> : Module
        where TMasterDbContext : SqlServerMasterDbContext, ISqlServerMasterDbContext
        where TTenantDbContext : SqlServerLongRunningTenantDbContext, ISqlServerTenantDbContext
    {
        private readonly DatabaseType _databaseType;
        private readonly string _connectionString;
        private readonly SqlServerDbContextRegistrationFeatures _features;

        public SqlServerModule(DatabaseType databaseType, string connectionString) :
            this(databaseType, connectionString, new SqlServerDbContextRegistrationFeatures())
        {
        }

        public SqlServerModule(
            DatabaseType databaseType,
            string connectionString,
            SqlServerDbContextRegistrationFeatures features)
        {
            _databaseType = databaseType;
            _connectionString = connectionString;
            _features = features ?? throw new ArgumentNullException(nameof(features));
        }

        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterModule<SqlServerInfrastructureModule>();

            switch (_databaseType)
            {
                case DatabaseType.Master:
                    _ = builder.RegisterModule(new SqlServerMasterModule<TMasterDbContext>(_connectionString, _features));
                    break;

                case DatabaseType.Tenant:
                    _ = builder.RegisterModule(new SqlServerTenantModule<TTenantDbContext, TTenantDbContext>(_connectionString, _features));
                    break;
            }

            _ = builder.RegisterType<SqlServerDomainModelContext>()
                .As<IDomainModelContext>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SeedDomainModelHandlerResolver>()
                .As<IDomainModelHandlerResolver>()
                .InstancePerLifetimeScope();

            // master domain model handlers
            _ = builder.RegisterType<AccessTokenDomainModelHandler>()
                .As<IDomainModelHandler<AccessToken>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<UsersDomainModelHandler>()
               .As<IDomainModelHandler<User>>()
               .InstancePerLifetimeScope();

            _ = builder.RegisterType<TenantsDomainModelHandler>()
               .As<IDomainModelHandler<Tenant>>()
               .InstancePerLifetimeScope();

            _ = builder.RegisterType<TenantProductFeatureDomainModelHandler>()
                .As<IDomainModelHandler<TenantProductFeature>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ProductSettingsDomainModelHandler>()
               .As<IDomainModelHandler<ProductSettings>>()
               .InstancePerLifetimeScope();

            _ = builder.RegisterType<TaskInstanceDomainModelHandler>()
               .As<IDomainModelHandler<TaskInstance>>()
               .InstancePerLifetimeScope();

            _ = builder.RegisterType<UpdateDetailsDomainModelHandler>()
               .As<IDomainModelHandler<UpdateDetails>>()
               .InstancePerLifetimeScope();

            //tenant domain model handlers
            _ = builder.RegisterType<AutoBookDomainModelHandler>()
                .As<IDomainModelHandler<AutoBook>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<AutoBookInstanceConfigurationDomainModelHandler>()
                .As<IDomainModelHandler<AutoBookInstanceConfiguration>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<AutoBookSettingsDomainModelHandler>()
                .As<IDomainModelHandler<AutoBookSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<BookingPositionDomainModelHandler>()
                .As<IDomainModelHandler<BookingPosition>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<BookingPositionGroupDomainModelHandler>()
                .As<IDomainModelHandler<BookingPositionGroup>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<AutoBookDefaultParametersDomainModelHandler>()
                .As<IDomainModelHandler<AutoBookDefaultParameters>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<BreakDomainModelHandler>()
                .As<IDomainModelHandler<Break>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<BusinessTypeDomainModelHandler>()
                .As<IDomainModelHandler<BusinessType>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<CampaignDomainModelHandler>()
                .As<IDomainModelHandler<Campaign>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<CampaignSettingsDomainModelHandler>()
                .As<IDomainModelHandler<CampaignSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ChannelDomainModelHandler>()
                .As<IDomainModelHandler<Channel>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ClashDomainModelHandler>()
                .As<IDomainModelHandler<Clash>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ClashExceptionDomainModelHandler>()
                .As<IDomainModelHandler<ClashException>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ClearanceDomainModelHandler>()
                .As<IDomainModelHandler<ClearanceCode>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<DemographicDomainModelHandler>()
                .As<IDomainModelHandler<Demographic>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<EfficiencySettingsDomainModelHandler>()
                .As<IDomainModelHandler<EfficiencySettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<EmailAuditEventSettingsDomainModelHandler>()
                .As<IDomainModelHandler<EmailAuditEventSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<FacilityDomainModelHandler>()
                .As<IDomainModelHandler<Facility>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<IndexTypeDomainModelHandler>()
                .As<IDomainModelHandler<IndexType>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ISRSettingsDomainModelHandler>()
                .As<IDomainModelHandler<ISRSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RSSettingsDomainModelHandler>()
                .As<IDomainModelHandler<RSSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<KPIComparisonConfigDomainModelHandler>()
                .As<IDomainModelHandler<KPIComparisonConfig>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<LanguageDomainModelHandler>()
                .As<IDomainModelHandler<Language>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<LibrarySalesAreaPassPrioritiesDomainModelHandler>()
                .As<IDomainModelHandler<LibrarySalesAreaPassPriority>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<MetadataDomainModelHandler>()
                .As<IDomainModelHandler<Metadata>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<MSTeamsAuditEventSettingsDomainModelHandler>()
                .As<IDomainModelHandler<MSTeamsAuditEventSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<OutputFileDomainModelHandler>()
                .As<IDomainModelHandler<OutputFile>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<PassDomainModelHandler>()
                .As<IDomainModelHandler<Pass>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ProductDomainModelHandler>()
                .As<IDomainModelHandler<Product>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ProgrammeClassificationDomainModelHandler>()
                .As<IDomainModelHandler<ProgrammeClassification>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ProgrammeDictionaryDomainModelHandler>()
                .As<IDomainModelHandler<ProgrammeDictionary>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ProgrammeDomainModelHandler>()
                .As<IDomainModelHandler<Programme>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RatingsPredictionScheduleDomainModelHandler>()
                .As<IDomainModelHandler<RatingsPredictionSchedule>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RecommendationDomainModelHandler>()
                .As<IDomainModelHandler<Recommendation>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RestrictionDomainModelHandler>()
                .As<IDomainModelHandler<Restriction>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RunDomainModelHandler>()
                .As<IDomainModelHandler<Run>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SalesAreaDomainModelHandler>()
                .As<IDomainModelHandler<SalesArea>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ScheduleDomainModelHandler>()
                .As<IDomainModelHandler<Schedule>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SpotPlacementDomainModelHandler>()
                .As<IDomainModelHandler<SpotPlacement>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<TenantSettingsDomainModelHandler>()
                .As<IDomainModelHandler<TenantSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SpotDomainModelHandler>()
                .As<IDomainModelHandler<Spot>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ScenarioDomainModelHandler>()
                .As<IDomainModelHandler<Scenario>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<UniverseDomainModelHandler>()
                .As<IDomainModelHandler<Universe>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<FunctionalAreaDomainModelHandler>()
                .As<IDomainModelHandler<FunctionalArea>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ScenarioResultDomainModelHandler>()
                .As<IDomainModelHandler<ScenarioResult>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SmoothFailureDomainModelHandler>()
                .As<IDomainModelHandler<SmoothFailure>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SmoothFailureMessageDomainModelHandler>()
                .As<IDomainModelHandler<SmoothFailureMessage>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SponsorshipDomainModelHandler>()
                .As<IDomainModelHandler<Sponsorship>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ResultFileDomainModelHandler>()
                .As<IDomainModelHandler<ResultFileMigrationDocumentHandler.ResultFile>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<SmoothConfigurationDomainModelHandler>()
                .As<IDomainModelHandler<SmoothConfiguration>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<FailureDomainModelHandler>()
                .As<IDomainModelHandler<Failures>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ISRGlobalSettingsDomainModelHandler>()
                .As<IDomainModelHandler<ISRGlobalSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RSGlobalSettingsDomainModelHandler>()
                .As<IDomainModelHandler<RSGlobalSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<AutopilotSettingsDomainModelHandler>()
                .As<IDomainModelHandler<AutopilotSettings>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<AutopilotRuleDomainModelHandler>()
                .As<IDomainModelHandler<AutopilotRule>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RuleDomainModelHandler>()
                .As<IDomainModelHandler<Rule>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<RuleTypeDomainModelHandler>()
                .As<IDomainModelHandler<RuleType>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<FlexibilityLevelDomainModelHandler>()
                .As<IDomainModelHandler<FlexibilityLevel>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ScenarioCampaignResultDomainModelHandler>()
                .As<IDomainModelHandler<ScenarioCampaignResult>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ScenarioCampaignMetricDomainModelHandler>()
                .As<IDomainModelHandler<ScenarioCampaignMetric>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<ProgrammeCategoryHierarchyModelHandler>()
                .As<IDomainModelHandler<ProgrammeCategoryHierarchy>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<BRSConfigurationTemplateDomainModelHandler>()
                .As<IDomainModelHandler<BRSConfigurationTemplate>>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<KPIPriorityDomainModelHandler>()
                .As<IDomainModelHandler<KPIPriority>>()
                .InstancePerLifetimeScope();
        }
    }
}
