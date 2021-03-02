using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerTestBulkInsertEngine : ISqlServerBulkInsertEngine
    {
        private readonly ISqlServerTestDbContext _dbContext;

        public SqlServerTestBulkInsertEngine(SqlServerTestDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void BulkInsert<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            _dbContext.AddRange(entities.ToArray());
        }

        public Task BulkInsertAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default) 
            where TEntity : class
        {
            return _dbContext.AddRangeAsync(entities.ToArray());
        }

        public void BulkInsertOrUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            _dbContext.AddRange(entities.ToArray());
        }

        public Task BulkInsertOrUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            return _dbContext.AddRangeAsync(entities.ToArray());
        }

        public void BulkUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            _dbContext.UpdateRange(entities.ToArray());
        }

        public Task BulkUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            _dbContext.UpdateRange(entities.ToArray());
            return Task.CompletedTask;
        }

        public void BulkDelete<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            _dbContext.RemoveRange(entities);
        }
    }
}
