using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
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
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _dbContext.AddRange(entities.ToArray());
        }

        public Task BulkInsertAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            return _dbContext.AddRangeAsync(entities.ToArray());
        }

        public void BulkInsertOrUpdate<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null) where TEntity : class
        {
            List<TEntity> listToAdd = null;
            IEnumerable<TEntity> toUpdate = null;
            if (typeof(IIdentityPrimaryKey).IsAssignableFrom(typeof(TEntity)))
            {
                listToAdd = entities.Where(t => ((IIdentityPrimaryKey)t).Id <= 0).ToList();
                toUpdate = entities.Except(listToAdd);
                foreach (var ent in toUpdate)
                {
                    var obj = _dbContext.Query<TEntity>()
                        .FirstOrDefault(t => ((IIdentityPrimaryKey)t).Id == ((IIdentityPrimaryKey)ent).Id);
                    if (obj == null)
                    {
                        listToAdd.Add(ent);
                    }
                    else
                    {
                        PropertyCopier<TEntity, TEntity>.Copy(ent, obj);
                    }
                }
            }
            if (typeof(IUniqueIdentifierPrimaryKey).IsAssignableFrom(typeof(TEntity)))
            {
                listToAdd = entities.Where(t => ((IUniqueIdentifierPrimaryKey)t).Id == null || ((IUniqueIdentifierPrimaryKey)t).Id == Guid.Empty).ToList();
                toUpdate = entities.Except(listToAdd);
                foreach (var ent in toUpdate)
                {
                    var obj = _dbContext.Query<TEntity>().FirstOrDefault(t =>
                        ((IUniqueIdentifierPrimaryKey)t).Id == ((IUniqueIdentifierPrimaryKey)ent).Id);
                    if (obj == null)
                    {
                        listToAdd.Add(ent);
                    }
                    else
                    {
                        PropertyCopier<TEntity, TEntity>.Copy(ent, obj);
                    }
                }
            }
            _dbContext.AddRange(listToAdd.ToArray());
        }

        public Task BulkInsertOrUpdateAsync<TEntity>(IList<TEntity> entities, BulkInsertOptions options = null, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
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
            _dbContext.RemoveRange(entities.ToArray());
        }
    }
}
