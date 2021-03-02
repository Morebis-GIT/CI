using System.Reflection;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
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
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Intelligence.Common;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.FileSystem;
using xggameplan.core.Validations;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Dependencies
{
    public class DatabaseDependencyModule : IDependencyModule
    {
        private readonly string _connectionString;

        public DatabaseDependencyModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Register(IServiceCollection services)
        {
            var documentStore = DocumentStoreFactory.CreateStore(_connectionString, Assembly.GetExecutingAssembly());
            services.AddSingleton<IDocumentStore>(documentStore);

            var filesStore = FileStoreFactory.CreateStore(_connectionString);
            services.AddSingleton<IFilesStore>(filesStore);

            services.AddScoped<IDocumentSession>(x =>
            {
                var docStore = x.GetService<IDocumentStore>();
                var documentConvention = new DocumentConvention();
                var jsonSerializer = documentConvention.CreateSerializer();
                docStore.Conventions.CreateSerializer();
                return docStore.OpenSession();
            });
            services.AddScoped<IAsyncDocumentSession>(x => x.GetService<IDocumentStore>().OpenAsyncSession());
            services.AddScoped<IAsyncFilesSession>(x => x.GetService<IFilesStore>().OpenAsyncSession());
            services.AddScoped<IIdentityGenerator, RavenIdentityGenerator>();
            services.AddScoped<IRavenIdentityGenerator, RavenIdentityGenerator>();

            //Repositories
            services.AddScoped<IBookingPositionRepository, RavenBookingPositionRepository>();
            services.AddScoped<IBookingPositionGroupRepository, RavenBookingPositionGroupRepository>();
            services.AddScoped<IUniverseRepository, RavenUniverseRepository>();
            services.AddScoped<ISalesAreaRepository, RavenSalesAreaRepository>();
            services.AddScoped<IDemographicRepository, RavenDemographicRepository>();
            services.AddScoped<IMetadataRepository, RavenMetadataRepository>();
            services.AddScoped<IScheduleRepository, RavenScheduleRepository>();
            services.AddScoped<IProgrammeRepository, RavenProgrammeRepository>();
            services.AddScoped<IProgrammeClassificationRepository, RavenProgrammeClassificationRepository>();
            services.AddScoped<IClashRepository, RavenClashRepository>();
            services.AddScoped<IProductRepository, RavenProductRepository>();
            services.AddScoped<IRatingsScheduleRepository, RavenRatingsScheduleRepository>();
            services.AddScoped<IRestrictionRepository, RavenRestrictionRepository>();
            services.AddScoped<IClearanceRepository, RavenClearanceRepository>();
            services.AddScoped<ISpotRepository, RavenSpotRepository>();
            services.AddScoped<IBreakRepository, RavenBreakRepository>();
            services.AddScoped<IBusinessTypeRepository, RavenBusinessTypeRepository>();
            services.AddScoped<IClashExceptionRepository, RavenClashExceptionRepository>();
            services.AddScoped<ICampaignRepository, RavenCampaignRepository>();
            services.AddScoped<ICampaignSettingsRepository, RavenCampaignSettingsRepository>();
            services.AddScoped<ITenantSettingsRepository, RavenTenantSettingsRepository>();
            services.AddScoped<IScenarioRepository, RavenScenarioRepository>();
            services.AddScoped<IPassRepository, RavenPassRepository>();
            services.AddScoped<ISalesAreaDemographicRepository, RavenSalesAreaDemographicRepository>();
            services.AddScoped<IProgrammeCategoryHierarchyRepository, RavenProgrammeCategoryHierarchyRepository>();
            services.AddScoped<IProgrammeEpisodeRepository, RavenProgrammeEpisodeRepository>();
            services.AddScoped<IInventoryTypeRepository, RavenInventoryTypeRepository>();
            services.AddScoped<IInventoryLockRepository, RavenInventoryLockRepository>();
            services.AddScoped<ILockTypeRepository, RavenLockTypeRepository>();
            services.AddScoped<ITotalRatingRepository, RavenTotalRatingRepository>();
            services.AddScoped<IStandardDayPartRepository, RavenStandardDayPartRepository>();
            services.AddScoped<IStandardDayPartGroupRepository, RavenStandardDayPartGroupRepository>();
            services.AddScoped<ILengthFactorRepository, RavenLengthFactorRepository>();

            // ValidationServices
            services.AddScoped<IClashExceptionValidations, ClashExceptionValidations>();
        }
    }
}
