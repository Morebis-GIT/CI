using System;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory
{
    public class IntelligenceMigrationContextFactory :
        IDesignTimeDbContextFactory<IntelligenceMigrationDbContext>,
        IMigrationDbContextFactory<IntelligenceMigrationDbContext>
    {
        private readonly IConfiguration _configuration;
        private readonly int _timeoutSetting;

        public IntelligenceMigrationContextFactory()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("./config.json")
                .Build();

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(IntelligenceMigrationDbContext));
        }

        public IntelligenceMigrationContextFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _timeoutSetting = _configuration.GetSection("TimeoutSettings")
                .GetValue<int>(nameof(IntelligenceMigrationDbContext));
        }

        public IntelligenceMigrationDbContext CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }

        public IntelligenceMigrationDbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntelligenceDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                options => options.CommandTimeout(_timeoutSetting));

            return new IntelligenceMigrationDbContext(optionsBuilder.Options);
        }

        public IntelligenceMigrationDbContext CreateDbContext()
        {
            return CreateDbContext(_configuration.GetConnectionString("IntelligenceDb"));
        }
    }
}
