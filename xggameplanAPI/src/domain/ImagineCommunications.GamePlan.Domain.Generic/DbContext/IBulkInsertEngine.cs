using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Domain.Generic.DbContext
{
    public interface IBulkInsertEngine<in TOptions> where TOptions : class
    {
        void BulkInsert<TEntity>(IList<TEntity> entities, TOptions options = null) where TEntity : class;
        Task BulkInsertAsync<TEntity>(IList<TEntity> entities, TOptions options = null, CancellationToken cancellationToken = default) where TEntity : class;
        void BulkInsertOrUpdate<TEntity>(IList<TEntity> entities, TOptions options = null) where TEntity : class;
        Task BulkInsertOrUpdateAsync<TEntity>(IList<TEntity> entities, TOptions options = null, CancellationToken cancellationToken = default) where TEntity : class;
    }
}
