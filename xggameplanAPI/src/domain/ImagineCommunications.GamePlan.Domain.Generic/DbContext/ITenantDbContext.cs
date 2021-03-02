namespace ImagineCommunications.GamePlan.Domain.Generic.DbContext
{
    public interface ITenantDbContext : IDbContext
    {
    }

    public interface ITenantDbContext<out TSpecificDbAdapter> : ITenantDbContext, IDbContext<TSpecificDbAdapter>
        where TSpecificDbAdapter : class
    {
    }
}
