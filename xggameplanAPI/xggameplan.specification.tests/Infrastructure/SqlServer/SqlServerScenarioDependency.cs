﻿using System;
using System.Globalization;
using BoDi;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.common.Caching;
using xggameplan.core.Repository;
using xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers;
using xggameplan.specification.tests.Infrastructure.SqlServer.Fts;
using xggameplan.specification.tests.Infrastructure.SqlServer.Interception;
using xggameplan.specification.tests.Interfaces;
using AnalysisGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AnalysisGroups.AnalysisGroup;
using AutoBookEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBook;
using AutoBookInstanceConfigurationCriteriaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookInstanceConfigurationCriteria;
using AutoBookInstanceConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookInstanceConfiguration;
using AutoBookSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookSettings;
using AutopilotRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotRule;
using AutopilotSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutopilotSettings;
using AWSInstanceConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AWSInstanceConfiguration;
using BookingPositionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPosition;
using BookingPositionGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup;
using BRSConfigurationTemplateEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS.BRSConfigurationTemplate;
using BusinessTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BusinessTypes.BusinessType;
using CampaignSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.CampaignSettings;
using ChannelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Channel;
using ClashExceptionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions.ClashException;
using ClearanceCodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClearanceCode;
using DeliveryCappingGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DeliveryCappingGroup;
using DemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic;
using EfficiencySettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.EfficiencySettings;
using EmailAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.EmailAuditEventSettings;
using FacilityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Facility;
using FlexibilityLevelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FlexibilityLevel;
using FunctionalAreaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas.FunctionalArea;
using InventoryTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses.InventoryType;
using ISRGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRGlobalSettings;
using KPIComparisonConfigEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.KPIComparisonConfig;
using LanguageEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Language;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using MSTeamsAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsAuditEventSettings;
using OutputFileEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.OutputFiles.OutputFile;
using ProgrammeCategoryHierarchyEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeCategoryHierarchy;
using ProgrammeClassificationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeClassification;
using ProgrammeDictionaryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeDictionary;
using RecommendationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Recommendation;
using RSGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSGlobalSettings;
using RuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Rule;
using RuleTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RuleType;
using RunEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.Run;
using RunTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RunType;
using SalesAreaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesArea;
using ScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.Scenario;
using ScenarioResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioResult;
using SmoothConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothConfiguration;
using SmoothFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailure;
using SmoothFailureMessageEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.SmoothFailureMessage;
using SpotBookingRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules.SpotBookingRule;
using SpotPlacementEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotPlacement;
using TenantSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.TenantSettings;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerScenarioDependency : IScenarioDependency
    {
        public virtual void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterInstanceAs(
                new DbContextOptionsBuilder<SqlServerTestDbContext>()
                    .UseInternalServiceProvider(new Func<IServiceProvider>(() => new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .AddScoped<IQueryModelGenerator, TestQueryModelGenerator>()
                        .AddSingleton(objectContainer.Resolve<IFtsInterceptionProvider>())
                        .BuildServiceProvider())())
                    .UseInMemoryDatabase(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture))
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .UseBulkInsertEngine(dbContext => new SqlServerTestBulkInsertEngine(dbContext))
                    .Options);
            objectContainer.RegisterFactoryAs<DbContextOptions>(oc =>
                oc.Resolve<DbContextOptions<SqlServerTestDbContext>>());

            objectContainer.RegisterTypeAs<SqlServerTestDbContext, ISqlServerTestDbContext>();
            objectContainer.RegisterFactoryAs<ISqlServerDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<IMasterDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ITenantDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ISqlServerMasterDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ISqlServerTenantDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ISqlServerLongRunningTenantDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());

            objectContainer
                .RegisterTypeAs<SqlServerTestDbContextFactory<ISqlServerTenantDbContext, SqlServerTestDbContext>,
                    ISqlServerDbContextFactory<ISqlServerTenantDbContext>>();

            objectContainer.RegisterTypeAs<SalesAreaCacheAccessor, SalesAreaCacheAccessor>();
            objectContainer.RegisterFactoryAs<ISqlServerSalesAreaByIdCacheAccessor>(oc => oc.Resolve<SalesAreaCacheAccessor>());
            objectContainer.RegisterFactoryAs<ISqlServerSalesAreaByNullableIdCacheAccessor>(oc => oc.Resolve<SalesAreaCacheAccessor>());
            objectContainer.RegisterFactoryAs<ISqlServerSalesAreaByNameCacheAccessor>(oc => oc.Resolve<SalesAreaCacheAccessor>());

            objectContainer.RegisterTypeAs<InMemoryCache, ICache>();
            objectContainer.RegisterTypeAs<AccessTokenRepository, IAccessTokensRepository>();
            objectContainer.RegisterTypeAs<TenantsRepository, ITenantsRepository>();
            objectContainer.RegisterTypeAs<UserRepository, IUsersRepository>();
            objectContainer.RegisterTypeAs<ProductSettingsRepository, IProductSettingsRepository>();
            objectContainer.RegisterTypeAs<UserSettingsRepository, IUserSettingsService>();
            objectContainer.RegisterTypeAs<TaskInstanceRepository, ITaskInstanceRepository>();
            objectContainer.RegisterTypeAs<UpdateDetailsRepository, IUpdateDetailsRepository>();

            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.AccessToken, AccessToken>, IDomainModelHandler<AccessToken>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.User, User>, IDomainModelHandler<User>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.Tenant, Tenant>, IDomainModelHandler<Tenant>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.ProductSettings, ProductSettings>, IDomainModelHandler<ProductSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.PreviewFile, PreviewFile>, IDomainModelHandler<PreviewFile>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.TaskInstance, TaskInstance>, IDomainModelHandler<TaskInstance>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MasterEntities.UpdateDetails, UpdateDetails>, IDomainModelHandler<UpdateDetails>>();

            objectContainer.RegisterTypeAs<AnalysisGroupRepository, IAnalysisGroupRepository>();
            objectContainer.RegisterTypeAs<AWSInstanceConfigurationRepository, IAWSInstanceConfigurationRepository>();
            objectContainer.RegisterTypeAs<AutoBookDefaultParametersRepository, IAutoBookDefaultParametersRepository>();
            objectContainer.RegisterTypeAs<AutoBookInstanceConfigurationRepository, IAutoBookInstanceConfigurationRepository>();
            objectContainer.RegisterTypeAs<AutoBookSettingsRepository, IAutoBookSettingsRepository>();
            objectContainer.RegisterTypeAs<AutoBookRepository, IAutoBookRepository>();
            objectContainer.RegisterTypeAs<BookingPositionRepository, IBookingPositionRepository>();
            objectContainer.RegisterTypeAs<BookingPositionGroupRepository, IBookingPositionGroupRepository>();
            objectContainer.RegisterTypeAs<BreakRepository, IBreakRepository>();
            objectContainer.RegisterTypeAs<BusinessTypeRepository, IBusinessTypeRepository>();
            objectContainer.RegisterTypeAs<BRSConfigurationTemplateRepository, IBRSConfigurationTemplateRepository>();
            objectContainer.RegisterTypeAs<CampaignRepository, ICampaignRepository>();
            objectContainer.RegisterTypeAs<CampaignSettingsRepository, ICampaignSettingsRepository>();
            objectContainer.RegisterTypeAs<ChannelRepository, IChannelsRepository>();
            objectContainer.RegisterTypeAs<ClashExceptionRepository, IClashExceptionRepository>();
            objectContainer.RegisterTypeAs<ClashRepository, IClashRepository>();
            objectContainer.RegisterTypeAs<ClearanceRepository, IClearanceRepository>();
            objectContainer.RegisterTypeAs<DeliveryCappingGroupRepository, IDeliveryCappingGroupRepository>();
            objectContainer.RegisterTypeAs<DemographicRepository, IDemographicRepository>();
            objectContainer.RegisterTypeAs<EfficiencySettingsRepository, IEfficiencySettingsRepository>();
            objectContainer.RegisterTypeAs<FacilityRepository, IFacilityRepository>();
            objectContainer.RegisterTypeAs<EmailAuditEventSettingsRepository, IEmailAuditEventSettingsRepository>();
            objectContainer.RegisterTypeAs<FunctionalAreaRepository, IFunctionalAreaRepository>();
            objectContainer.RegisterTypeAs<FailuresRepository, IFailuresRepository>();
            objectContainer.RegisterTypeAs<IndexTypeRepository, IIndexTypeRepository>();
            objectContainer.RegisterTypeAs<ISRSettingsRepository, IISRSettingsRepository>();
            objectContainer.RegisterTypeAs<KPIComparisonConfigRepository, IKPIComparisonConfigRepository>();
            objectContainer.RegisterTypeAs<LanguageRepository, ILanguageRepository>();
            objectContainer.RegisterTypeAs<LibrarySalesAreaPassPrioritiesRepository, ILibrarySalesAreaPassPrioritiesRepository>();
            objectContainer.RegisterTypeAs<LengthFactorRepository, ILengthFactorRepository>();
            objectContainer.RegisterTypeAs<MetadataRepository, IMetadataRepository>();
            objectContainer.RegisterTypeAs<MSTeamsAuditEventSettingsRepository, IMSTeamsAuditEventSettingsRepository>();
            objectContainer.RegisterTypeAs<OutputFileRepository, IOutputFileRepository>();
            objectContainer.RegisterTypeAs<PassRepository, IPassRepository>();
            objectContainer.RegisterTypeAs<ProductRepository, IProductRepository>();
            objectContainer.RegisterTypeAs<ProgrammeClassificationRepository, IProgrammeClassificationRepository>();
            objectContainer.RegisterTypeAs<ProgrammeDictionaryRepository, IProgrammeDictionaryRepository>();
            objectContainer.RegisterTypeAs<ProgrammeRepository, IProgrammeRepository>();
            objectContainer.RegisterTypeAs<RatingsScheduleRepository, IRatingsScheduleRepository>();
            objectContainer.RegisterTypeAs<RecommendationRepository, IRecommendationRepository>();
            objectContainer.RegisterTypeAs<RestrictionRepository, IRestrictionRepository>();
            objectContainer.RegisterTypeAs<ResultsFileRepository, IResultsFileRepository>();
            objectContainer.RegisterTypeAs<RSSettingsRepository, IRSSettingsRepository>();
            objectContainer.RegisterTypeAs<RunRepository, IRunRepository>();
            objectContainer.RegisterTypeAs<SalesAreaRepository, ISalesAreaRepository>();
            objectContainer.RegisterTypeAs<SmoothConfigurationRepository, ISmoothConfigurationRepository>();
            objectContainer.RegisterTypeAs<ScenarioResultRepository, IScenarioResultRepository>();
            objectContainer.RegisterTypeAs<ScenarioCampaignResultRepository, IScenarioCampaignResultRepository>();
            objectContainer.RegisterTypeAs<ScenarioCampaignFailureRepository, IScenarioCampaignFailureRepository>();
            objectContainer.RegisterTypeAs<ScenarioCampaignMetricRepository, IScenarioCampaignMetricRepository>();
            objectContainer.RegisterTypeAs<ScheduleRepository, IScheduleRepository>();
            objectContainer.RegisterTypeAs<SmoothFailureMessageRepository, ISmoothFailureMessageRepository>();
            objectContainer.RegisterTypeAs<SmoothFailureRepository, ISmoothFailureRepository>();
            objectContainer.RegisterTypeAs<SponsorshipRepository, ISponsorshipRepository>();
            objectContainer.RegisterTypeAs<SpotRepository, ISpotRepository>();
            objectContainer.RegisterTypeAs<SpotPlacementRepository, ISpotPlacementRepository>();
            objectContainer.RegisterTypeAs<TenantSettingsRepository, ITenantSettingsRepository>();
            objectContainer.RegisterTypeAs<ScenarioRepository, IScenarioRepository>();
            objectContainer.RegisterTypeAs<UniverseRepository, IUniverseRepository>();
            objectContainer.RegisterTypeAs<ISRGlobalSettingsRepository, IISRGlobalSettingsRepository>();
            objectContainer.RegisterTypeAs<RSGlobalSettingsRepository, IRSGlobalSettingsRepository>();
            objectContainer.RegisterTypeAs<AutopilotSettingsRepository, IAutopilotSettingsRepository>();
            objectContainer.RegisterTypeAs<AutopilotRuleRepository, IAutopilotRuleRepository>();
            objectContainer.RegisterTypeAs<RuleRepository, IRuleRepository>();
            objectContainer.RegisterTypeAs<RuleTypeRepository, IRuleTypeRepository>();
            objectContainer.RegisterTypeAs<FlexibilityLevelRepository, IFlexibilityLevelRepository>();
            objectContainer.RegisterTypeAs<LockTypeRepository, ILockTypeRepository>();
            objectContainer.RegisterTypeAs<InventoryTypeRepository, IInventoryTypeRepository>();
            objectContainer.RegisterTypeAs<InventoryLockRepository, IInventoryLockRepository>();
            objectContainer.RegisterTypeAs<MetadataCategoryDomainModelHandler, IDomainModelHandler<Metadata>>();
            objectContainer.RegisterTypeAs<TotalRatingRepository, ITotalRatingRepository>();
            objectContainer.RegisterTypeAs<UniverseRepository, IUniverseRepository>();
            objectContainer.RegisterTypeAs<ProgrammeEpisodeRepository, IProgrammeEpisodeRepository>();
            objectContainer.RegisterTypeAs<SalesAreaDemographicRepository, ISalesAreaDemographicRepository>();
            objectContainer.RegisterTypeAs<StandardDayPartRepository, IStandardDayPartRepository>();
            objectContainer.RegisterTypeAs<StandardDayPartGroupRepository, IStandardDayPartGroupRepository>();
            objectContainer.RegisterTypeAs<ProgrammeCategoryHierarchyRepository, IProgrammeCategoryHierarchyRepository>();
            objectContainer.RegisterTypeAs<PipelineAuditEventRepository, IPipelineAuditEventRepository>();
            objectContainer.RegisterTypeAs<RunTypeRepository, IRunTypeRepository>();
            objectContainer.RegisterTypeAs<SpotBookingRuleRepository, ISpotBookingRuleRepository>();
            objectContainer.RegisterTypeAs<LandmarkRunQueueRepository, ILandmarkRunQueueRepository>();

            objectContainer.RegisterTypeAs<AutoBookDefaultParametersDomainModelHandler, IDomainModelHandler<AutoBookDefaultParameters>>();
            objectContainer.RegisterTypeAs<MetadataCategoryDomainModelHandler, IDomainModelHandler<Metadata>>();
            objectContainer.RegisterTypeAs<ScheduleDomainModelHandler, IDomainModelHandler<Schedule>>();
            objectContainer.RegisterTypeAs<FunctionalAreaDomainModelHandler, IDomainModelHandler<FunctionalArea>>();
            objectContainer.RegisterTypeAs<ScenarioCampaignResultDomainModelHandler, IDomainModelHandler<ScenarioCampaignResult>>();
            objectContainer.RegisterTypeAs<BusinessTypeDomainModelHandler, IDomainModelHandler<BusinessType>>();
            objectContainer.RegisterTypeAs<ClashDomainModelHandler, IDomainModelHandler<Clash>>();
            objectContainer.RegisterTypeAs<IndexTypeDomainModelHandler, IDomainModelHandler<IndexType>>();
            objectContainer.RegisterTypeAs<InventoryLockDomainModelHandler, IDomainModelHandler<InventoryLock>>();
            objectContainer.RegisterTypeAs<RestrictionDomainModelHandler, IDomainModelHandler<Restriction>>();
            objectContainer.RegisterTypeAs<ScenarioCampaignFailureDomainModelHandler, IDomainModelHandler<ScenarioCampaignFailure>>();
            objectContainer.RegisterTypeAs<ScenarioCampaignMetricDomainModelHandler, IDomainModelHandler<ScenarioCampaignMetric>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AnalysisGroupEntity, AnalysisGroup>, IDomainModelHandler<AnalysisGroup>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AWSInstanceConfigurationEntity, AWSInstanceConfiguration>, IDomainModelHandler<AWSInstanceConfiguration>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AutoBookInstanceConfigurationEntity, AutoBookInstanceConfiguration>, IDomainModelHandler<AutoBookInstanceConfiguration>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AutoBookInstanceConfigurationCriteriaEntity, AutoBookInstanceConfigurationCriteria>, IDomainModelHandler<AutoBookInstanceConfigurationCriteria>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AutoBookSettingsEntity, AutoBookSettings>, IDomainModelHandler<AutoBookSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AutoBookEntity, AutoBook>, IDomainModelHandler<AutoBook>>();
            objectContainer.RegisterTypeAs<BreakDomainModelHandler, IDomainModelHandler<Break>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<BusinessTypeEntity, BusinessType>, IDomainModelHandler<BusinessType>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<BRSConfigurationTemplateEntity, BRSConfigurationTemplate>, IDomainModelHandler<BRSConfigurationTemplate>>();
            objectContainer.RegisterTypeAs<CampaignDomainModelHandler, IDomainModelHandler<Campaign>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<CampaignSettingsEntity, CampaignSettings>, IDomainModelHandler<CampaignSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ChannelEntity, Channel>, IDomainModelHandler<Channel>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ClashExceptionEntity, ClashException>, IDomainModelHandler<ClashException>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ClearanceCodeEntity, ClearanceCode>, IDomainModelHandler<ClearanceCode>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<DemographicEntity, Demographic>, IDomainModelHandler<Demographic>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<EfficiencySettingsEntity, EfficiencySettings>, IDomainModelHandler<EfficiencySettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<FunctionalAreaEntity, FunctionalArea>, IDomainModelHandler<FunctionalArea>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<KPIComparisonConfigEntity, KPIComparisonConfig>, IDomainModelHandler<KPIComparisonConfig>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<LanguageEntity, Language>, IDomainModelHandler<Language>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<BookingPositionEntity, BookingPosition>, IDomainModelHandler<BookingPosition>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<BookingPositionGroupEntity, BookingPositionGroup>, IDomainModelHandler<BookingPositionGroup>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<DeliveryCappingGroupEntity, DeliveryCappingGroup>, IDomainModelHandler<DeliveryCappingGroup>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<EmailAuditEventSettingsEntity, EmailAuditEventSettings>, IDomainModelHandler<EmailAuditEventSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures.Failure, Failures>, IDomainModelHandler<Failures>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<FacilityEntity, Facility>, IDomainModelHandler<Facility>>();
            objectContainer.RegisterTypeAs<LibrarySalesAreaPassPriorityDomainModelHandler, IDomainModelHandler<LibrarySalesAreaPassPriority>>();
            objectContainer.RegisterTypeAs<LengthFactorDomainModelHandler, IDomainModelHandler<LengthFactor>>();
            objectContainer.RegisterTypeAs<ISRSettingsDomainModelHandler, IDomainModelHandler<ISRSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<MSTeamsAuditEventSettingsEntity, MSTeamsAuditEventSettings>, IDomainModelHandler<MSTeamsAuditEventSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<OutputFileEntity, OutputFile>, IDomainModelHandler<OutputFile>>();
            objectContainer.RegisterTypeAs<PassDomainModelHandler, IDomainModelHandler<Pass>>();
            objectContainer.RegisterTypeAs<ProductDomainModelHandler, IDomainModelHandler<Product>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeClassificationEntity, ProgrammeClassification>, IDomainModelHandler<ProgrammeClassification>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeDictionaryEntity, ProgrammeDictionary>, IDomainModelHandler<ProgrammeDictionary>>();
            objectContainer.RegisterTypeAs<ProgrammeDomainModelHandler, IDomainModelHandler<Programme>>();
            objectContainer.RegisterTypeAs<RatingsPredictionScheduleDomainModelHandler, IDomainModelHandler<RatingsPredictionSchedule>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RecommendationEntity, Recommendation>, IDomainModelHandler<Recommendation>>();
            objectContainer.RegisterTypeAs<RSSettingsDomainModelHandler, IDomainModelHandler<RSSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RunEntity, Run>, IDomainModelHandler<Run>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses.InventoryLockType, InventoryLockType>, IDomainModelHandler<InventoryLockType>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<InventoryTypeEntity, InventoryType>, IDomainModelHandler<InventoryType>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SalesAreaEntity, SalesArea>, IDomainModelHandler<SalesArea>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SmoothConfigurationEntity, SmoothConfiguration>, IDomainModelHandler<SmoothConfiguration>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ScenarioResultEntity, ScenarioResult>, IDomainModelHandler<ScenarioResult>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SmoothFailureMessageEntity, SmoothFailureMessage>, IDomainModelHandler<SmoothFailureMessage>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SmoothFailureEntity, SmoothFailure>, IDomainModelHandler<SmoothFailure>>();
            objectContainer.RegisterTypeAs<SpotDomainModelHandler, IDomainModelHandler<Spot>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SpotPlacementEntity, SpotPlacement>, IDomainModelHandler<SpotPlacement>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ScenarioEntity, Scenario>, IDomainModelHandler<Scenario>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<TenantSettingsEntity, TenantSettings>, IDomainModelHandler<TenantSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ISRGlobalSettingsEntity, ISRGlobalSettings>, IDomainModelHandler<ISRGlobalSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RSGlobalSettingsEntity, RSGlobalSettings>, IDomainModelHandler<RSGlobalSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AutopilotSettingsEntity, AutopilotSettings>, IDomainModelHandler<AutopilotSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<AutopilotRuleEntity, AutopilotRule>, IDomainModelHandler<AutopilotRule>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RuleEntity, Rule>, IDomainModelHandler<Rule>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RuleTypeEntity, RuleType>, IDomainModelHandler<RuleType>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<FlexibilityLevelEntity, FlexibilityLevel>, IDomainModelHandler<FlexibilityLevel>>();
            objectContainer.RegisterTypeAs<UniverseDomainModelHandler, IDomainModelHandler<Universe>>();
            objectContainer.RegisterTypeAs<StandardDayPartDomainModelHandler, IDomainModelHandler<StandardDayPart>>();
            objectContainer.RegisterTypeAs<StandardDayPartGroupDomainModelHandler, IDomainModelHandler<StandardDayPartGroup>>();
            objectContainer.RegisterTypeAs<TotalRatingDomainModelHandler, IDomainModelHandler<TotalRating>>();
            objectContainer.RegisterTypeAs<SalesAreaDemographicDomainModelHandler, IDomainModelHandler<SalesAreaDemographic>>();
            objectContainer.RegisterTypeAs<ProgrammeEpisodeDomainModelHandler, IDomainModelHandler<ProgrammeEpisode>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeCategoryHierarchyEntity, ProgrammeCategoryHierarchy>, IDomainModelHandler<ProgrammeCategoryHierarchy>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RunTypeEntity, RunType>, IDomainModelHandler<RunType>>();
            objectContainer.RegisterTypeAs<SpotBookingRuleDomainModelHandler, IDomainModelHandler<SpotBookingRule>>();

            objectContainer.RegisterTypeAs<SqlServerScenarioDbContext, IScenarioDbContext>();
            objectContainer.RegisterTypeAs<SqlServerJsonTestDataImporter, ITestDataImporter>();
            objectContainer.RegisterTypeAs<SqlServerTestDomainModelHandlerResolver, IDomainModelHandlerResolver>();
            objectContainer.RegisterTypeAs<RegexSearchConditionBuilder, IFullTextSearchConditionBuilder>();
            objectContainer.RegisterTypeAs<ResultsFileTenantDbStorage, IResultsFileStorage>();

            objectContainer.RegisterTypeAs<SqlServerTestIdentityGenerator, IIdentityGenerator>();

            objectContainer.RegisterTypeAs<TestClock, ITestClock>();
            objectContainer.RegisterFactoryAs<IClock>(oc => oc.Resolve<ITestClock>());
        }
    }
}
