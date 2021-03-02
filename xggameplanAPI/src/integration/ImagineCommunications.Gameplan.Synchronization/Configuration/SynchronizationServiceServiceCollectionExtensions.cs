using System;
using Microsoft.Extensions.DependencyInjection;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;

namespace ImagineCommunications.Gameplan.Synchronization
{
    public static class SynchronizationServiceServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for using payload reference.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configurator">The configurator.</param>
        public static IServiceCollection AddSynchronizationService(this IServiceCollection services, Action<SynchronizationServiceConfigurator> configurator = null)
        {
            services.AddScoped<ISynchronizationService, SynchronizationService>();

            var collectionConfigurator = new SynchronizationServiceConfigurator(services);
            configurator?.Invoke(collectionConfigurator);

            return services;
        }
    }
}
