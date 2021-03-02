using System;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Context;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory
{
    public class SynchronizationServiceMigrationContextFactory :
        IDesignTimeDbContextFactory<SynchronizationMigrationDbContext>,
        IMigrationDbContextFactory<SynchronizationMigrationDbContext>
    {
        private readonly IConfiguration _configuration;
        private readonly int _timeoutSetting;

        public SynchronizationServiceMigrationContextFactory()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("./config.json")
                .Build();

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(SynchronizationMigrationDbContext));
        }

        public SynchronizationServiceMigrationContextFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(SynchronizationMigrationDbContext));
        }

        public SynchronizationMigrationDbContext CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }

        public SynchronizationMigrationDbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SynchronizationDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                options => options.CommandTimeout(_timeoutSetting));

            return new SynchronizationMigrationDbContext(optionsBuilder.Options);
        }

        public SynchronizationMigrationDbContext CreateDbContext()
        {
            return CreateDbContext(_configuration.GetConnectionString("TenantDb"));
        }
    }
}
