using System;
using Autofac;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.DependencyInjection;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using ImagineCommunications.GamePlan.Utils.DataPurging.Helpers;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.Extensions.Configuration;
using xggameplan.core.DependencyInjection;
using xggameplan.core.Extensions;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.DependencyInjection
{
    /// <summary>
    /// Contains registrations of Autofac container.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class PurgingAutofacModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly CommandLineOptions _options;

        public PurgingAutofacModule(IConfiguration configuration, CommandLineOptions options)
        {
            _configuration = configuration;
            _options = options;
        }

        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterModule(new CloudModule(_configuration));

            _ = builder.RegisterInstance(_options).AsSelf();
            _ = builder.AddAutoMapper(typeof(AccessTokenProfile).Assembly);

            var dbProvider = _options.DbProviderType ??
                             ConnectionStringHelper.RecognizeDbProviderType(_options.ConnectionString);

            switch (dbProvider)
            {
                case DbProviderType.SqlServer:
                    _ = builder.RegisterModule<SqlServerInfrastructureModule>();
                    _ = builder.RegisterModule(
                        new SqlServerTenantModule<SqlServerTenantDbContext, SqlServerLongRunningTenantDbContext>(
                            _options.ConnectionString, new SqlServerDbContextRegistrationFeatures() { CommandTimeout = 600 }, new SqlServerDbContextRegistrationFeatures() { CommandTimeout = 600 }));
                    break;

                default:
                    throw new InvalidOperationException($"'{dbProvider}' is unsupported.");
            }
        }
    }
}
