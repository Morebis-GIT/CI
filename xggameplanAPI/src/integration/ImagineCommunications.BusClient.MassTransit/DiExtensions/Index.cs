using System;

namespace ImagineCommunications.Extensions.DependencyInjection
{
    internal class Index<T> : IIndex<T> where T : class
    {
        private readonly Keys<T> _keys;
        private readonly IServiceProvider _provider;

        public Index(Keys<T> keys, IServiceProvider provider)
        {
            _keys = keys;
            _provider = provider;
        }

        public T Resolve(string key) =>
            _provider.GetService(_keys.GetByKey(key)) as T;
    }
}
