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

#pragma warning disable DF0010 // Marks undisposed local variables.
            // Register this so it's also disposed.
            var ravenDb = new RavenDbDataStore();
            objectContainer.RegisterInstanceAs(ravenDb, null, true);
#pragma warning restore DF0010 // Marks undisposed local variables.

            // Stores and sessions should be registered as instances in order to
            // be disposed after scenario execution.
            objectContainer.RegisterInstanceAs(ravenDb.DocumentStore, null, true);
            objectContainer.RegisterInstanceAs(ravenDb.FilesStore, null, true);

            objectContainer.RegisterTypeAs<LocalFileCache, ILocalFileCache>();
        }
    }
}
