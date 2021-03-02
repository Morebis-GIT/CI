using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.Extensions.DependencyInjection
{
    public static class KeyedServiceRegistrationExtensions
    {
        private static IServiceCollection AddServiceKey<I, T>(this IServiceCollection services, string key)
            where I : class
            where T : class, I
        {
            var descriptor = services.FirstOrDefault(i => i.ImplementationInstance?.GetType() == typeof(Keys<I>));

            Keys<I> keys;

            if (descriptor == null)
            {
                services.AddSingleton(keys = new Keys<I>());
                services.AddScoped<IIndex<I>, Index<I>>();
            }
            else
            {
                keys = descriptor.ImplementationInstance as Keys<I>;
            }

            keys.AddServiceKey<T>(key);

            return services;
        }

        public static IServiceCollection AddSingleton<I, T>(this IServiceCollection services, string key)
            where I : class
            where T : class, I
        {
            services.AddServiceKey<I, T>(key);

            services.AddSingleton<T>();

            return services;
        }

        public static IServiceCollection AddScoped<I, T>(this IServiceCollection services, string key)
            where I : class
            where T : class, I
        {
            services.AddServiceKey<I, T>(key);

            services.AddScoped<T>();

            return services;
        }

        public static IServiceCollection AddTransient<I, T>(this IServiceCollection services, string key)
            where I : class
            where T : class, I
        {
            services.AddServiceKey<I, T>(key);

            services.AddTransient<T>();

            return services;
        }
    }
}
