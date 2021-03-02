using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory
{
    public class MasterMigrationContextFactory : IDesignTimeDbContextFactory<MasterMigrationDbContext>, IMigrationDbContextFactory<MasterMigrationDbContext>
    {
        private readonly IConfiguration _configuration;
        private readonly int _timeoutSetting;

        public MasterMigrationContextFactory()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("./config.json")
                .Build();

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(MasterMigrationDbContext));
        }

        public MasterMigrationContextFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(MasterMigrationDbContext));
        }

        public MasterMigrationDbContext CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }

        public MasterMigrationDbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasterMigrationDbContext>();
            optionsBuilder.UseMySql(connectionString,
                options => options.CommandTimeout(_timeoutSetting));

            return new MasterMigrationDbContext(optionsBuilder.Options);
        }

        public MasterMigrationDbContext CreateDbContext()
        {
            return CreateDbContext(_configuration.GetConnectionString("MasterDb"));
        }
    }
}
