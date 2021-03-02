using System;
using System.Globalization;
using System.IO;
using System.Linq;
using BoDi;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Mappings;
using ImagineCommunications.GamePlan.Intelligence.Configurations.Mappings;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Dependencies;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Dependencies;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Dependencies;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Extensions;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using Moq;
using TechTalk.SpecFlow;
using xggameplan.common.Caching;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange
{
    [Binding, Scope(Tag = "Exchange")]
    public static class DataExchangeGlobalHook
    {
        [BeforeFeature]
        public static void BeforeFeature(IObjectContainer objectContainer, FeatureInfo featureInfo)
        {
            _ = objectContainer.AddAutoMapper(
                typeof(MappingProfile).Assembly,
                typeof(AccessTokenProfile).Assembly,
                typeof(SynchronizationProfile).Assembly,
                typeof(GroupTransactionInfoProfile).Assembly,
                typeof(MappingOverride).Assembly);

            objectContainer.RegisterTypeAs<InMemoryCache, ICache>();
            objectContainer.RegisterTypeAs<ContractValidatorService, IContractValidatorService>();

            objectContainer.RegisterInstanceAs(Mock.Of<ILoggerService>());
            objectContainer.RegisterInstanceAs(Mock.Of<Serilog.ILogger>());
            objectContainer.RegisterInstanceAs<IBigMessageService>(Mock.Of<IBigMessageService>(
                service => service.SerializeMessage<IEvent>(It.IsAny<IEvent>()) == new MemoryStream()));
            var configurationServiceMock = new Mock<IConfigurationService>();
            _ = configurationServiceMock
                .Setup(m => m.GetData<long>(It.IsAny<string>()))
                .Returns<string>(s => s == "PayloadStorage:SerializedSizeThreshold" ? 1000000 : default);
            objectContainer.RegisterInstanceAs<IConfigurationService>(configurationServiceMock.Object);
            objectContainer.RegisterFeatureDependency<ImportModelFeatureDependency>();
            objectContainer.RegisterScenarioDependency<GroupTransactionDbDependency>();
            objectContainer.RegisterScenarioDependency<ResultCheckerScenarioDependency>();
            objectContainer.RegisterScenarioDependency<EventServiceScenarioDependency>();
            objectContainer.RegisterScenarioDependency<ConsumerScenarioDependency>();
            objectContainer.RegisterScenarioDependency<SynchronizationScenarioDependency>();

            var dataProviders = Enum.GetNames(typeof(DataProvider));
            var featureDataProviders = dataProviders.Intersect(featureInfo.Tags).ToArray();
            if (featureDataProviders.Length != 1)
            {
                throw new SpecFlowException("Data provider has not been defined.");
            }

            var dataProvider = (DataProvider)Enum.Parse(typeof(DataProvider), featureDataProviders.Single());
            switch (dataProvider)
            {
                case DataProvider.RavenDb:
                    RegisterFtsInjectionProvider(objectContainer);
                    objectContainer.RegisterFeatureDependency<RavenDbFeatureDependency>();
                    objectContainer.RegisterScenarioDependency<RavenDbScenarioDependency>();
                    break;

                case DataProvider.SqlServer:
                    RegisterFtsInjectionProvider(objectContainer);
                    objectContainer.RegisterFeatureDependency<SqlServerFeatureDependency>();
                    objectContainer.RegisterScenarioDependency<SqlServerScenarioDependency>();
                    break;
            }

            objectContainer.ResolveFeatureDependencies().ToList()
              .ForEach(dependency => dependency.Register(objectContainer));
        }

        [BeforeScenario]
        public static void BeforeScenario(IObjectContainer objectContainer)
        {
            objectContainer.Resolve<FeatureContext>().FeatureContainer.ResolveScenarioDependencies().ToList()
                .ForEach(dependency => dependency.Register(objectContainer));
            if (objectContainer.IsRegistered<IScenarioDbContext>())
            {
                objectContainer.Resolve<IScenarioDbContext>().Cleanup();
            }
            if (objectContainer.IsRegistered<IntelligenceDbContext>())
            {
                _ = objectContainer.Resolve<IntelligenceDbContext>().Database.EnsureDeleted();
                _ = objectContainer.Resolve<IntelligenceDbContext>().Database.EnsureCreated();
            }
        }

        [AfterScenario]
        public static void AfterScenario(IObjectContainer objectContainer)
        {
            if (objectContainer.IsRegistered<IScenarioDbContext>())
            {
                objectContainer.Resolve<IScenarioDbContext>().Cleanup();
            }
            if (objectContainer.IsRegistered<IntelligenceDbContext>())
            {
                _ = objectContainer.Resolve<IntelligenceDbContext>().Database.EnsureDeleted();
            }
        }

        private static void RegisterFtsInjectionProvider(IObjectContainer objectContainer)
        {
            var factory = new FtsInterceptionFactory();
            objectContainer.RegisterInstanceAs<IFtsInterceptionProvider>(factory);

            _ = factory.Register<Clash>(Clash.SearchField).SearchValue(c =>
                  FtsInterceptionHelpers.ComputedField(c.Externalref, c.Description));
            _ = factory.Register<Campaign>(Campaign.SearchTokensFieldName).SearchValue(c =>
                  FtsInterceptionHelpers.ComputedField(c.CampaignGroup, c.Name, c.ExternalId, c.BusinessType));
            _ = factory.Register<Product>(Product.SearchFieldName).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.Externalidentifier, p.Name));
            _ = factory.Register<Advertiser>(Advertiser.SearchFieldName).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.ExternalIdentifier, p.Name, p.ShortName));
            _ = factory.Register<Agency>(Advertiser.SearchFieldName).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.ExternalIdentifier, p.Name, p.ShortName));
            _ = factory.Register<ProgrammeDictionary>(ProgrammeDictionary.SearchField).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.ExternalReference, p.Name));
            _ = factory.Register<Scenario>(x => x.Name);
            _ = factory.Register<Scenario>(Scenario.SearchField).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.Id.ToString(), p.Name));
            _ = factory.Register<Pass>(x => x.Name);
            _ = factory.Register<Pass>(Pass.SearchField).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.Id.ToString(CultureInfo.InvariantCulture), p.Name));
            _ = factory.Register<RunAuthor>(x => x.Name);
            _ = factory.Register<Run>(Run.SearchField).SearchValue(p =>
                  FtsInterceptionHelpers.ComputedField(p.Id.ToString(), p.Description));
        }
    }
}
