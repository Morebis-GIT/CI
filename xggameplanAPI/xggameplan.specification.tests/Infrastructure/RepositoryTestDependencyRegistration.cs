using System;
using System.Globalization;
using BoDi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Infrastructure.Dependencies;
using xggameplan.specification.tests.Infrastructure.EnvironmentSettings;
using xggameplan.specification.tests.Infrastructure.RavenDb;
using xggameplan.specification.tests.Infrastructure.SqlServer;
using xggameplan.specification.tests.Infrastructure.SqlServer.Interception;

namespace xggameplan.specification.tests.Infrastructure
{
    public class RepositoryTestDependencyRegistration
    {
        public RepositoryTestDependencyRegistration(IObjectContainer objectContainer, FeatureInfo featureInfo)
        {
            if (objectContainer == null)
            {
                throw new ArgumentNullException(nameof(objectContainer));
            }

            var dataProvider = GetDataProvider(objectContainer);
            if (dataProvider == null)
            {
                return;
            }

            switch (dataProvider)
            {
                case DataProvider.RavenDb:
                    objectContainer.RegisterFeatureDependency<RavenDbFeatureDependency>();
                    objectContainer.RegisterScenarioDependency<RavenDbScenarioDependency>();
                    break;

                case DataProvider.SqlServer:
                    RegisterFtsInjectionProvider(objectContainer);
                    objectContainer.RegisterFeatureDependency<SqlServerFeatureDependency>();
                    objectContainer.RegisterScenarioDependency<SqlServerScenarioDependency>();
                    break;
            }

            objectContainer.RegisterFeatureDependency<ImportModelFeatureDependency>();
            objectContainer.RegisterScenarioDependency<RepositoryAdapterScenarioDependency>();
        }

        private DataProvider? GetDataProvider(IObjectContainer objectContainer)
        {
            var environmentSettings = objectContainer.Resolve<IEnvironmentSettings>();
            var dataProvider = environmentSettings.GetDataProvider();
            return dataProvider;
        }

        private void RegisterFtsInjectionProvider(IObjectContainer objectContainer)
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
