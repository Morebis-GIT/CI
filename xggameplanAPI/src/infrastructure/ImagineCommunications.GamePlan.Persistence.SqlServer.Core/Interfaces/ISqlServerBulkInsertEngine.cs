using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ISqlServerBulkInsertEngine : IBulkInsertEngine<BulkInsertOptions>
    {
        void BulkUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class;

        Task BulkUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default) where TEntity : class;

        void BulkDelete<TEntity>(IList<TEntity> entities) where TEntity : class;
    }
}
