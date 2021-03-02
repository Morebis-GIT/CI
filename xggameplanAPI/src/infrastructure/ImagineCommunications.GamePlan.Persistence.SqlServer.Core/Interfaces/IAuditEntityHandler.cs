namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface IAuditEntityHandler
    {
        void AddAuditInfo<TEntity>(params TEntity[] entities) where TEntity : class, IAuditEntity;
    }
}
