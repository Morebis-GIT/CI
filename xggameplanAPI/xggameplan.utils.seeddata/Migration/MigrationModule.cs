using Autofac;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
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
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.common.Caching;
using xggameplan.core.Extensions;
using xggameplan.utils.seeddata.Migration.RavenToSql;
using xggameplan.utils.seeddata.RavenDb;
using xggameplan.utils.seeddata.SqlServer;
using xggameplan.utils.seeddata.SqlServer.Migration;
using xggameplan.utils.seeddata.SqlServer.Migration.DbContext;
using xggameplan.utils.seeddata.SqlServer.Migration.Interfaces;
using AutoBookSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookSettings;
using AutopilotRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotRule;
using AutopilotSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotSettings;
using BookingPositionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPosition;
using BookingPositionGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup;
using BRSConfigurationTemplateEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS.BRSConfigurationTemplate;
using ChannelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Channel;
using ClashExceptionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions.ClashException;
using ClearanceCodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClearanceCode;
using DemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic;
using FacilityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Facility;
using FlexibilityLevelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FlexibilityLevel;
using IndexTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.IndexType;
using ISRSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings.ISRSettings;
using LibrarySalesAreaPassPriorityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.LibrarySalesAreaPassPriority;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using ProgrammeDictionaryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeDictionary;
using RecommendationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Recommendation;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;
using RuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Rule;
using RuleTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RuleType;
using SponsorshipEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.Sponsorship;
using SpotEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Spot;
using SpotPlacementEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotPlacement;

namespace xggameplan.utils.seeddata.Migration
{
    public class MigrationModule : Module
    {
        private readonly MigrationOptions _options;

