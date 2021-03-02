using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext
{
    public class RavenDbContext : IRavenDbContext
    {
        private static readonly MethodInfo RemoveMethodInfo = typeof(IDbContext)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x => x.Name == nameof(IDbContext.Remove) && x.IsGenericMethod)
            ?.GetGenericMethodDefinition();

        private static readonly MethodInfo RemoveRangeMethodInfo = typeof(IDbContext)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x => x.Name == nameof(IDbContext.RemoveRange) && x.IsGenericMethod)
            ?.GetGenericMethodDefinition();

        private static readonly MethodInfo FindMethodInfo = typeof(IDbContext)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x => x.Name == nameof(IDbContext.Find) && x.IsGenericMethod)
            ?.GetGenericMethodDefinition();

        private static readonly MethodInfo FindAsyncMethodInfo = typeof(IDbContext)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x => x.Name == nameof(IDbContext.FindAsync) && x.IsGenericMethod)
            ?.GetGenericMethodDefinition();

        private static readonly MethodInfo TruncateGenericMethodInfo =
            typeof(IDbContext).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x =>
                x.Name == nameof(IDbContext.Truncate) && x.IsGenericMethod)?.GetGenericMethodDefinition();

        private static readonly MethodInfo TruncateAsyncGenericMethodInfo =
            typeof(IDbContext).GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(x =>
                x.Name == nameof(IDbContext.TruncateAsync) && x.IsGenericMethod)?.GetGenericMethodDefinition();

        private readonly IDocumentSession _documentSession;
        private readonly IAsyncDocumentSession _asyncDocumentSession;
        private readonly RavenDocumentsByEntityName _documentsByEntityNameIndex = new RavenDocumentsByEntityName();

        private RavenSpecificDbAdapter _specificDbAdapter;
        private IRavenDbBulkInsertEngine _bulkInsertEngine;
        private bool _disposed;

        protected virtual void BeginBulkInsert(Action<BulkInsertOperation> bulkInsertAction)
        {
            if (bulkInsertAction == null)
            {
                return;
            }

            var options = new Raven.Abstractions.Data.BulkInsertOptions() { OverwriteExisting = true };
            using (var bulkInsert = _documentSession.Advanced.DocumentStore.BulkInsert(null, options))
            {
                bulkInsertAction(bulkInsert);

                if (Specific.BulkInsertOptions.IsWaitForLastTaskToFinish)
                {
                    bulkInsert.WaitForLastTaskToFinish().Wait();
                }
            }
        }

        protected virtual IRavenDbBulkInsertEngine CreateBulkInsertEngine()
        {
            return new RavenDbBulkInsertEngine(_documentSession, _asyncDocumentSession);
        }

        public RavenDbContext(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
            _asyncDocumentSession =
                asyncDocumentSession ?? throw new ArgumentNullException(nameof(asyncDocumentSession));
        }

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            _documentSession.Store(entity);
            return entity;
        }

        public object Add(object entity)
        {
            _documentSession.Store(entity);
            return entity;
        }

        public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _asyncDocumentSession.StoreAsync(entity).ConfigureAwait(false);
            return entity;
        }

        public async Task<object> AddAsync(object entity)
        {
            await _asyncDocumentSession.StoreAsync(entity).ConfigureAwait(false);
            return entity;
        }

        public void AddRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            BeginBulkInsert(bulkInsert => entities.ForEach(item => bulkInsert.Store(item)));
        }

        public void AddRange(params object[] entities)
        {
            AddRange(entities.AsEnumerable());
        }

        public void AddRange(IEnumerable<object> entities)
        {
            BeginBulkInsert(bulkInsert => entities.ForEach(item => bulkInsert.Store(item)));
        }

        public Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class
        {
            return Task.Run(() => ((IDbContext)this).AddRange(entities));
        }

        public Task AddRangeAsync(params object[] entities)
        {
            return Task.Run(() => ((IDbContext)this).AddRange(entities));
        }

        public Task AddRangeAsync(IEnumerable<object> entities)
        {
            return Task.Run(() => ((IDbContext)this).AddRange(entities));
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _documentSession.Delete(entity);
        }

        public void Remove(object entity)
        {
            _ = RemoveMethodInfo.MakeGenericMethod((entity ?? throw new ArgumentNullException(nameof(entity))).GetType())
                .Invoke(this, new[] { entity });
        }

        public void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            entities.ForEach(e => _documentSession.Delete(e));
        }

        public void RemoveRange(params object[] entities)
        {
            RemoveRange(entities.AsEnumerable());
        }

        public void RemoveRange(IEnumerable<object> entities)
        {
            foreach (var group in entities.Where(e => e != null).GroupBy(k => k.GetType()))
            {
                _ = RemoveRangeMethodInfo.MakeGenericMethod(group.Key).Invoke(this,
                    new object[] { EnumerableCastHelper.CastToArray(group.AsEnumerable(), group.Key) });
            }
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            _documentSession.Store(entity);
            return entity;
        }

        public object Update(object entity)
        {
            _documentSession.Store(entity);
            return entity;
        }

        public void UpdateRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            foreach (var entity in entities ?? throw new ArgumentNullException(nameof(entities)))
            {
                _documentSession.Store(entity);
            }
        }

        public void UpdateRange(params object[] entities)
        {
            UpdateRange(entities.AsEnumerable());
        }

        public void UpdateRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities ?? throw new ArgumentNullException(nameof(entities)))
            {
                _documentSession.Store(entity);
            }
        }

        public TEntity Find<TEntity>(params object[] ids) where TEntity : class
        {
            if (ids == null)
            {
                return null;
            }

            return (ids.All(x => x is ValueType)
                ? _documentSession.Load<TEntity>(ids.Cast<ValueType>())
                : _documentSession.Load<TEntity>(ids.Select(x => x.ToString())))?.FirstOrDefault();
        }

        public object Find(Type entityType, params object[] ids)
        {
            return FindMethodInfo.MakeGenericMethod(entityType).Invoke(this, new object[] { ids });
        }

        public async Task<TEntity> FindAsync<TEntity>(params object[] ids) where TEntity : class
        {
            if (ids == null)
            {
                return null;
            }

            return (await (ids.All(x => x is ValueType)
                    ? _asyncDocumentSession.LoadAsync<TEntity>(ids.Cast<ValueType>())
                    : _asyncDocumentSession.LoadAsync<TEntity>(ids.Select(x => x.ToString()))).ConfigureAwait(false))
                .FirstOrDefault();
        }

        public async Task<object> FindAsync(Type entityType, params object[] ids)
        {
            return await TaskReflectionHelper
                .WaitForTaskAsync((Task)FindAsyncMethodInfo.MakeGenericMethod(entityType)
                    .Invoke(this, new object[] { ids })).ConfigureAwait(false);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return _documentSession.Query<TEntity>();
        }

        public void Truncate<TEntity>() where TEntity : class
        {
            var tagName = _documentSession.Advanced.DocumentStore.Conventions.FindTypeTagName(typeof(TEntity));
            _ = _documentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(_documentsByEntityNameIndex.IndexName,
                new IndexQuery { Query = $"Tag:{tagName}" });
        }

        public void Truncate(Type entityType)
        {
            _ = TruncateGenericMethodInfo.MakeGenericMethod(entityType).Invoke(this, Array.Empty<object>());
        }

        public async Task TruncateAsync<TEntity>() where TEntity : class
        {
            var tagName = _documentSession.Advanced.DocumentStore.Conventions.FindTypeTagName(typeof(TEntity));
            _ = await _documentSession.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync(_documentsByEntityNameIndex.IndexName,
                new IndexQuery { Query = $"Tag:{tagName}" }).ConfigureAwait(false);
        }

        public async Task TruncateAsync(Type entityType)
        {
            await ((Task)TruncateAsyncGenericMethodInfo.MakeGenericMethod(entityType)
                .Invoke(this, Array.Empty<object>())).ConfigureAwait(false);
        }

        public void SaveChanges()
        {
            _documentSession.SaveChanges();
            _asyncDocumentSession.SaveChangesAsync().Wait();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            _documentSession.SaveChanges();
            await _asyncDocumentSession.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public IRavenDbBulkInsertEngine BulkInsertEngine =>
            _bulkInsertEngine ?? (_bulkInsertEngine = CreateBulkInsertEngine());

        public RavenSpecificDbAdapter Specific =>
            _specificDbAdapter ?? (_specificDbAdapter = new RavenSpecificDbAdapter(_documentSession, _asyncDocumentSession));

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    _documentSession?.Dispose();
                    _asyncDocumentSession?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
