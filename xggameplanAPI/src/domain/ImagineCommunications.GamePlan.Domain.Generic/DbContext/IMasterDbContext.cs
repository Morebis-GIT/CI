namespace ImagineCommunications.GamePlan.Domain.Generic.DbContext
{
    public interface IMasterDbContext : IDbContext
    {
    }

    public interface IMasterDbContext<out TSpecificDbAdapter> : IMasterDbContext, IDbContext<TSpecificDbAdapter>
        where TSpecificDbAdapter : class
    {
    }
}
