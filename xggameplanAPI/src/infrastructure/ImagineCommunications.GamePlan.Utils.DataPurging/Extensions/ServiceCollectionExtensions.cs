using System;
using System.Linq;
using System.Reflection;
using ImagineCommunications.GamePlan.Utils.DataPurging.Attributes;
using ImagineCommunications.GamePlan.Utils.DataPurging.Handlers;
using ImagineCommunications.GamePlan.Utils.DataPurging.Infrastructure;
using ImagineCommunications.GamePlan.Utils.DataPurging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using xggameplan.common.Types;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly MethodInfo _configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == nameof(OptionsConfigurationServiceCollectionExtensions.Configure) &&
                        m.IsGenericMethod)
            .FirstOrDefault(m =>
            {
                var prm = m.GetParameters();

                return prm.Length == 3 && prm[2].ParameterType == typeof(IConfiguration);
            })?.GetGenericMethodDefinition();

        private static void Configure(IServiceCollection serviceCollection, Type type, string name,
            IConfiguration configuration)
        {
            _ = _configureMethod?.MakeGenericMethod(type)
                .Invoke(null, new object[] { serviceCollection, name, configuration });
        }

        /// <summary>
        /// Registers the specified <see cref="THandler"/> data purging handler with all related services.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns></returns>
        public static IServiceCollection AddDataPurgingHandler<THandler>(this IServiceCollection serviceCollection,
            string entityName)
            where THandler : class, IDataPurgingHandler =>
            AddDataPurgingHandler<THandler>(serviceCollection, entityName, Priority.Lowest);

        /// <summary>
        /// Registers the specified <see cref="THandler"/> data purging handler with all related services.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="priority">The priority.</param>
        /// <returns></returns>
        public static IServiceCollection AddDataPurgingHandler<THandler>(this IServiceCollection serviceCollection,
            string entityName, Priority priority)
            where THandler : class, IDataPurgingHandler =>
            AddDataPurgingHandler<THandler>(serviceCollection, entityName, priority, null);

        /// <summary>
        /// Registers the specified <see cref="THandler"/> data purging handler with all related services.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="configuration">
        /// The configuration. If handler inherits <see cref="DataPurgingHandlerBase{TOptions}"/> class
        /// the specified options will be registered using <see cref="PurgingOptionsAttribute"/> information.
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddDataPurgingHandler<THandler>(this IServiceCollection serviceCollection,
            string entityName, IConfiguration configuration)
            where THandler : class, IDataPurgingHandler =>
            AddDataPurgingHandler<THandler>(serviceCollection, entityName, Priority.Lowest, configuration);

        /// <summary>
        /// Registers the specified <see cref="THandler"/> data purging handler with all related services.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="configuration">
        /// The configuration. If handler inherits <see cref="DataPurgingHandlerBase{TOptions}"/> class
        /// the specified options will be registered using <see cref="PurgingOptionsAttribute"/> information.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">serviceCollection</exception>
        /// <exception cref="InvalidOperationException">'{typeof(THandler).Name}' inherits '{typeof(DataPurgingHandlerBase<>).Name}' type but '{nameof(PurgingOptionsAttribute)}' hasn't been defined.</exception>
        public static IServiceCollection AddDataPurgingHandler<THandler>(this IServiceCollection serviceCollection,
            string entityName, Priority priority, IConfiguration configuration)
            where THandler : class, IDataPurgingHandler
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            entityName = string.IsNullOrWhiteSpace(entityName) ? typeof(THandler).Name : entityName;

            _ = serviceCollection
                .AddScoped<THandler>()
                .AddScoped<IDataPurgingHandler>(sp => sp.GetRequiredService<THandler>())
                .AddScoped<IDataPurgingHandlerProvider>(sp =>
                    new DataPurgingHandlerProvider<THandler>(entityName) { Priority = priority });

            if (!(configuration is null))
            {
                var optionType = typeof(THandler).GetBaseTypes().FirstOrDefault(x =>
                        x.IsGenericType && x.GetGenericTypeDefinition() == typeof(DataPurgingHandlerBase<>))
                    ?.GetGenericArguments().FirstOrDefault();
                if (!(optionType is null))
                {
                    var attr = typeof(THandler).GetCustomAttribute<PurgingOptionsAttribute>(true);

                    if (attr is null)
                    {
                        throw new InvalidOperationException(
                            $"'{typeof(THandler).Name}' inherits '{typeof(DataPurgingHandlerBase<>).Name}' type but '{nameof(PurgingOptionsAttribute)}' hasn't been defined.");
                    }

                    Configure(serviceCollection, optionType,
                        attr.Name ?? Microsoft.Extensions.Options.Options.DefaultName,
                        configuration.GetSection(attr.FullSectionName));
                }
            }

            return serviceCollection;
        }
    }
}