        public MigrationModule(MigrationOptions options)
        {
            _options = options;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRavenDependencies(builder);
            RegisterSqlServerDependencies(builder);

            switch (_options.DatabaseType)
            {
                case DatabaseType.Tenant:
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<AWSInstanceConfiguration>>()
                        .As<IMigrationDocumentHandler<AWSInstanceConfiguration>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<AutoBook>>()
                        .As<IMigrationDocumentHandler<AutoBook>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<AutoBookInstanceConfiguration>>()
                        .As<IMigrationDocumentHandler<AutoBookInstanceConfiguration>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<AutoBookSettings, AutoBookSettingsEntity>>()
                        .As<IMigrationDocumentHandler<AutoBookSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<BookingPosition, BookingPositionEntity>>()
                        .As<IMigrationDocumentHandler<BookingPosition>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<BookingPositionGroup, BookingPositionGroupEntity>>()
                        .As<IMigrationDocumentHandler<BookingPositionGroup>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Break>>()
                        .As<IMigrationDocumentHandler<Break>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Campaign>>()
                        .As<IMigrationDocumentHandler<Campaign>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<ClearanceCode, ClearanceCodeEntity>>()
                        .As<IMigrationDocumentHandler<ClearanceCode>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Channel, ChannelEntity>>()
                        .As<IMigrationDocumentHandler<Channel>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Clash>>()
                        .As<IMigrationDocumentHandler<Clash>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<ClashException, ClashExceptionEntity>>()
                        .As<IMigrationDocumentHandler<ClashException>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Demographic, DemographicEntity>>()
                        .As<IMigrationDocumentHandler<Demographic>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<EfficiencySettings>>()
                        .As<IMigrationDocumentHandler<EfficiencySettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<EmailAuditEventSettings>>()
                        .As<IMigrationDocumentHandler<EmailAuditEventSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Facility, FacilityEntity>>()
                        .As<IMigrationDocumentHandler<Facility>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<IndexType, IndexTypeEntity>>()
                        .As<IMigrationDocumentHandler<IndexType>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<ISRSettings, ISRSettingsEntity>>()
                        .As<IMigrationDocumentHandler<ISRSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<RSSettings, RSSettingsEntity>>()
                        .As<IMigrationDocumentHandler<RSSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<KPIComparisonConfig>>()
                        .As<IMigrationDocumentHandler<KPIComparisonConfig>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Language>>()
                        .As<IMigrationDocumentHandler<Language>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder
                        .RegisterType<RavenToSqlIdentityMigrationDocumentHandler<LibrarySalesAreaPassPriority,
                            LibrarySalesAreaPassPriorityEntity>>()
                        .As<IMigrationDocumentHandler<LibrarySalesAreaPassPriority>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Metadata>>()
                        .As<IMigrationDocumentHandler<Metadata>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<MSTeamsAuditEventSettings>>()
                        .As<IMigrationDocumentHandler<MSTeamsAuditEventSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<OutputFile>>()
                        .As<IMigrationDocumentHandler<OutputFile>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<PassMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<Pass>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Product>>()
                        .As<IMigrationDocumentHandler<Product>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<ProgrammeClassification>>()
                        .As<IMigrationDocumentHandler<ProgrammeClassification>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder
                        .RegisterType<RavenToSqlIdentityMigrationDocumentHandler<ProgrammeDictionary,
                            ProgrammeDictionaryEntity>>()
                        .As<IMigrationDocumentHandler<ProgrammeDictionary>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Programme>>()
                        .As<IMigrationDocumentHandler<Programme>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RatingsPredictionScheduleMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<RatingsPredictionSchedule>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Recommendation, RecommendationEntity>>()
                        .As<IMigrationDocumentHandler<Recommendation>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Restriction, RestrictionEntity>>()
                        .As<IMigrationDocumentHandler<Restriction>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<ResultFileMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<ResultFileMigrationDocumentHandler.ResultFile>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RunMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<Run>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder
                        .RegisterType<RavenToSqlMigrationDocumentHandler<SalesArea>>()
                        .As<IMigrationDocumentHandler<SalesArea>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder
                        .RegisterType<ScheduleMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<Schedule>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder
                        .RegisterType<RavenToSqlIdentityMigrationDocumentHandler<SpotPlacement, SpotPlacementEntity>>()
                        .As<IMigrationDocumentHandler<SpotPlacement>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<ScenarioMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<Scenario>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder
                        .RegisterType<RavenToSqlMigrationDocumentHandler<TenantSettings>>()
                        .As<IMigrationDocumentHandler<TenantSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Universe>>()
                        .As<IMigrationDocumentHandler<Universe>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<FunctionalArea>>()
                        .As<IMigrationDocumentHandler<FunctionalArea>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<ScenarioResult>>()
                        .As<IMigrationDocumentHandler<ScenarioResult>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<SmoothFailure>>()
                        .As<IMigrationDocumentHandler<SmoothFailure>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<SmoothFailureMessage>>()
                        .As<IMigrationDocumentHandler<SmoothFailureMessage>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<SmoothConfiguration>>()
                        .As<IMigrationDocumentHandler<SmoothConfiguration>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Sponsorship, SponsorshipEntity>>()
                        .As<IMigrationDocumentHandler<Sponsorship>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Spot, SpotEntity>>()
                        .As<IMigrationDocumentHandler<Spot>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<Failures>>()
                        .As<IMigrationDocumentHandler<Failures>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<AutopilotSettings, AutopilotSettingsEntity>>()
                        .As<IMigrationDocumentHandler<AutopilotSettings>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<AutopilotRule, AutopilotRuleEntity>>()
                       .As<IMigrationDocumentHandler<AutopilotRule>>()
                       .As<IMigrationDocumentHandler>()
                       .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<Rule, RuleEntity>>()
                       .As<IMigrationDocumentHandler<Rule>>()
                       .As<IMigrationDocumentHandler>()
                       .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<RuleType, RuleTypeEntity>>()
                       .As<IMigrationDocumentHandler<RuleType>>()
                       .As<IMigrationDocumentHandler>()
                       .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<FlexibilityLevel, FlexibilityLevelEntity>>()
                        .As<IMigrationDocumentHandler<FlexibilityLevel>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<AutoBookDefaultParameters>>()
                        .As<IMigrationDocumentHandler<AutoBookDefaultParameters>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<ScenarioCampaignResultMigrationDocumentHandler>()
                        .As<IMigrationDocumentHandler<ScenarioCampaignResult>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<KPIPriority>>()
                       .As<IMigrationDocumentHandler<KPIPriority>>()
                       .As<IMigrationDocumentHandler>()
                       .InstancePerLifetimeScope();
                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<BRSConfigurationTemplate, BRSConfigurationTemplateEntity>>()
                      .As<IMigrationDocumentHandler<BRSConfigurationTemplate>>()
                      .As<IMigrationDocumentHandler>()
                      .InstancePerLifetimeScope();
                    break;

                case DatabaseType.Master:

                    _ = builder.RegisterType<PreviewableEntityMigrationDocumentHandler<User, MasterEntities.User, IUsersRepository>>()
                    .As<IMigrationDocumentHandler<User>>()
                    .As<IMigrationDocumentHandler>()
                    .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<AccessToken>>()
                    .As<IMigrationDocumentHandler<AccessToken>>()
                    .As<IMigrationDocumentHandler>()
                    .InstancePerLifetimeScope();

                    _ = builder.RegisterType<TenantMigrationDocumentHandler>()
                    .As<IMigrationDocumentHandler<Tenant>>()
                    .As<IMigrationDocumentHandler>()
                    .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<TenantProductFeature, MasterEntities.TenantProductFeatures.TenantProductFeature>>()
                        .As<IMigrationDocumentHandler<TenantProductFeature>>()
                        .As<IMigrationDocumentHandler>()
                        .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlIdentityMigrationDocumentHandler<ProductSettings, MasterEntities.ProductSettings>>()
                    .As<IMigrationDocumentHandler<ProductSettings>>()
                    .As<IMigrationDocumentHandler>()
                    .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<TaskInstance>>()
                    .As<IMigrationDocumentHandler<TaskInstance>>()
                    .As<IMigrationDocumentHandler>()
                    .InstancePerLifetimeScope();

                    _ = builder.RegisterType<RavenToSqlMigrationDocumentHandler<UpdateDetails>>()
                    .As<IMigrationDocumentHandler<UpdateDetails>>()
                    .As<IMigrationDocumentHandler>()
                    .InstancePerLifetimeScope();
                    break;
            }
        }

