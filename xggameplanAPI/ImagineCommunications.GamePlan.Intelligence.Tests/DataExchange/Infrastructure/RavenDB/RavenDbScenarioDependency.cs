using BoDi;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Maintenance.DatabaseDetail;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
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
using ImagineCommunications.GamePlan.Domain.Shared.System.AccessTokens;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;
using Raven.Client;
using Raven.Client.FileSystem;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    public class RavenDbScenarioDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterTypeAs<RavenAccessTokensRepository, IAccessTokensRepository>();
            objectContainer.RegisterTypeAs<RavenAutoBookInstanceConfigurationRepository, IAutoBookInstanceConfigurationRepository>();
            objectContainer.RegisterTypeAs<RavenAutoBookRepository, IAutoBookRepository>();
            objectContainer.RegisterTypeAs<RavenAutoBookSettingsRepository, IAutoBookSettingsRepository>();
            objectContainer.RegisterTypeAs<RavenAWSInstanceConfigurationRepository, IAWSInstanceConfigurationRepository>();
            objectContainer.RegisterTypeAs<RavenBreakRepository, IBreakRepository>();
            objectContainer.RegisterTypeAs<RavenBusinessTypeRepository, IBusinessTypeRepository>();
            objectContainer.RegisterTypeAs<RavenCampaignRepository, ICampaignRepository>();
            objectContainer.RegisterTypeAs<RavenCampaignSettingsRepository, ICampaignSettingsRepository>();
            objectContainer.RegisterTypeAs<RavenChannelRepository, IChannelsRepository>();
            objectContainer.RegisterTypeAs<RavenClashExceptionRepository, IClashExceptionRepository>();
            objectContainer.RegisterTypeAs<RavenClashRepository, IClashRepository>();
            objectContainer.RegisterTypeAs<RavenClearanceRepository, IClearanceRepository>();
            objectContainer.RegisterTypeAs<RavenDatabaseDetailsRepository, IDatabaseDetailsRepository>();
            objectContainer.RegisterTypeAs<RavenDemographicRepository, IDemographicRepository>();
            objectContainer.RegisterTypeAs<RavenEfficiencySettingsRepository, IEfficiencySettingsRepository>();
            objectContainer.RegisterTypeAs<RavenFailuresRepository, IFailuresRepository>();
            objectContainer.RegisterTypeAs<RavenFunctionalAreaRepository, IFunctionalAreaRepository>();
            objectContainer.RegisterTypeAs<RavenIndexTypeRepository, IIndexTypeRepository>();
            objectContainer.RegisterTypeAs<RavenISRSettingsRepository, IISRSettingsRepository>();
            objectContainer.RegisterTypeAs<RavenKPIComparisonConfigRepository, IKPIComparisonConfigRepository>();
            objectContainer.RegisterTypeAs<RavenLanguageRepository, ILanguageRepository>();
            objectContainer.RegisterTypeAs<RavenLibrarySalesAreaPassPrioritiesRepository, ILibrarySalesAreaPassPrioritiesRepository>();
            objectContainer.RegisterTypeAs<RavenMetadataRepository, IMetadataRepository>();
            objectContainer.RegisterTypeAs<RavenOutputFileRepository, IOutputFileRepository>();
            objectContainer.RegisterTypeAs<RavenPassRepository, IPassRepository>();
            objectContainer.RegisterTypeAs<RavenProductRepository, IProductRepository>();
            objectContainer.RegisterTypeAs<RavenProductSettingsRepository, IProductSettingsRepository>();
            objectContainer.RegisterTypeAs<RavenProgrammeClassificationRepository, IProgrammeClassificationRepository>();
            objectContainer.RegisterTypeAs<RavenProgrammeDictionaryRepository, IProgrammeDictionaryRepository>();
            objectContainer.RegisterTypeAs<RavenProgrammeRepository, IProgrammeRepository>();
            objectContainer.RegisterTypeAs<RavenRatingsScheduleRepository, IRatingsScheduleRepository>();
            objectContainer.RegisterTypeAs<RavenRecommendationRepository, IRecommendationRepository>();
            objectContainer.RegisterTypeAs<RavenRestrictionRepository, IRestrictionRepository>();
            objectContainer.RegisterTypeAs<RavenResultsFileRepository, IResultsFileRepository>();
            objectContainer.RegisterTypeAs<RavenRSSettingsRepository, IRSSettingsRepository>();
            objectContainer.RegisterTypeAs<RavenRunRepository, IRunRepository>();
            objectContainer.RegisterTypeAs<RavenSalesAreaRepository, ISalesAreaRepository>();
            objectContainer.RegisterTypeAs<RavenScenarioRepository, IScenarioRepository>();
            objectContainer.RegisterTypeAs<RavenScenarioResultRepository, IScenarioResultRepository>();
            objectContainer.RegisterTypeAs<RavenScheduleRepository, IScheduleRepository>();
            objectContainer.RegisterTypeAs<RavenSmoothConfigurationRepository, ISmoothConfigurationRepository>();
            objectContainer.RegisterTypeAs<RavenSmoothFailureMessageRepository, ISmoothFailureMessageRepository>();
            objectContainer.RegisterTypeAs<RavenSmoothFailureRepository, ISmoothFailureRepository>();
            objectContainer.RegisterTypeAs<RavenSpotPlacementRepository, ISpotPlacementRepository>();
            objectContainer.RegisterTypeAs<RavenSpotRepository, ISpotRepository>();
            objectContainer.RegisterTypeAs<RavenTaskInstanceRepository, ITaskInstanceRepository>();
            objectContainer.RegisterTypeAs<RavenTenantSettingsRepository, ITenantSettingsRepository>();
            objectContainer.RegisterTypeAs<RavenUniverseRepository, IUniverseRepository>();
            objectContainer.RegisterTypeAs<RavenUpdateDetailsRepository, IUpdateDetailsRepository>();
            objectContainer.RegisterTypeAs<RavenBookingPositionRepository, IBookingPositionRepository>();
            objectContainer.RegisterTypeAs<RavenBookingPositionGroupRepository, IBookingPositionGroupRepository>();
            objectContainer.RegisterTypeAs<RavenSalesAreaDemographicRepository, ISalesAreaDemographicRepository>();
            objectContainer.RegisterTypeAs<RavenProgrammeCategoryHierarchyRepository, IProgrammeCategoryHierarchyRepository>();
            objectContainer.RegisterTypeAs<RavenInventoryTypeRepository, IInventoryTypeRepository>();
            objectContainer.RegisterTypeAs<RavenInventoryLockRepository, IInventoryLockRepository>();
            objectContainer.RegisterTypeAs<RavenLockTypeRepository, ILockTypeRepository>();
            objectContainer.RegisterTypeAs<RavenTotalRatingRepository, ITotalRatingRepository>();
            objectContainer.RegisterTypeAs<RavenStandardDayPartRepository, IStandardDayPartRepository>();
            objectContainer.RegisterTypeAs<RavenStandardDayPartGroupRepository, IStandardDayPartGroupRepository>();
            objectContainer.RegisterTypeAs<RavenProgrammeEpisodeRepository, IProgrammeEpisodeRepository>();
            objectContainer.RegisterTypeAs<RavenLengthFactorRepository, ILengthFactorRepository>();

            var documentStore = objectContainer.Resolve<IDocumentStore>();
            objectContainer.RegisterInstanceAs(documentStore.OpenSession(), null, true);
            objectContainer.RegisterInstanceAs(documentStore.OpenAsyncSession(), null, true);

            objectContainer.RegisterFactoryAs<IRavenTestDbContext>(oc =>
            {
                var dbContext = new RavenTestDbContext(oc.Resolve<IDocumentSession>(), oc.Resolve<IAsyncDocumentSession>());
                dbContext.Specific.BulkInsertOptions.IsWaitForLastTaskToFinish = true;
                return dbContext;
            });
            objectContainer.RegisterFactoryAs<IRavenDbContext>(oc => oc.Resolve<IRavenTestDbContext>());
            objectContainer.RegisterFactoryAs<IMasterDbContext>(oc => oc.Resolve<IRavenTestDbContext>());
            objectContainer.RegisterFactoryAs<ITenantDbContext>(oc => oc.Resolve<IRavenTestDbContext>());
            objectContainer.RegisterFactoryAs<IRavenMasterDbContext>(oc => oc.Resolve<IRavenTestDbContext>());
            objectContainer.RegisterFactoryAs<IRavenTenantDbContext>(oc => oc.Resolve<IRavenTestDbContext>());

            var fileStore = objectContainer.Resolve<IFilesStore>();
            objectContainer.RegisterInstanceAs(fileStore.OpenAsyncSession(), null, true);

            objectContainer.RegisterTypeAs<RavenIdentityGenerator, IIdentityGenerator>();
            objectContainer.RegisterTypeAs<RavenScenarioDbContext, IScenarioDbContext>();
            objectContainer.RegisterTypeAs<RavenDbJsonTestDataImporter, ITestDataImporter>();
        }
    }
}
