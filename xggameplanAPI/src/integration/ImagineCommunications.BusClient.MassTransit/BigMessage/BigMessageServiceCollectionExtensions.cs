using Microsoft.Extensions.DependencyInjection;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Implementation.Services;

namespace ImagineCommunications.BusClient.Implementation.BigMessage
{
    /// <summary>
    /// Extension methods for adding big message services.
    /// </summary>
    /// <seealso cref="IBigMessage"/>
    /// <seealso cref="IBigMessageService"/>
    /// <seealso cref="IObjectStorage"/>
    public static class BigMessageServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for using big messages.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The AWS object storage configuration.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddBigMessages(this IServiceCollection services, ObjectStorageConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddScoped<IObjectStorage, AwsObjectStorage>();
            services.AddScoped<IObjectSerializer, JsonObjectSerializer>();
            services.AddScoped<IBigMessageService, BigMessageService>();

            return services;
        }
    }
}
