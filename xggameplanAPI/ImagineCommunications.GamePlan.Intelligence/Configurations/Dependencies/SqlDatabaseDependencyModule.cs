using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Intelligence.Common;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.RunCleaning;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Fts;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using xggameplan.core.Interfaces;
using xggameplan.core.Validations;
using CampaignCleaner = ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.CampaignCleaning.CampaignCleaner;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Dependencies
{
    public class SqlDatabaseDependencyModule : IDependencyModule
    {
        private readonly string _connectionString;
        private readonly int _timeout;

        public SqlDatabaseDependencyModule(string connectionString, int timeout)
        {
            _connectionString = connectionString;
            _timeout = timeout;
        }

        public void Register(IServiceCollection services)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SqlServerLongRunningTenantDbContext>()
                .UseMySql(_connectionString, options => options.CommandTimeout(_timeout))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseBulkInsertEngine(dbContext => new SqlServerBulkInsertEngine(dbContext, new SqlServerBulkInsertEngineOptionsBuilder<SqlServerBulkInsertEngineOptions>().Options));

            _ = services.AddSingleton(dbContextOptionsBuilder.Options);
            _ = services.AddSingleton<DbContextOptions>(sp => sp.GetService<DbContextOptions<SqlServerLongRunningTenantDbContext>>());

            _ = services
                .AddScoped<ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext>>(sp =>
                    new SqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext,
                        SqlServerLongRunningTenantDbContext>(
                        sp.GetRequiredService<DbContextOptions<SqlServerLongRunningTenantDbContext>>()))
                .AddScoped<ISqlServerDbContextFactory<ISqlServerTenantDbContext>>(sp =>
                    new SqlServerDbContextFactory<ISqlServerTenantDbContext,
                        SqlServerLongRunningTenantDbContext>(
                        sp.GetRequiredService<DbContextOptions<SqlServerLongRunningTenantDbContext>>()));

            // explicitly register DbContext
            _ = services.AddScoped(sp =>
                sp.GetRequiredService<ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext>>().Create());

            // delegate requests for the interfaces to the concrete type
            _ = services.AddScoped<ISqlServerTenantDbContext>(x => x.GetRequiredService<ISqlServerLongRunningTenantDbContext>());
            _ = services.AddScoped<ISqlServerDbContext>(x => x.GetRequiredService<ISqlServerLongRunningTenantDbContext>());
            _ = services.AddScoped<ITenantDbContext>(x => x.GetRequiredService<ISqlServerLongRunningTenantDbContext>());
            _ = services.AddScoped<IDbContext>(x => x.GetRequiredService<ISqlServerLongRunningTenantDbContext>());

            _ = services.AddScoped<IFullTextSearchConditionBuilder, FullTextSearchConditionBuilder>();
            _ = services.AddScoped<ILockTypeRepository, LockTypeRepository>();
            _ = services.AddScoped<IInventoryTypeRepository, InventoryTypeRepository>();
            _ = services.AddScoped<IInventoryLockRepository, InventoryLockRepository>();
            _ = services.AddScoped<ITotalRatingRepository, TotalRatingRepository>();
            _ = services.AddScoped<IUniverseRepository, UniverseRepository>();
            _ = services.AddScoped<ISalesAreaRepository, SalesAreaRepository>();
            _ = services.AddScoped<IScenarioCampaignFailureRepository, ScenarioCampaignFailureRepository>();
            _ = services.AddScoped<IDemographicRepository, DemographicRepository>();
            _ = services.AddScoped<IMetadataRepository, MetadataRepository>();
            _ = services.AddScoped<IProgrammeRepository, ProgrammeRepository>();
            _ = services.AddScoped<IProgrammeClassificationRepository, ProgrammeClassificationRepository>();
            _ = services.AddScoped<IClashRepository, ClashRepository>();
            _ = services.AddScoped<IProductRepository, LandmarkProductRepository>();
            _ = services.AddScoped<IRestrictionRepository, LandmarkRestrictionRepository>();
            _ = services.AddScoped<IClearanceRepository, ClearanceRepository>();
            _ = services.AddScoped<ISpotRepository, SpotRepository>();
            _ = services.AddScoped<IBookingPositionRepository, BookingPositionRepository>();
            _ = services.AddScoped<IBookingPositionGroupRepository, BookingPositionGroupRepository>();
            _ = services.AddScoped<IBreakRepository, BreakRepository>();
            _ = services.AddScoped<IBusinessTypeRepository, BusinessTypeRepository>();
            _ = services.AddScoped<IClashExceptionRepository, ClashExceptionRepository>();
            _ = services.AddScoped<ICampaignRepository, LandmarkCampaignRepository>();
            _ = services.AddScoped<ICampaignSettingsRepository, CampaignSettingsRepository>();
            _ = services.AddScoped<ITenantSettingsRepository, TenantSettingsRepository>();
            _ = services.AddScoped<IProgrammeCategoryHierarchyRepository, ProgrammeCategoryHierarchyRepository>();
            _ = services.AddScoped<IRatingsScheduleRepository, RatingsScheduleRepository>();
            _ = services.AddScoped<ISalesAreaDemographicRepository, SalesAreaDemographicRepository>();
            _ = services.AddScoped<IScenarioRepository, ScenarioRepository>();
            _ = services.AddScoped<IPassRepository, PassRepository>();
            _ = services.AddScoped<IScheduleRepository, ScheduleRepository>();
            _ = services.AddScoped<IStandardDayPartRepository, StandardDayPartRepository>();
            _ = services.AddScoped<IStandardDayPartGroupRepository, StandardDayPartGroupRepository>();
            _ = services.AddScoped<IProgrammeEpisodeRepository, ProgrammeEpisodeRepository>();
            _ = services.AddScoped<IRunTypeRepository, RunTypeRepository>();
            _ = services.AddScoped<ILengthFactorRepository, LengthFactorRepository>();
            _ = services.AddScoped<ISpotBookingRuleRepository, SpotBookingRuleRepository>();

            // ValidationServices
            _ = services.AddScoped<IClashExceptionValidations, ClashExceptionValidations>();

            // SQL Services
            _ = services.AddScoped<SqlServerTenantIdentifierSequence>();
            _ = services.AddScoped<ISqlServerTenantIdentifierSequence>(x => x.GetRequiredService<SqlServerTenantIdentifierSequence>());
            _ = services.AddScoped<ISqlServerIdentifierSequence>(x => x.GetRequiredService<SqlServerTenantIdentifierSequence>());
            _ = services.AddScoped<ITenantIdentifierSequence>(x => x.GetRequiredService<SqlServerTenantIdentifierSequence>());
            _ = services.AddScoped<IIdentifierSequence>(x => x.GetRequiredService<SqlServerTenantIdentifierSequence>());

            _ = services.AddScoped<SqlServerIdentityGenerator>();
            _ = services.AddScoped<ISqlServerIdentityGenerator>(x => x.GetRequiredService<SqlServerIdentityGenerator>());
            _ = services.AddScoped<IIdentityGenerator>(x => x.GetRequiredService<SqlServerIdentityGenerator>());

            _ = services.AddScoped<IDatabaseIndexAwaiter, SqlServerDatabaseIndexAwaiter>();
            _ = services.AddScoped<IResultsFileStorage, ResultsFileTenantDbStorage>();

            _ = services.AddScoped<IRunCleaner, RunCleaner>();
            _ = services.AddScoped<ISpotCleaner, SpotCleaner>();
            _ = services.AddScoped<ICampaignCleaner, CampaignCleaner>();

            _ = services.AddScoped<SalesAreaCacheAccessor>();
            _ = services.AddScoped<ISqlServerSalesAreaByIdCacheAccessor>(x => x.GetRequiredService<SalesAreaCacheAccessor>());
            _ = services.AddScoped<ISqlServerSalesAreaByNullableIdCacheAccessor>(x => x.GetRequiredService<SalesAreaCacheAccessor>());
            _ = services.AddScoped<ISqlServerSalesAreaByNameCacheAccessor>(x => x.GetRequiredService<SalesAreaCacheAccessor>());

        }
    }
}
