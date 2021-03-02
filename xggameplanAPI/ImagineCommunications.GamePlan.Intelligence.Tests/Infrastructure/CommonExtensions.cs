using System;
using System.Linq;
using System.Reflection;
using BoDi;
using FluentValidation;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.MassTransit.Decorators;
using ImagineCommunications.Extensions.DependencyInjection;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using MassTransit.Testing;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure
{
    public static class CommonExtensions
    {
        private class Index<T> : IIndex<T> where T : class
        {
            private readonly IObjectContainer _container;

            public Index(IObjectContainer container)
            {
                _container = container;
            }

            public T Resolve(string key) => _container.Resolve<T>(key);
        }

        public static void FromResourceScript(this ITestDataImporter dataImporter, string resourceName)
        {
            if (dataImporter == null)
            {
                throw new ArgumentNullException(nameof(dataImporter));
            }

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(assembly.GetFullManifestResourceName(resourceName)))
            {
                dataImporter.Import(stream);
            }
        }

        public static string GetFullManifestResourceName(this Assembly assembly, string partialResourceName)
        {
            var resources = assembly.GetManifestResourceNames();
            return resources.FirstOrDefault(x => x == partialResourceName) ??
                   resources.FirstOrDefault(x => x.EndsWith(partialResourceName, StringComparison.InvariantCulture)) ??
                   throw new ArgumentException("The specified resource name doesn't exist.",
                       nameof(partialResourceName));
        }

        public static void AddConsumer<TEvent, TValidator>(this InMemoryTestHarness bus, IObjectContainer objectContainer) where TEvent : class, IEvent where TValidator : AbstractValidator<TEvent>, new()
        {
            _ = bus.Consumer(() => new EventConsumer<TEvent>(
                  objectContainer.Resolve<ILoggerService>(),
                  objectContainer.Resolve<IIndex<IPayloadStorageProviderService>>(),
                  objectContainer.Resolve<IMessageInfoRepository>(),
                  objectContainer.Resolve<IGroupTransactionInfoRepository>(),
                  objectContainer.Resolve<IMessageTypeService>()));
        }

        public static void AddConsumer<TEvent>(this InMemoryTestHarness bus, IObjectContainer objectContainer) where TEvent : class, IEvent
        {
            _ = bus.Consumer(() => new EventConsumer<TEvent>(
                  objectContainer.Resolve<ILoggerService>(),
                  objectContainer.Resolve<IIndex<IPayloadStorageProviderService>>(),
                  objectContainer.Resolve<IMessageInfoRepository>(),
                  objectContainer.Resolve<IGroupTransactionInfoRepository>(),
                  objectContainer.Resolve<IMessageTypeService>()));
        }

        public static IObjectContainer AddResolver<TService>(this IObjectContainer serviceCollection)
           where TService : class
        {
            serviceCollection?.RegisterFactoryAs<IIndex<TService>>(provider => new Index<TService>(provider));

            return serviceCollection;
        }
    }
}
