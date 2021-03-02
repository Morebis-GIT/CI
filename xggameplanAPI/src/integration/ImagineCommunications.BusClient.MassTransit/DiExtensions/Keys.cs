using System;
using System.Collections.Generic;

namespace ImagineCommunications.Extensions.DependencyInjection
{
    internal class Keys<T>
    {
        private readonly Dictionary<string, Type> _keys = new Dictionary<string, Type>();

        public Keys() { }

        public void AddServiceKey<S>(string key) where S : T
            => _keys[key] = typeof(S);

        public Type GetByKey(string key) => _keys[key];
    }
}
