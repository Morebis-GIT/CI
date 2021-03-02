using System;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer
{
    public class SqlServerTestSpecificDbAdapter : SqlServerSpecificDbAdapter
    {
        private readonly DbContext _dbContext;

        protected override ITruncateHandler CreateTruncateHandler() =>
            new SqlServerTestTruncateHandler(_dbContext);

        protected override int RemoveBySinglePrimaryKeyIds<TEntity, TKey>(TKey[] ids,
            Func<TKey, string> formatKeyValueFunc)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            if (_dbContext.Database.IsInMemory())
            {
                _dbContext.RemoveRange(_dbContext.Set<TEntity>().AsEnumerable().Where(x => ids.Contains(x.Id)));
                return ids.Length;
            }
            return base.RemoveBySinglePrimaryKeyIds<TEntity, TKey>(ids, formatKeyValueFunc);
        }

        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));

            if (!(entityType is null))
            {
                var key = entityType.FindPrimaryKey();
                if (!(key is null))
                {
                    var entityEntry = _dbContext.Entry(entity);
                    var entityKeyValue = key.Properties.Select(p => entityEntry.CurrentValues[p])
                        .ToArray();

                    var trackedEntity = _dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x =>
                    {
                        var trackedKeyValue = key.Properties.Select(p => x.CurrentValues[p]).ToArray();
                        if (entityKeyValue.Length == trackedKeyValue.Length)
                        {
                            return entityKeyValue.Select((v, idx) => Equals(v, trackedKeyValue[idx])).All(b => b);
                        }

                        return false;
                    });

                    if (!(trackedEntity is null))
                    {
                        return trackedEntity;
                    }
                }
            }

            return base.Attach(entity);
        }

        public override void IdentityInsertOn<TEntity>()
        {
        }

        public override void IdentityInsertOff<TEntity>()
        {
        }

        public SqlServerTestSpecificDbAdapter(DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
    }
}
