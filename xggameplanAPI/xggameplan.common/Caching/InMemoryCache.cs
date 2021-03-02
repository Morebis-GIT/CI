using System;
using System.Runtime.Caching;

namespace xggameplan.common.Caching
{
    // TODO: Use either this in-memory cache OR the Memory Repository
    // WARNING: this cache implementation is not thread safe
    public class InMemoryCache : ICache
    {
        /// <summary>
        /// empty constructor for reflection
        /// </summary>
        public InMemoryCache()
        { }

        private readonly MemoryCache _cache = MemoryCache.Default;

        public T Get<T>(string key, Func<T> valueFactory = null)
            => Get(key, valueFactory, TimeSpan.FromMinutes(10));

        public T Get<T>(string key, Func<T> valueFactory, TimeSpan expiration)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string internalKey = GetInternalKey(key);

            if (_cache.Contains(internalKey))
            {
                return (T)_cache[internalKey];
            }

            if (valueFactory != null)
            {
                var value = valueFactory();
                AddToCache(internalKey, value, expiration);

                return value;
            }

            return default;
        }

        public void Add<T>(string key, T value, TimeSpan expiration)
        {
            string internalKey = GetInternalKey(key);

            if (_cache.Contains(internalKey))
            {
                _ = _cache.Remove(internalKey);
            }

            AddToCache(internalKey, value, expiration);
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string internalKey = GetInternalKey(key);
            if (_cache.Contains(internalKey))
            {
                _ = _cache.Remove(internalKey);
            }
        }

        private static string GetInternalKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return $"$IMC|{key}";
        }

        private void AddToCache(string key, object value, TimeSpan duration) =>
            _cache.Add(new CacheItem(key, value),
                new CacheItemPolicy()
                {
                    Priority = CacheItemPriority.Default,
                    AbsoluteExpiration = duration.TotalMilliseconds == 0
                                ? DateTimeOffset.MaxValue
                                : DateTimeOffset.Now.AddMilliseconds(duration.TotalMilliseconds)
                });
    }
}
