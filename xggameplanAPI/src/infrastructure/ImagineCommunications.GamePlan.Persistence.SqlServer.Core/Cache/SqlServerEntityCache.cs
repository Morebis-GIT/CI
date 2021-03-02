using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.common.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Cache
{
    public class SqlServerEntityCache<TKey, TEntity> : IEntityCache<TKey, TEntity>
        where TEntity : class
    {
        private static readonly MethodInfo _containsMethodInfo = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
            ?.GetGenericMethodDefinition().MakeGenericMethod(typeof(TKey));

        private readonly Lazy<IDictionary<TKey, TEntity>> _storage;
        private readonly HashSet<TKey> _addedKeys = new HashSet<TKey>();

        public SqlServerEntityCache(ISqlServerDbContext dbContext,
            Expression<Func<TEntity, TKey>> keyPropertyExpression, bool preloadData = true, bool trackingChanges = true)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            if (!keyPropertyExpression.IsParameterMemberExpression(MemberTypes.Property))
            {
                throw new ArgumentException("Expression must be the property of the entity.",
                    nameof(keyPropertyExpression));
            }

            KeyMemberInfo = keyPropertyExpression.GetMemberInfo();
            KeyGetter = keyPropertyExpression.Compile();
            TrackingChanges = trackingChanges;
            IsDataLoaded = preloadData;

            _storage = new Lazy<IDictionary<TKey, TEntity>>(InitializeStorage,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public TEntity GetOrAdd(TKey key, Func<TKey, TEntity> factory = null)
        {
            var entity = InternalGetOrAdd(key, factory, out var isAdded);

            if (isAdded)
            {
                _ = _addedKeys.Add(AdjustKeyValue(key));

                if (TrackingChanges)
                {
                    AddEntityToDbContext(entity);
                }
            }

            return entity;
        }

        public TEntity Get(TKey key) => InternalGetOrAdd(key, null, out _);

        public void Add(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var key = KeyGetter(entity);
            _ = InternalGetOrAdd(key, k => entity, out var isAdded);

            if (!isAdded)
            {
                throw new InvalidOperationException(
                    $"Entity of '{typeof(TEntity).Name}' type with '{key}' key value already exists in the cache.");
            }

            _ = _addedKeys.Add(AdjustKeyValue(key));

            if (TrackingChanges)
            {
                AddEntityToDbContext(entity);
            }
        }

        public bool Remove(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var key = AdjustKeyValue(KeyGetter(entity));
            _ = _addedKeys.Remove(key);
            var res = Storage.Remove(key);

            if (res && TrackingChanges)
            {
                RemoveEntityFromDbContext(entity);
            }

            return res;
        }

        public void Load()
        {
            if (IsDataLoaded)
            {
                if (!_storage.IsValueCreated)
                {
                    _ = Storage;
                }

                return;
            }

            var entities = GetEntities();
            foreach (var entity in entities)
            {
                var key = AdjustKeyValue(KeyGetter(entity));
                if (_addedKeys.Contains(key))
                {
                    throw new InvalidOperationException(
                        $"Cache of '{typeof(TEntity).Name}' entities already contains '{key}' key.");
                }

                if (!Storage.ContainsKey(key))
                {
                    Storage[key] = entity;
                }
            }

            IsDataLoaded = true;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TEntity> GetEnumerator() => Storage.Values.GetEnumerator();

        protected virtual TKey AdjustKeyValue(TKey key)
        {
            return key;
        }

        protected virtual TEntity InternalGetOrAdd(TKey key, Func<TKey, TEntity> factory, out bool isAdded)
        {
            isAdded = false;

            if (Storage.TryGetValue(AdjustKeyValue(key), out var entity))
            {
                return entity;
            }

            if (!IsDataLoaded)
            {
                entity = GetEntityByKey(key);
                if (!(entity is null))
                {
                    Storage.Add(AdjustKeyValue(KeyGetter(entity)), entity);
                    return entity;
                }
            }

            if (!(factory is null))
            {
                entity = factory(key);
                Storage.Add(AdjustKeyValue(KeyGetter(entity)), entity);
                isAdded = true;
            }

            return entity;
        }

        protected virtual IDictionary<TKey, TEntity> CreateInternalStorage(IEnumerable<TEntity> entities)
        {
            return entities?.ToDictionary(entity => AdjustKeyValue(KeyGetter(entity))) ?? new Dictionary<TKey, TEntity>();
        }

        protected virtual TEntity GetEntityByKey(TKey key)
        {
            var query = DbContext.Query<TEntity>().Where(WhereByKeyPredicate());

            if (!TrackingChanges)
            {
                query = query.AsNoTracking();
            }

            query = ExtendEntityQuery(query);

            return query.FirstOrDefault();

            Expression<Func<TEntity, bool>> WhereByKeyPredicate()
            {
                var p = Expression.Parameter(typeof(TEntity), "x");

                return Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Equal(Expression.MakeMemberAccess(p, KeyMemberInfo), Expression.Constant(key)), p);
            }
        }

        protected virtual IEnumerable<TEntity> GetEntities()
        {
            var query = DbContext.Query<TEntity>();

            if (!TrackingChanges)
            {
                query = query.AsNoTracking();
            }

            query = ExtendEntityQuery(query);

            return query.AsEnumerable();
        }

        protected virtual IQueryable<TEntity> ExtendEntityQuery(IQueryable<TEntity> entityQuery)
        {
            return entityQuery;
        }

        protected virtual IDictionary<TKey, TEntity> InitializeStorage()
        {
            return CreateInternalStorage(IsDataLoaded ? GetEntities() : null);
        }

        protected void AddEntityToDbContext(TEntity entity)
        {
            _ = DbContext.Add(entity);
        }

        protected void RemoveEntityFromDbContext(TEntity entity)
        {
            DbContext.Remove(entity);
        }

        protected virtual Func<TEntity, TKey> KeyGetter { get; }

        protected ISqlServerDbContext DbContext { get; }

        protected MemberInfo KeyMemberInfo { get; }

        protected IDictionary<TKey, TEntity> Storage => _storage.Value;

        protected bool IsDataLoaded { get; private set; }

        protected bool TrackingChanges { get; }
    }
}
