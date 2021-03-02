using Autofac;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Persistence.RavenDb.DependencyInjection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using Microsoft.Extensions.Configuration;
using xggameplan.common.Extensions;

namespace xggameplan.taskexecutor.DependencyInjection
{
    public class MasterModule : Module
    {
        private readonly IConfiguration _applicationConfiguration;

        public MasterModule(IConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Register master db
            var masterDbConnectionString = _applicationConfiguration["db:master:connectionString"];
            var provider = _applicationConfiguration.GetDbProvider();
            switch (provider)
            {
                case DbProviderType.RavenDb:
                    _ = builder.RegisterModule(new RavenDbMasterModule(masterDbConnectionString));
                    break;

                case DbProviderType.SqlServer:
                    _ = builder.RegisterModule<SqlServerInfrastructureModule>();
                    _ = builder.RegisterModule(new SqlServerMasterModule(masterDbConnectionString));
                    break;
            }
        }
    }
}
