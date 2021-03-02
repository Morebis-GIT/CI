using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Infrastructure.Assist;
using xggameplan.specification.tests.Infrastructure.EnvironmentSettings;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure
{
    [Binding]
    public class ManageDataStorageHook
    {
        [BeforeTestRun]
        public static void BeforeTestRun(IObjectContainer objectContainer)
        {
            var durationValueRetriever = new DurationValueRetriever();
            Service.Instance.ValueRetrievers.Register(durationValueRetriever);
            Service.Instance.ValueComparers.Register(new DurationValueComparer(durationValueRetriever));
            var dateRangeValueRetriever = new DateRangeValueRetriever(new DateTimeValueRetriever());
            Service.Instance.ValueRetrievers.Register(dateRangeValueRetriever);
            Service.Instance.ValueComparers.Register(new DateRangeValueComparer(dateRangeValueRetriever));
            var bufferValueRetriever = new BufferValueRetriever();
            Service.Instance.ValueRetrievers.Register(bufferValueRetriever);
            Service.Instance.ValueComparers.Register(new BufferValueComparer(bufferValueRetriever));
        }

        [BeforeFeature("ManageDataStorage")]
        public static void BeforeFeature(IObjectContainer objectContainer, RepositoryTestDependencyRegistration dependencyRegistration)
        {
            objectContainer.ResolveFeatureDependencies().ToList()
                .ForEach(dependency => dependency.Register(objectContainer));

            var environmentSettings = objectContainer.Resolve<IEnvironmentSettings>();
            var featureContext = objectContainer.Resolve<FeatureContext>();

            IgnoreDataProviderSpecificFeature(featureContext, environmentSettings);
        }

        private static void IgnoreDataProviderSpecificFeature(FeatureContext featureContext, IEnvironmentSettings environmentSettings)
        {
            var dataProvider = environmentSettings.GetDataProvider();
            if (dataProvider == null)
            {
                featureContext.IgnoreFeature("DataProvider is not defined");
            }

            var featureInfo = featureContext.FeatureInfo;

            var specificDataProvider = GetFeatureSpecificDataProvider(featureInfo);

            if (dataProvider == DataProvider.SqlServer && specificDataProvider == DataProvider.RavenDb)
            {
                featureContext.IgnoreFeature($"Current feature is {DataProvider.RavenDb} specific.");
            }

            if (dataProvider == DataProvider.RavenDb && specificDataProvider == DataProvider.SqlServer)
            {
                featureContext.IgnoreFeature($"Current feature is {DataProvider.SqlServer} specific.");
            }
        }

        private static DataProvider? GetFeatureSpecificDataProvider(FeatureInfo featureInfo)
        {
            var enumNames = Enum.GetNames(typeof(DataProvider));
            var featureDataProviders = enumNames.Intersect(featureInfo.Tags).ToArray();

            if (featureDataProviders.Length == 0)
            {
                return null;
            }

            if (featureDataProviders.Length == 1)
            {
                return (DataProvider)Enum.Parse(typeof(DataProvider), featureDataProviders.Single());
            }

            throw new SpecFlowException("Wrong Data provider defined.");
        }

        [BeforeScenario]
        public void BeforeScenario(IObjectContainer objectContainer)
        {
            objectContainer.Resolve<FeatureContext>().FeatureContainer.ResolveScenarioDependencies().ToList()
                .ForEach(dependency => dependency.Register(objectContainer));
        }

        [AfterScenario]
        public void AfterScenario(IObjectContainer objectContainer)
        {
            if (objectContainer.IsRegistered<IScenarioDbContext>())
            {
                objectContainer.Resolve<IScenarioDbContext>().Cleanup();
            }
        }

        [StepArgumentTransformation(@"(try to |)")]
        public bool ThrowException(string tryTo)
        {
            return !"try to ".Equals(tryTo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
