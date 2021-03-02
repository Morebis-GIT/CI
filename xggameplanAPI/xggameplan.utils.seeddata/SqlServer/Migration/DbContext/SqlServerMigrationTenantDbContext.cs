using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using Microsoft.EntityFrameworkCore;
using xggameplan.utils.seeddata.SqlServer.Migration.EntityConfigurations;

namespace xggameplan.utils.seeddata.SqlServer.Migration.DbContext
{
    public class SqlServerMigrationTenantDbContext : SqlServerLongRunningTenantDbContext
    {
        public SqlServerMigrationTenantDbContext(DbContextOptions dbContextOptions) :
            base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            _ = modelBuilder.ApplyConfiguration(new MigrationHistoryEntityConfiguration());
        }
    }
}
