using System;
using System.Globalization;
using BoDi;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Fts;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.CampaignCleaning;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using xggameplan.core.Interfaces;
using BookingPositionGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.BookingPositionGroup;
using CampaignEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign;
using CampaignSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.CampaignSettings;
using ClashEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Clash;
using ClashExceptionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions.ClashException;
using ClearanceCodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClearanceCode;
using DemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic;
using InventoryTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses.InventoryType;
using LengthFactorEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.LengthFactor;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;
using ProgrammeCategoryHierarchyEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeCategoryHierarchy;
using ProgrammeClassificationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeClassification;
using ProgrammeDictionaryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeDictionary;
using ProgrammeEpisodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.ProgrammeEpisode;
using RecommendationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Recommendation;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;
using SalesAreaDemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesAreaDemographic;
using SalesAreaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesArea;
using ScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.Scenario;
using ScenarioResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioResult;
using SpotBookingRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules.SpotBookingRule;
using SpotEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Spot;
using StandardDayPartEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPart;
using StandardDayPartGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPartGroup;
using TenantSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.TenantSettings;
using TotalRatingEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TotalRating;
using UniverseEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Universe;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
{
    public class SqlServerScenarioDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
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

            objectContainer.RegisterTypeAs<
                    SqlServerTestDbContextFactory<ISqlServerTenantDbContext, SqlServerTestDbContext>,
                    ISqlServerDbContextFactory<ISqlServerTenantDbContext>>();

            objectContainer.RegisterTypeAs<SqlServerTestDbContext, ISqlServerTestDbContext>();
            objectContainer.RegisterFactoryAs<ISqlServerDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<IMasterDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ITenantDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ISqlServerMasterDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ISqlServerTenantDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());
            objectContainer.RegisterFactoryAs<ISqlServerLongRunningTenantDbContext>(oc => oc.Resolve<ISqlServerTestDbContext>());

            objectContainer.RegisterTypeAs<SalesAreaCacheAccessor, SalesAreaCacheAccessor>();
            objectContainer.RegisterFactoryAs<ISqlServerSalesAreaByIdCacheAccessor>(oc => oc.Resolve<SalesAreaCacheAccessor>());
            objectContainer.RegisterFactoryAs<ISqlServerSalesAreaByNullableIdCacheAccessor>(oc => oc.Resolve<SalesAreaCacheAccessor>());
            objectContainer.RegisterFactoryAs<ISqlServerSalesAreaByNameCacheAccessor>(oc => oc.Resolve<SalesAreaCacheAccessor>());

            objectContainer.RegisterTypeAs<LockTypeRepository, ILockTypeRepository>();
            objectContainer.RegisterTypeAs<InventoryTypeRepository, IInventoryTypeRepository>();
            objectContainer.RegisterTypeAs<InventoryLockRepository, IInventoryLockRepository>();
            objectContainer.RegisterTypeAs<TotalRatingRepository, ITotalRatingRepository>();
            objectContainer.RegisterTypeAs<UniverseRepository, IUniverseRepository>();
            objectContainer.RegisterTypeAs<SalesAreaRepository, ISalesAreaRepository>();
            objectContainer.RegisterTypeAs<DemographicRepository, IDemographicRepository>();
            objectContainer.RegisterTypeAs<MetadataRepository, IMetadataRepository>();
            objectContainer.RegisterTypeAs<ProgrammeRepository, IProgrammeRepository>();
            objectContainer.RegisterTypeAs<ProductRepository, IProductRepository>();
            objectContainer.RegisterTypeAs<ProgrammeClassificationRepository, IProgrammeClassificationRepository>();
            objectContainer.RegisterTypeAs<ClashRepository, IClashRepository>();
            objectContainer.RegisterTypeAs<ProductRepository, IProductRepository>();
            objectContainer.RegisterTypeAs<RestrictionRepository, IRestrictionRepository>();
            objectContainer.RegisterTypeAs<ClearanceRepository, IClearanceRepository>();
            objectContainer.RegisterTypeAs<SpotRepository, ISpotRepository>();
            objectContainer.RegisterTypeAs<BreakRepository, IBreakRepository>();
            objectContainer.RegisterTypeAs<BusinessTypeRepository, IBusinessTypeRepository>();
            objectContainer.RegisterTypeAs<ClashExceptionRepository, IClashExceptionRepository>();
            objectContainer.RegisterTypeAs<CampaignRepository, ICampaignRepository>();
            objectContainer.RegisterTypeAs<CampaignSettingsRepository, ICampaignSettingsRepository>();
            objectContainer.RegisterTypeAs<ISRSettingsRepository, IISRSettingsRepository>();
            objectContainer.RegisterTypeAs<SpotPlacementRepository, ISpotPlacementRepository>();
            objectContainer.RegisterTypeAs<AWSInstanceConfigurationRepository, IAWSInstanceConfigurationRepository>();
            objectContainer.RegisterTypeAs<BookingPositionRepository, IBookingPositionRepository>();
            objectContainer.RegisterTypeAs<TenantSettingsRepository, ITenantSettingsRepository>();
            objectContainer.RegisterTypeAs<BookingPositionGroupRepository, IBookingPositionGroupRepository>();
            objectContainer.RegisterTypeAs<ProgrammeCategoryHierarchyRepository, IProgrammeCategoryHierarchyRepository>();
            objectContainer.RegisterTypeAs<RatingsScheduleRepository, IRatingsScheduleRepository>();
            objectContainer.RegisterTypeAs<SalesAreaDemographicRepository, ISalesAreaDemographicRepository>();
            objectContainer.RegisterTypeAs<ScenarioRepository, IScenarioRepository>();
            objectContainer.RegisterTypeAs<PassRepository, IPassRepository>();
            objectContainer.RegisterTypeAs<ScheduleRepository, IScheduleRepository>();
            objectContainer.RegisterTypeAs<StandardDayPartRepository, IStandardDayPartRepository>();
            objectContainer.RegisterTypeAs<StandardDayPartGroupRepository, IStandardDayPartGroupRepository>();
            objectContainer.RegisterTypeAs<ProgrammeEpisodeRepository, IProgrammeEpisodeRepository>();
            objectContainer.RegisterTypeAs<SpotBookingRuleRepository, ISpotBookingRuleRepository>();
            objectContainer.RegisterTypeAs<LengthFactorRepository, ILengthFactorRepository>();

            objectContainer.RegisterTypeAs<SpotCleaner, ISpotCleaner>();

            objectContainer.RegisterTypeAs<InventoryLockDomainModelHandler, IDomainModelHandler<InventoryLock>>();
            objectContainer.RegisterTypeAs<MetadataCategoryDomainModelHandler, IDomainModelHandler<Metadata>>();
            objectContainer.RegisterTypeAs<BreakDomainModelHandler, IDomainModelHandler<Break>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<CampaignEntity, Campaign>, IDomainModelHandler<Campaign>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<CampaignSettingsEntity, CampaignSettings>, IDomainModelHandler<CampaignSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ClashEntity, Clash>, IDomainModelHandler<Clash>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ClashExceptionEntity, ClashException>, IDomainModelHandler<ClashException>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<DemographicEntity, Demographic>, IDomainModelHandler<Demographic>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeClassificationEntity, ProgrammeClassification>, IDomainModelHandler<ProgrammeClassification>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeDictionaryEntity, ProgrammeDictionary>, IDomainModelHandler<ProgrammeDictionary>>();
            objectContainer.RegisterTypeAs<ProgrammeDomainModelHandler, IDomainModelHandler<Programme>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<PredictionSchedule, RatingsPredictionSchedule>, IDomainModelHandler<RatingsPredictionSchedule>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProductEntity, Product>, IDomainModelHandler<Product>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RecommendationEntity, Recommendation>, IDomainModelHandler<Recommendation>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<RestrictionEntity, Restriction>, IDomainModelHandler<Restriction>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.InventoryStatuses.InventoryLockType, InventoryLockType>, IDomainModelHandler<InventoryLockType>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<InventoryTypeEntity, InventoryType>, IDomainModelHandler<InventoryType>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SalesAreaEntity, SalesArea>, IDomainModelHandler<SalesArea>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ScenarioResultEntity, ScenarioResult>, IDomainModelHandler<ScenarioResult>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SpotEntity, Spot>, IDomainModelHandler<Spot>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<TenantSettingsEntity, TenantSettings>, IDomainModelHandler<TenantSettings>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<BookingPositionGroupEntity, BookingPositionGroup>, IDomainModelHandler<BookingPositionGroup>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<UniverseEntity, Universe>, IDomainModelHandler<Universe>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<TotalRatingEntity, TotalRating>, IDomainModelHandler<TotalRating>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SalesAreaDemographicEntity, SalesAreaDemographic>, IDomainModelHandler<SalesAreaDemographic>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeCategoryHierarchyEntity, ProgrammeCategoryHierarchy>, IDomainModelHandler<ProgrammeCategoryHierarchy>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ClearanceCodeEntity, ClearanceCode>, IDomainModelHandler<ClearanceCode>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ScenarioEntity, Scenario>, IDomainModelHandler<Scenario>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<StandardDayPartEntity, StandardDayPart>, IDomainModelHandler<StandardDayPart>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<StandardDayPartGroupEntity, StandardDayPartGroup>, IDomainModelHandler<StandardDayPartGroup>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<SpotBookingRuleEntity, SpotBookingRule>, IDomainModelHandler<SpotBookingRule>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<ProgrammeEpisodeEntity, ProgrammeEpisode>, IDomainModelHandler<ProgrammeEpisode>>();
            objectContainer.RegisterTypeAs<SimpleDomainModelMappingHandler<LengthFactorEntity, LengthFactor>, IDomainModelHandler<LengthFactor>>();
            objectContainer.RegisterTypeAs<ScheduleDomainModelHandler, IDomainModelHandler<Schedule>>();

            objectContainer.RegisterTypeAs<SqlServerScenarioDbContext, IScenarioDbContext>();
            objectContainer.RegisterTypeAs<SqlServerJsonTestDataImporter, ITestDataImporter>();
            objectContainer.RegisterTypeAs<SqlServerTestDomainModelHandlerResolver, IDomainModelHandlerResolver>();
            objectContainer.RegisterTypeAs<RegexSearchConditionBuilder, IFullTextSearchConditionBuilder>();
            objectContainer.RegisterTypeAs<ResultsFileTenantDbStorage, IResultsFileStorage>();

            objectContainer.RegisterTypeAs<SqlServerIdentityGenerator, IIdentityGenerator>();
            objectContainer.RegisterTypeAs<SqlServerIdentityGenerator, ISqlServerIdentityGenerator>();
            objectContainer.RegisterTypeAs<SqlServerTenantIdentifierTestSequence, ISqlServerTenantIdentifierSequence>();
            objectContainer.RegisterTypeAs<SqlServerTenantIdentifierTestSequence, ISqlServerIdentifierSequence>();
            objectContainer.RegisterTypeAs<SqlServerTenantIdentifierTestSequence, ITenantIdentifierSequence>();
            objectContainer.RegisterTypeAs<SqlServerTenantIdentifierTestSequence, IIdentifierSequence>();

            objectContainer.RegisterInstanceAs<IClock>(SystemClock.Instance);
            objectContainer.RegisterTypeAs<CampaignCleaner, ICampaignCleaner>();
        }
    }
}
