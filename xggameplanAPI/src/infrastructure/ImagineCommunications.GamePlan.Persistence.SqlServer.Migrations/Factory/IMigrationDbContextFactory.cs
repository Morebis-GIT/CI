using ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.Factory
{
    public interface IMigrationDbContextFactory<out TContext> where TContext : class, IMigrationDbContext
    {
        TContext CreateDbContext(string connectionString);

        TContext CreateDbContext();
    }
}
