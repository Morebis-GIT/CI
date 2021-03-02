using AutoMapper;
using BoDi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
{
    public class RavenDbFeatureDependency : IFeatureDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            var mapperConfig = new MapperConfiguration(expression => expression.AddMaps(typeof(AccessTokenProfile).Assembly));
            objectContainer.RegisterInstanceAs<IConfigurationProvider>(mapperConfig);
            objectContainer.RegisterFactoryAs<IMapper>(container => new Mapper(container.Resolve<IConfigurationProvider>()));

            //stores and sessions should be registered as instances in order to be disposed after scenario execution
            var ravenDb = new RavenDbDataStore();

            objectContainer.RegisterInstanceAs(ravenDb.DocumentStore, null, true);
            objectContainer.RegisterInstanceAs(ravenDb.FilesStore, null, true);

            objectContainer.RegisterTypeAs<LocalFileCache, ILocalFileCache>();
        }
    }
}
