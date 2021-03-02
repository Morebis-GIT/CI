using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Truncate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext
{
    public class SqlServerSpecificDbAdapter
    {
        public const string StoredProcAnnotationName = "__stored_proc_name_";

        private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;
        private ITruncateHandler _truncateHandler;

        protected ITruncateHandler TruncateHandler =>
            _truncateHandler ?? (_truncateHandler = CreateTruncateHandler());

        protected virtual ITruncateHandler CreateTruncateHandler() => new TruncateHandler(_dbContext);

        protected virtual int RemoveBySinglePrimaryKeyIds<TEntity, TKey>(TKey[] ids,
            Func<TKey, string> formatKeyValueFunc)
            where TEntity : class, ISinglePrimaryKey<TKey>
        {
            if (formatKeyValueFunc == null)
            {
                throw new ArgumentNullException();
            }

            if (!ids.Any())
            {
                return 0;
            }

            var pkName = _dbContext.Model.GetEntityType<TEntity>().FindPrimaryKey().Properties.Single()
                .Relational()?.ColumnName;

            var query = new RawSqlString(
                $"DELETE FROM {_dbContext.Model.GetFullTableName<TEntity>()} WHERE [{pkName}] IN ({string.Join(",", ids.Select(formatKeyValueFunc))})");

            return _dbContext.Database.ExecuteSqlCommand(query);
        }

        public SqlServerSpecificDbAdapter(Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual DatabaseFacade Database => _dbContext.Database;

        public IModel Model => _dbContext.Model;

        public virtual EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            var internalEntry =
                ((IDbContextDependencies)_dbContext).StateManager.Entries.FirstOrDefault(ee => ee.Entity == entity);
            if (internalEntry != null)
            {
                return new EntityEntry<TEntity>(internalEntry);
            }

            return _dbContext.Entry(entity);
        }

        public virtual EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class =>
            _dbContext.Attach(entity);

        public virtual EntityEntry Attach(object entity) => _dbContext.Attach(entity);

        public virtual void IdentityInsertOn<TEntity>() where TEntity : class
        {
            var tableName = _dbContext.Model.GetFullTableName<TEntity>();
            var insertCommand = new RawSqlString($"SET IDENTITY_INSERT {tableName} ON");
            _ = _dbContext.Database.ExecuteSqlCommand(insertCommand);
        }

        public virtual void IdentityInsertOff<TEntity>() where TEntity : class
        {
            var tableName = _dbContext.Model.GetFullTableName<TEntity>();
            var insertCommand = new RawSqlString($"SET IDENTITY_INSERT {tableName} OFF");
            _ = _dbContext.Database.ExecuteSqlCommand(insertCommand);
        }

        public int RemoveByIdentityIds<TEntity>(params int[] ids) where TEntity : class, IIdentityPrimaryKey =>
            RemoveBySinglePrimaryKeyIds<TEntity, int>((ids ?? Enumerable.Empty<int>()).ToArray(),
                value => value.ToString(CultureInfo.InvariantCulture));

        public int RemoveByUniqueIdentifierIds<TEntity>(params Guid[] ids)
            where TEntity : class, IUniqueIdentifierPrimaryKey =>
            RemoveBySinglePrimaryKeyIds<TEntity, Guid>((ids ?? Enumerable.Empty<Guid>()).ToArray(),
                value => $"'{value}'");

        public int RemoveByStringIdentifierIds<TEntity>(params string[] ids)
            where TEntity : class, IStringIndentifierPrimaryKey =>
            RemoveBySinglePrimaryKeyIds<TEntity, string>((ids ?? Enumerable.Empty<string>()).ToArray(),
                value => $"'{value}'");

        public async Task TruncateOrDeleteAsync<TEntity>(DeleteFromOptions deleteFromOptions = DeleteFromOptions.None,
            CancellationToken cancellationToken = default)
            where TEntity : class =>
            await TruncateHandler.TruncateAsync(Model.GetEntityType<TEntity>(), deleteFromOptions, cancellationToken)
                .ConfigureAwait(false);

        public void TruncateOrDelete<TEntity>(DeleteFromOptions deleteFromOptions = DeleteFromOptions.None)
            where TEntity : class =>
            Task.Run(async () =>
            {
                await TruncateHandler.TruncateAsync(Model.GetEntityType<TEntity>(), deleteFromOptions)
                    .ConfigureAwait(false);
            }).GetAwaiter().GetResult();

        public virtual ChangeTracker ChangeTracker => _dbContext.ChangeTracker;

        public DbQuery<T> View<T>() where T : class => _dbContext.Query<T>();

        public IQueryable<T> StoredProcedure<T>(params SqlParameter[] parameters) where T : class
        {
            var annotation = _dbContext.Model.GetEntityType<T>().FindAnnotation(StoredProcAnnotationName);

            if (!(annotation?.Value is string))
            {
                throw new InvalidOperationException($"'{typeof(T).Name}' is not a stored procedure result type.");
            }

            var spName = annotation.Value.ToString();
            var dbSet = _dbContext.Set<T>().AsNoTracking();

            if (parameters is null || parameters.Length == 0)
            {
                return dbSet.FromSql(new RawSqlString(spName));
            }

            var prms = string.Join(" ", parameters.Select(x => x.ParameterName));

            return dbSet.FromSql(new RawSqlString($"{spName} {prms}"), parameters.Cast<object>().ToArray());
        }
    }
}
