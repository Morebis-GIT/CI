using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection;
using xggameplan.utils.seeddata.Infrastructure;

namespace xggameplan.utils.seeddata.RavenDb
{
    public class RavenModule : Module
    {
        private readonly DatabaseType _databaseType;
        private readonly string _connectionString;

        public RavenModule(DatabaseType databaseType, string connectionString)
        {
            _databaseType = databaseType;
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            switch (_databaseType)
            {
                case DatabaseType.Master:
                    //builder.RegisterModule(new RavenMasterAutofacModule(_connectionString));
                    _ = builder.RegisterModule(new RavenDbMasterModule(_connectionString));
                    break;
                case DatabaseType.Tenant:
                    _ = builder.RegisterModule(new RavenDbTenantModule(_connectionString));
                    break;
            }

            _ = builder.RegisterType<SeedRavenDomainModelContext>()
                .As<IDomainModelContext>()
                .InstancePerLifetimeScope();

            _ = builder.RegisterType<PageReadingOptions>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
