using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Core;
using ImagineCommunications.GamePlan.Domain.Shared.System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping;
using NodaTime;
using Serilog;
using xggameplan.common.Caching;
using xggameplan.core.Extensions;
using xggameplan.utils.seeddata.Extensions;
using xggameplan.utils.seeddata.Helpers;
using xggameplan.utils.seeddata.RavenDb;
using xggameplan.utils.seeddata.SqlServer;

namespace xggameplan.utils.seeddata.Seeding
{
    public class Seeder
    {
        private readonly SeedOptions _options;

        public Seeder(SeedOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public bool Run(out string message)
        {
            message = "OK";
            using (var container = CreateDiContainer())
            {
                try
                {
                    var seedDataFolder = Path.Combine(_options.SeedDataFolder, _options.DatabaseType.ToString());
                    if (!AnySeedDataFiles(seedDataFolder))
                    {
                        message = $"No seed data was found at {seedDataFolder}";
                        return false;
                    }

                    var seedTypes = container.ComponentRegistry.Registrations
                        .SelectMany(r =>
                            r.Services.Where(s => s is KeyedService).Cast<KeyedService>().Where(s =>
                                    s.ServiceType.IsAssignableFrom(typeof(IJsonFileImporter)) && s.ServiceKey is Type)
                                .Select(s => s.ServiceKey))
                        .ToArray();

                    foreach (var type in seedTypes)
                    {
                        using var scope = container.BeginLifetimeScope();
                        var importer = scope.ResolveKeyed<IJsonFileImporter>(type);
                        var docCount = importer.GetDocumentCount();
                        if (docCount == 0 || _options.ReplaceExistingData)
                        {
                            _ = importer.Import(seedDataFolder, _options.ReplaceExistingData);
                        }
                    }

                    return true;

                    // Local functions
                    bool AnySeedDataFiles(string folder) =>
                        Directory.Exists(folder) &&
                        Directory.GetFiles(folder, "*.*").Any();
                }
                catch (Exception ex)
                {
                    container.Resolve<ILogger>().Error(ex.ToString());
                    throw;
                }
            }
        }

        protected IContainer CreateDiContainer()
        {
            var builder = new ContainerBuilder();

            _ = builder.AddAutoMapper(typeof(AccessTokenProfile).Assembly);

            var dbProvider = _options.DbProviderType ??
                ConnectionStringHelper.RecognizeDbProviderType(_options.ConnectionString);

            _ = builder.RegisterInstance(SystemClock.Instance).As<IClock>();
            _ = builder.RegisterType<InMemoryCache>().As<ICache>().InstancePerLifetimeScope();
            switch (dbProvider)
            {
                case DbProviderType.RavenDb:
                    _ = builder.RegisterModule(new RavenModule(_options.DatabaseType, _options.ConnectionString));
                    _ = builder.RegisterModule(new RavenDbImportersModule(_options.DatabaseType));
                    break;

                case DbProviderType.SqlServer:
                    _ = builder.RegisterModule(new SqlServerModule(_options.DatabaseType, _options.ConnectionString));
                    _ = builder.RegisterModule(new SqlServerImportersModule(_options.DatabaseType));
                    break;
            }

            builder.RegisterSerilog("serilogsettings.json");

            return builder.Build();
        }
    }
}
