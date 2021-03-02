using AutoMapper;
using BoDi;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerFeatureDependency : IFeatureDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            var mapperConfig = new MapperConfiguration(expression =>
                expression.AddMaps(typeof(AccessTokenProfile).Assembly));
            objectContainer.RegisterInstanceAs<IConfigurationProvider>(mapperConfig);
            objectContainer.RegisterFactoryAs<IMapper>(container => new Mapper(container.Resolve<IConfigurationProvider>()));

            objectContainer.RegisterTypeAs<LocalFileCache, ILocalFileCache>();
        }
    }
}
