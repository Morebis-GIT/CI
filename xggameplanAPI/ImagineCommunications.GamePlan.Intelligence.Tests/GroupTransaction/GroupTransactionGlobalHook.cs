using System.IO;
using System.Linq;
using BoDi;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using ImagineCommunications.GamePlan.Intelligence.Configurations.Mappings;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.Interception;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Dependencies;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Dependencies;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Mappings;
using Moq;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction
{
    [Binding, Scope(Tag = "GroupTransaction")]
    public static class GroupTransactionGlobalHook
    {
        [BeforeFeature()]
        public static void BeforeFeature(IObjectContainer objectContainer, FeatureInfo featureInfo)
        {
            _ = objectContainer.AddAutoMapper(
                typeof(MappingProfile).Assembly,
                typeof(AccessTokenProfile).Assembly,
                typeof(SynchronizationProfile).Assembly,
                typeof(GroupTransactionInfoProfile).Assembly);

            objectContainer.RegisterTypeAs<MockContractValidatorService, IContractValidatorService>();
            objectContainer.RegisterInstanceAs(Mock.Of<ILoggerService>());
            objectContainer.RegisterInstanceAs<IBigMessageService>(Mock.Of<IBigMessageService>(
                service => service.SerializeMessage<IEvent>(It.IsAny<IEvent>()) == new MemoryStream()));
            var configurationServiceMock = new Mock<IConfigurationService>();
            _ = configurationServiceMock
                .Setup(m => m.GetData<long>(It.IsAny<string>()))
                .Returns<string>(s => s == "PayloadStorage:SerializedSizeThreshold" ? 1000000 : default);
            objectContainer.RegisterInstanceAs<IConfigurationService>(configurationServiceMock.Object);
            RegisterFtsInjectionProvider(objectContainer);
            objectContainer.RegisterScenarioDependency<GroupTransactionDbDependency>();
            objectContainer.RegisterScenarioDependency<GroupTransactionConsumerDependency>();
            objectContainer.RegisterScenarioDependency<SynchronizationScenarioDependency>();

            objectContainer.ResolveFeatureDependencies().ToList()
              .ForEach(dependency => dependency.Register(objectContainer));
        }

        [BeforeScenario]
        public static void BeforeScenario(IObjectContainer objectContainer)
        {
            objectContainer.Resolve<FeatureContext>().FeatureContainer.ResolveScenarioDependencies().ToList()
                .ForEach(dependency => dependency.Register(objectContainer));
        }

        [AfterScenario]
        public static void AfterScenario(IObjectContainer objectContainer)
        {
            if (objectContainer.IsRegistered<IntelligenceDbContext>())
            {
                _ = objectContainer.Resolve<IntelligenceDbContext>().Database.EnsureDeleted();
            }
        }

        private static void RegisterFtsInjectionProvider(IObjectContainer objectContainer)
        {
            var factory = new FtsInterceptionFactory();
            objectContainer.RegisterInstanceAs<IFtsInterceptionProvider>(factory);
        }
    }
}
