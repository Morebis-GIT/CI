using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory
{
    public class TenantMigrationContextFactory : IDesignTimeDbContextFactory<TenantMigrationDbContext>, IMigrationDbContextFactory<TenantMigrationDbContext>
    {
        private readonly IConfiguration _configuration;
        private readonly int _timeoutSetting;

        public TenantMigrationContextFactory()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("./config.json")
                .Build();

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(TenantMigrationDbContext));
        }

        public TenantMigrationContextFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(TenantMigrationDbContext));
        }

        public TenantMigrationDbContext CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }

        public TenantMigrationDbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantMigrationDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                options => options.CommandTimeout(_timeoutSetting));

            return new TenantMigrationDbContext(optionsBuilder.Options);
        }

        public TenantMigrationDbContext CreateDbContext()
        {
            return CreateDbContext(_configuration.GetConnectionString("TenantDb"));
        }
    }
}
