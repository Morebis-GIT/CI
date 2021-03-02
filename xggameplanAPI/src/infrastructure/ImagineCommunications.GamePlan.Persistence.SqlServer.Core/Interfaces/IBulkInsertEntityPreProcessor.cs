using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface IBulkInsertEntityPreProcessor
    {
        BulkInsertOperation SupportedOperations { get; }

        void Process<TEntity>(TEntity entity, BulkInsertOperation operation, BulkInsertOptions options) where TEntity : class;
    }
}
