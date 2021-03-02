using System;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using Microsoft.EntityFrameworkCore;

namespace xggameplan.specification.tests.Infrastructure.SqlServer
{
    public class SqlServerTestSpecificDbAdapter : SqlServerSpecificDbAdapter
    {
        private readonly DbContext _dbContext;

        public SqlServerTestSpecificDbAdapter(DbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

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
    }
}
