using Autofac;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.Spots;
using Moq;
using xggameplan.core.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    [Trait("Factory", nameof(IRepositoryScope.CreateRepositories))]
    public static class RepositoryFactoryTests
    {
        private static IContainer CreateTestContainer()
        {
            var builder = new ContainerBuilder();

            _ = builder.RegisterInstance(Mock.Of<IAWSInstanceConfigurationRepository>()).As<IAWSInstanceConfigurationRepository>();
            _ = builder.RegisterInstance(Mock.Of<IAutoBookInstanceConfigurationRepository>()).As<IAutoBookInstanceConfigurationRepository>();
            _ = builder.RegisterInstance(Mock.Of<IAutoBookRepository>()).As<IAutoBookRepository>();
            _ = builder.RegisterInstance(Mock.Of<IAutoBookSettingsRepository>()).As<IAutoBookSettingsRepository>();
            _ = builder.RegisterInstance(Mock.Of<IBreakRepository>()).As<IBreakRepository>();
            _ = builder.RegisterInstance(Mock.Of<ICampaignRepository>()).As<ICampaignRepository>();
            _ = builder.RegisterInstance(Mock.Of<ICampaignSettingsRepository>()).As<ICampaignSettingsRepository>();
            _ = builder.RegisterInstance(Mock.Of<IClashExceptionRepository>()).As<IClashExceptionRepository>();
            _ = builder.RegisterInstance(Mock.Of<IClashRepository>()).As<IClashRepository>();
            _ = builder.RegisterInstance(Mock.Of<IClearanceRepository>()).As<IClearanceRepository>();
            _ = builder.RegisterInstance(Mock.Of<IDemographicRepository>()).As<IDemographicRepository>();
            _ = builder.RegisterInstance(Mock.Of<IFailuresRepository>()).As<IFailuresRepository>();
            _ = builder.RegisterInstance(Mock.Of<IISRSettingsRepository>()).As<IISRSettingsRepository>();
            _ = builder.RegisterInstance(Mock.Of<IMetadataRepository>()).As<IMetadataRepository>();
            _ = builder.RegisterInstance(Mock.Of<IOutputFileRepository>()).As<IOutputFileRepository>();
            _ = builder.RegisterInstance(Mock.Of<IPassRepository>()).As<IPassRepository>();
            _ = builder.RegisterInstance(Mock.Of<IProductRepository>()).As<IProductRepository>();
            _ = builder.RegisterInstance(Mock.Of<IProgrammeDictionaryRepository>()).As<IProgrammeDictionaryRepository>();
            _ = builder.RegisterInstance(Mock.Of<IProgrammeRepository>()).As<IProgrammeRepository>();
            _ = builder.RegisterInstance(Mock.Of<IRSSettingsRepository>()).As<IRSSettingsRepository>();
            _ = builder.RegisterInstance(Mock.Of<IRatingsScheduleRepository>()).As<IRatingsScheduleRepository>();
            _ = builder.RegisterInstance(Mock.Of<IRecommendationRepository>()).As<IRecommendationRepository>();
            _ = builder.RegisterInstance(Mock.Of<IRestrictionRepository>()).As<IRestrictionRepository>();
            _ = builder.RegisterInstance(Mock.Of<IRunTypeRepository>()).As<IRunTypeRepository>();
            _ = builder.RegisterInstance(Mock.Of<ISalesAreaRepository>()).As<ISalesAreaRepository>();
            _ = builder.RegisterInstance(Mock.Of<IScenarioCampaignFailureRepository>()).As<IScenarioCampaignFailureRepository>();
            _ = builder.RegisterInstance(Mock.Of<IScenarioCampaignResultRepository>()).As<IScenarioCampaignResultRepository>();
            _ = builder.RegisterInstance(Mock.Of<IScenarioRepository>()).As<IScenarioRepository>();
            _ = builder.RegisterInstance(Mock.Of<IScenarioResultRepository>()).As<IScenarioResultRepository>();
            _ = builder.RegisterInstance(Mock.Of<IScheduleRepository>()).As<IScheduleRepository>();
            _ = builder.RegisterInstance(Mock.Of<ISpotRepository>()).As<ISpotRepository>();
            _ = builder.RegisterInstance(Mock.Of<ITenantSettingsRepository>()).As<ITenantSettingsRepository>();
            _ = builder.RegisterInstance(Mock.Of<IUniverseRepository>()).As<IUniverseRepository>();

            return builder.Build();
        }

        [Fact]
        public static void CreateRepository_CanCreateRepositoriesWithMultipleParameters()
        {
            var listOfTypes = new[]
            {
                typeof(IAWSInstanceConfigurationRepository),
                typeof(IAutoBookInstanceConfigurationRepository),
                typeof(IAutoBookRepository),
                typeof(IAutoBookSettingsRepository),
                typeof(IBreakRepository),
                typeof(ICampaignRepository),
                typeof(IClashExceptionRepository),
                typeof(IClashRepository),
                typeof(IClearanceRepository),
                typeof(IDemographicRepository),
                typeof(IFailuresRepository),
                typeof(IISRSettingsRepository),
                typeof(IMetadataRepository),
                typeof(IOutputFileRepository),
                typeof(IPassRepository),
                typeof(IProductRepository),
                typeof(IProgrammeDictionaryRepository),
                typeof(IProgrammeRepository),
                typeof(IRSSettingsRepository),
                typeof(IRatingsScheduleRepository),
                typeof(IRecommendationRepository),
                typeof(IRestrictionRepository),
                typeof(IRunTypeRepository),
                typeof(ISalesAreaRepository),
                typeof(IScenarioCampaignFailureRepository),
                typeof(IScenarioCampaignResultRepository),
                typeof(IScenarioRepository),
                typeof(IScenarioResultRepository),
                typeof(IScheduleRepository),
                typeof(ISpotRepository),
                typeof(ITenantSettingsRepository),
                typeof(IUniverseRepository)
            };

            using var container = CreateTestContainer();
            var repositoryFactory = new RepositoryFactory(container);

            using var scope = repositoryFactory.BeginRepositoryScope();

            var result = scope.CreateRepositories(listOfTypes);

            _ = result.Should().NotContainValue(null);
            _ = result.Should().HaveSameCount(listOfTypes);
        }
    }
}
