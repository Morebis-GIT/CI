using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    /// <summary>
    /// Base class for memory repository.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class MemoryRepositoryBase<T1> :
        IRepositoryObjectTTL,
        IMemoryRepositoryCompartment
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
        private string _compartmentKeyPrefix = String.Empty;

        private static readonly double _defaultTTL = TimeSpan.FromDays(1).TotalMilliseconds;

        /// <summary>
        /// An optional salting key used to help generate unique instance keys.
        /// </summary>
        public string CompartmentKey { get; set; } = String.Empty;

        /// <inheritdoc/>
        public double ObjectTTLMilliseconds { private get; set; }

        /// <inheritdoc/>
        public void ResetObjectTTLMilliseconds() =>
            ObjectTTLMilliseconds = _defaultTTL;

        private DateTimeOffset AbsoluteObjectTTL =>
            ObjectTTLMilliseconds.CompareTo(0) <= 0
                ? DateTimeOffset.MaxValue
                : DateTimeOffset.Now.AddMilliseconds(ObjectTTLMilliseconds);

        protected MemoryRepositoryBase()
        {
            ResetObjectTTLMilliseconds();
        }

        private string CompartmentKeyPrefix
        {
            get
            {
                if (_compartmentKeyPrefix.Length == 0)
                {
                    Type type = typeof(T1);

                    _compartmentKeyPrefix = String.IsNullOrWhiteSpace(CompartmentKey)
                        ? $"$IMC|{type}|"
                        : $"$IMC|{CompartmentKey}|{type}|";
                }

                return _compartmentKeyPrefix;
            }

            set => _compartmentKeyPrefix = value;
        }

        /// <summary>
        /// Returns internal key, unique for item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetInternalKey(string id) =>
            CompartmentKeyPrefix + (id ?? throw new ArgumentNullException(nameof(id)));

        private bool IsKeyForCompartment(string key) =>
            key.StartsWith(CompartmentKeyPrefix, StringComparison.Ordinal);

        private void RemoveFromCacheByKey(string key)
        {
            var obj = _cache.Remove(key);
            if (obj is IDisposable dobj)
            {
                dobj.Dispose();
            }
        }

        protected void InsertOrReplaceItem(
            T1 item,
            string id,
            DateTimeOffset? absoluteExpiration = null)
        {
            string key = GetInternalKey(id);
            RemoveFromCacheByKey(key);

            _ = _cache.Add(key, item,
                new CacheItemPolicy()
                {
                    Priority = CacheItemPriority.Default,
                    AbsoluteExpiration = absoluteExpiration ?? AbsoluteObjectTTL
                }
            );
        }

        protected T1 GetItemById(string id)
        {
            string key = GetInternalKey(id);

            return _cache.Contains(key)
                ? (T1)_cache[key]
                : default;
        }

        protected void DeleteItem(string id)
        {
            RemoveFromCacheByKey(GetInternalKey(id));
        }

        protected void DeleteAllItems()
        {
            List<string> keys = _cache
                .Select(i => i.Key)
                .Where(key => IsKeyForCompartment(key) && _cache[key] is T1)
                .ToList();

            int idx = keys.Count;
            while (idx > 0)
            {
                RemoveFromCacheByKey(keys[--idx]);
            }
        }

        protected void DeleteAllItems(Expression<Func<T1, bool>> filter)
        {
            var keys = new List<string>();
            var itemsToFilter = new List<T1>(1);

            foreach (string itemKey in _cache
                .Select(i => i.Key)
                .Where(key => IsKeyForCompartment(key) && _cache[key] is T1))
            {
                itemsToFilter.Add((T1)_cache[itemKey]);

                var result = itemsToFilter
                    .AsQueryable<T1>()
                    .Any(filter);

                if (result)
                {
                    keys.Add(itemKey);
                }

                itemsToFilter.Clear();
            }

            int idx = keys.Count;
            while (idx > 0)
            {
                RemoveFromCacheByKey(keys[--idx]);
            }
        }

        protected List<T1> GetAllItems()
        {
            var cacheItems = _cache
                .Select(i => i.Key)
                .Where(key => IsKeyForCompartment(key) && _cache[key] is T1)
                .Select(key => (T1)_cache[key]);

            return new List<T1>(cacheItems);
        }

        protected List<T1> GetAllItems(Expression<Func<T1, bool>> filter)
        {
            var items = new List<T1>();
            var itemsToFilter = new List<T1>(1);

            foreach (string itemKey in _cache
                .Select(i => i.Key)
                .Where(key => IsKeyForCompartment(key) && _cache[key] is T1))
            {
                var itemToAdd = (T1)_cache[itemKey];
                itemsToFilter.Add(itemToAdd);

                var result = itemsToFilter
                    .AsQueryable<T1>()
                    .Any(filter);

                if (result)
                {
                    items.Add(itemToAdd);
                }

                itemsToFilter.Clear();
            }

            return items;
        }

        protected int GetCount() =>
            _cache
                .Where(i => IsKeyForCompartment(i.Key))
                .Count(i => _cache[i.Key].GetType() is T1);

        protected int GetCount(Expression<Func<T1, bool>> filter)
        {
            int count = 0;
            var itemsToFilter = new List<T1>(1);

            foreach (T1 cacheItem in _cache
                .Select(i => i.Key)
                .Where(key => IsKeyForCompartment(key) && _cache[key] is T1)
                .Select(key => (T1)_cache[key]))
            {
                itemsToFilter.Add(cacheItem);

                var result = itemsToFilter
                    .AsQueryable<T1>()
                    .Any(filter);

                if (result)
                {
                    count++;
                }

                itemsToFilter.Clear();
            }

            return count;
        }
    }
}
