using System;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations
{
    public class EfCoreMigrator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public EfCoreMigrator(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Migrate(CommandLineOptions options)
        {
            _logger.Information("Resolving context");

            var context = new MigrationContextResolver(_configuration)
                .ResolveDbContext(options);

            _logger.Information("Context resolved, checking...");

            var migrationsList = context.Database.GetPendingMigrations().ToArray();
            if (migrationsList.Any())
            {
                _logger.Information("Pending migrations found, applying...");
                _logger.Debug("Pending migrations: {migrationsList}", migrationsList);

                context.Database.Migrate();

                _logger.Information("Migrations applied successfully");
            }
            else
            {
                _logger.Warning("All migrations already applied");
            }
        }
    }
}
