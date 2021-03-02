using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Extensions
{
    public static class NamedDependencyExtension
    {
        private struct ContainerKey
        {
            public string Key { get; set; }
            public Type Type { get; set; }

            public ContainerKey(string key, Type type)
            {
                Key = key;
                Type = type;
            }
        }

        private static readonly IDictionary<ContainerKey, Type> _container = new Dictionary<ContainerKey, Type>();

        public static IServiceCollection AddTransientWithKey<TService, TImplementation>(this IServiceCollection serviceCollection, string key)
            where TService : class
            where TImplementation : class, TService
        {
            var containerKey = new ContainerKey(key, typeof(TService));
            serviceCollection.AddTransient<TService, TImplementation>();
            _container.Add(containerKey, typeof(TImplementation));
            return serviceCollection;
        }

        public static IServiceCollection AddResolver<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            serviceCollection.AddTransient<Func<string, TService>>(
                provider => key =>
                {
                    var containerKey = new ContainerKey(key, typeof(TService));
                    var errorMessage = $"Can't resolve {typeof(TService).FullName} service by key: '{key}'.";

                    if (!_container.TryGetValue(containerKey, out var implementationType))
                    {
                        throw new Exception(errorMessage);
                    }

                    return provider.GetServices<TService>().FirstOrDefault(x => x.GetType() == implementationType) ?? throw new Exception(errorMessage);
                });
            return serviceCollection;
        }
    }
}
