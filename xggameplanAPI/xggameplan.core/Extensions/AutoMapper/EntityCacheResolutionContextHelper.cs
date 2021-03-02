using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace xggameplan.core.Extensions.AutoMapper
{
    public static class EntityCacheResolutionContextHelper
    {
        private const string EntityCacheItemName = "_entityCaches_";

        private static IDictionary<Type, IDictionary<Type, object>> GetRootEntityCaches(IDictionary<string, object> items)
        {
            if (!items.TryGetValue(EntityCacheItemName, out var rootCache))
            {
                rootCache = new Dictionary<Type, IDictionary<Type, object>>();
                items.Add(EntityCacheItemName, rootCache);
            }

            return (IDictionary<Type, IDictionary<Type, object>>)rootCache;
        }

        private static IDictionary<Type, object> GetEntityKeysDictionary<TEntity>(IDictionary<string, object> items)
            where TEntity : class
        {
            var cache = GetRootEntityCaches(items);
            if (!cache.TryGetValue(typeof(TEntity), out var entityKeys))
            {
                entityKeys = new Dictionary<Type, object>();
                cache.Add(typeof(TEntity), entityKeys);
            }

            return entityKeys;
        }

        public static void Add<TKey, TEntity>(IDictionary<string, object> items,
            IEntityCacheAccessor<TKey, TEntity> cacheAccessor)
            where TEntity : class
        {
            var entityKeys = GetEntityKeysDictionary<TEntity>(items);
            if (entityKeys.ContainsKey(typeof(TKey)))
            {
                throw new InvalidOperationException(
                    $"'{nameof(IEntityCacheAccessor<TKey, TEntity>)}' entity cache has already been added to mapping options.");
            }

            entityKeys.Add(typeof(TKey), cacheAccessor);
        }

        public static IEntityCacheAccessor<TKey, TEntity> Get<TKey, TEntity>(IDictionary<string, object> items)
            where TEntity : class
        {
            var entityKeys = GetEntityKeysDictionary<TEntity>(items);
            if (!entityKeys.TryGetValue(typeof(TKey), out var cache))
            {
                throw new InvalidOperationException(
                    $"'{nameof(IEntityCacheAccessor<TKey, TEntity>)}' entity cache has not been added to resolution context.");
            }

            return (IEntityCacheAccessor<TKey, TEntity>)cache;
        }
    }
}
