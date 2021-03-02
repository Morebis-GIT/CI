using ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext
{
    public class TenantMigrationDbContext : SqlServerTenantDbContext, IMigrationDbContext
    {
        public TenantMigrationDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RemoveStoredProcEntities();
        }
    }
}
