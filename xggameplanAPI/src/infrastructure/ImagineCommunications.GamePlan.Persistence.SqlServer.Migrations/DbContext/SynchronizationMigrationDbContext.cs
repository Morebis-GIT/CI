using ImagineCommunications.Gameplan.Synchronization.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext
{
    public class SynchronizationMigrationDbContext : SynchronizationDbContext, IMigrationDbContext
    {
        public SynchronizationMigrationDbContext(DbContextOptions<SynchronizationDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }
    }
}
