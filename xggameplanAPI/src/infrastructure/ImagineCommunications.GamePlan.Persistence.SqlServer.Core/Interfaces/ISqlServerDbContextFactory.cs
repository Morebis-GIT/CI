namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ISqlServerDbContextFactory<out TDbContext> where TDbContext : ISqlServerDbContext
    {
        TDbContext Create();
    }
}
