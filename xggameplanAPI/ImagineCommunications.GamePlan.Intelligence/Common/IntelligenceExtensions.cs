using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Implementation.Decorators;
using ImagineCommunications.BusClient.Implementation.PayloadStorage;
using ImagineCommunications.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.GamePlan.Intelligence.Common
{
    public static class IntelligenceExtensions
    {
        public static void RegisterModule(this IServiceCollection collection, IDependencyModule dependencyModule) =>
            dependencyModule?.Register(collection);

        public static void AddHandler<TEvent, TEventHandler, TEventModel>(this IServiceCollection serviceCollection)
            where TEventHandler : class, IEventHandler<TEvent>
            where TEvent : class, IEvent
            where TEventModel : TEvent
        {
            serviceCollection.AddScoped<IEventHandler<TEvent>, TEventHandler>();
            serviceCollection.AddTransient<IHandlerDispatcher, EventHandlerDispatcher<TEvent>>(typeof(TEvent).Name);
            serviceCollection.AddTransient<IPayloadStorageProviderService, PayloadStorageProviderService<TEvent, TEventModel>>(typeof(TEvent).Name);
        }

        public static void AddHandler<TEvent, TEventHandler, TBatchingHandler, TEventModel>(this IServiceCollection serviceCollection)
            where TEventHandler : class, IEventHandler<TEvent>
            where TEvent : class, IEvent
            where TEventModel : TEvent
        where TBatchingHandler : class, IBatchingHandler<TEvent>
        {
            serviceCollection.AddScoped<IEventHandler<TEvent>, TEventHandler>();
            serviceCollection.AddTransient<IHandlerDispatcher, EventHandlerDispatcher<TEvent>>(typeof(TEvent).Name);
            serviceCollection.AddTransient<IPayloadStorageProviderService, PayloadStorageProviderService<TEvent, TEventModel>>(typeof(TEvent).Name);
            serviceCollection.AddScoped<IBatchingHandler<TEvent>, TBatchingHandler>();
        }
    }
}
