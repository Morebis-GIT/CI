using System;

namespace xggameplan.common.Caching
{
    public interface ICache
    {
        T Get<T>(string key, Func<T> valueFactory = null);

        T Get<T>(string key, Func<T> valueFactory, TimeSpan expiration);

        void Add<T>(string key, T value, TimeSpan duration);

        void Remove(string key);
    }
}