        protected void RegisterRavenDependencies(ContainerBuilder builder)
        {
            var ravenBuilder = new ContainerBuilder();
            _ = ravenBuilder.RegisterInstance(SystemClock.Instance).As<IClock>();
            _ = ravenBuilder.RegisterType<InMemoryCache>().As<ICache>().InstancePerLifetimeScope();
            _ = ravenBuilder.AddAutoMapper(typeof(AutofacBuilderExtensions).Assembly);
            _ = ravenBuilder.RegisterModule(new RavenModule(_options.DatabaseType, _options.ConnectionStringFrom));
            var ravenContainer = ravenBuilder.Build();

            _ = builder.Register(context => ravenContainer.BeginLifetimeScope(cb => { }))
                .Keyed<ILifetimeScope>(MigrationSource.From)
                .InstancePerDependency();
        }

        protected void RegisterSqlServerDependencies(ContainerBuilder builder)
        {
            var sqlServerBuilder = new ContainerBuilder();
            _ = sqlServerBuilder.RegisterInstance(SystemClock.Instance).As<IClock>();
            _ = sqlServerBuilder.RegisterType<InMemoryCache>().As<ICache>().InstancePerLifetimeScope();
            _ = sqlServerBuilder.AddAutoMapper(
                typeof(AutofacBuilderExtensions).Assembly,
                typeof(AccessTokenProfile).Assembly);
            _ = sqlServerBuilder.RegisterModule(
                new SqlServerModule<SqlServerMigrationMasterDbContext, SqlServerMigrationTenantDbContext>(
                    _options.DatabaseType, _options.ConnectionStringTo,
                    new SqlServerDbContextRegistrationFeatures { Audit = false, CommandTimeout = 600 }));
            var sqlServerContainer = sqlServerBuilder.Build();

            _ = builder.Register(context => sqlServerContainer.BeginLifetimeScope())
                .Keyed<ILifetimeScope>(MigrationSource.To)
                .InstancePerDependency();

            _ = builder.Register(context =>
                  {
                      var scope = context.ResolveKeyed<ILifetimeScope>(MigrationSource.To);
                      return new MigrationHistoryInitializer(scope.Resolve<ISqlServerDbContext>());
                  })
                .As<IMigrationPrepareAction>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterGeneric(typeof(SequenceRebuilder<,>))
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
