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
        /// Returns internal key, unique for item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string GetInternalKey(string id) =>
            CompartmentKeyPrefix + (id ?? throw new ArgumentNullException(nameof(id)));

        private bool KeyIsForCompartment(string key) =>
            key.StartsWith(CompartmentKeyPrefix);

        protected void UpdateOrInsertItem(
            T1 item,
            string id,
            DateTimeOffset? absoluteExpiration = null)
        {
            string key = GetInternalKey(id);
            _ = _cache.Remove(key);

            _ = _cache.Add(
                new CacheItem(key, item),
                new CacheItemPolicy()
                {
                    Priority = CacheItemPriority.Default,
                    AbsoluteExpiration = absoluteExpiration ?? AbsoluteObjectTTL
                });
        }

        protected void InsertItems(IList<T1> items, IList<string> ids)
        {
            if (items.Count == 0 || ids.Count == 0)
            {
                return;
            }

            var absoluteExpirationValue = AbsoluteObjectTTL;

            for (int index = 0; index < items.Count; index++)
            {
                UpdateOrInsertItem(items[index], ids[index], absoluteExpirationValue);
            }
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
            string key = GetInternalKey(id);
            _ = _cache.Remove(key);
        }

        protected void DeleteAllItems()
        {
            var keys = new List<string>();
            Type type = typeof(T1);

            foreach (var item in _cache.Where(i => KeyIsForCompartment(i.Key)))
            {
                if (_cache[item.Key].GetType() == type)
                {
                    keys.Add(item.Key);
                }
            }

            while (keys.Count > 0)
            {
                string key = keys[0];
                keys.RemoveAt(0);
                _ = _cache.Remove(key);
            }
        }

        protected void DeleteAllItems(Expression<Func<T1, bool>> filter)
        {
            var keys = new List<string>();
            Type type = typeof(T1);

            foreach (var item in _cache.Where(i => KeyIsForCompartment(i.Key)))
            {
                if (_cache[item.Key].GetType() != type)
                {
                    continue;
                }

                var itemsToFilter = new[] { (T1)_cache[item.Key] };

                var results = itemsToFilter
                    .AsQueryable<T1>()
                    .Where(filter);

                if (results.Any())
                {
                    keys.Add(item.Key);
                }
            }

            while (keys.Count > 0)
            {
                string key = keys[0];
                keys.RemoveAt(0);
                _ = _cache.Remove(key);
            }
        }

        protected List<T1> GetAllItems()
        {
            var items = new List<T1>();
            Type type = typeof(T1);

            foreach (var item in _cache.Where(i => KeyIsForCompartment(i.Key)))
            {
                if (_cache[item.Key].GetType() == type)
                {
                    items.Add((T1)_cache[item.Key]);
                }
            }

            return items;
        }

        protected List<T1> GetAllItems(Expression<Func<T1, bool>> filter)
        {
            var items = new List<T1>();
            Type type = typeof(T1);

            foreach (var item in _cache.Where(i => KeyIsForCompartment(i.Key)))
            {
                if (_cache[item.Key].GetType() != type)
                {
                    continue;
                }

                var itemsToFilter = new[] { (T1)_cache[item.Key] };

                var results = itemsToFilter
                    .AsQueryable<T1>()
                    .Where(filter);

                items.AddRange(results);
            }

            return items;
        }

        protected int GetCount()
        {
            int count = 0;
            Type type = typeof(T1);

            foreach (var item in _cache.Where(i => KeyIsForCompartment(i.Key)))
            {
                if (_cache[item.Key].GetType() == type)
                {
                    count++;
                }
            }

            return count;
        }

        protected int GetCount(Expression<Func<T1, bool>> filter)
        {
            int count = 0;
            Type type = typeof(T1);

            foreach (var item in _cache.Where(i => KeyIsForCompartment(i.Key)))
            {
                if (_cache[item.Key].GetType() != type)
                {
                    continue;
                }

                var itemsToFilter = new[] { (T1)_cache[item.Key] };

                var results = itemsToFilter
                    .AsQueryable<T1>()
                    .Where(filter);

                if (results.Any())
                {
                    count++;
                }
            }

            return count;
        }
    }
}
