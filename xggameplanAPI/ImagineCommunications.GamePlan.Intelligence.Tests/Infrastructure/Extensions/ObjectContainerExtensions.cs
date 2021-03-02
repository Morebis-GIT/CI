using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using BoDi;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.BusClient.Implementation.Decorators;
using ImagineCommunications.BusClient.Implementation.PayloadStorage;
using ImagineCommunications.BusClient.MassTransit.Decorators;
using ImagineCommunications.Extensions.DependencyInjection;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Extensions
{
    public static class ObjectContainerExtensions
    {
        private static string FeatureDependenciesName = "featureDependencies";
        private static string ScenarioDependenciesName = "scenarioDependencies";
        
        public static IObjectContainer AddAutoMapper(this IObjectContainer builder, params Assembly[] assemblies)
        {
            if (builder is null)
            {
                return null;
            }

            var configurationProvider = new MapperConfiguration(config => config.AddMaps(assemblies));

            builder.RegisterInstanceAs<IConfigurationProvider>(configurationProvider);
            builder.RegisterInstanceAs<IMapper>(new Mapper(builder.Resolve<IConfigurationProvider>()));

            return builder;
        }
        
        private static ICollection<IFeatureDependency> GetFeatureDependencies(IObjectContainer objectContainer)
        {
            if (!objectContainer.IsRegistered<ICollection<IFeatureDependency>>(FeatureDependenciesName))
            {
                objectContainer.RegisterInstanceAs<ICollection<IFeatureDependency>>(new List<IFeatureDependency>(), FeatureDependenciesName);
            }

            return objectContainer.Resolve<ICollection<IFeatureDependency>>(FeatureDependenciesName);
        }

        private static ICollection<IScenarioDependency> GetScenarioDependencies(IObjectContainer objectContainer)
        {
            if (!objectContainer.IsRegistered<ICollection<IScenarioDependency>>(ScenarioDependenciesName))
            {
                objectContainer.RegisterInstanceAs<ICollection<IScenarioDependency>>(new List<IScenarioDependency>(), ScenarioDependenciesName);
            }
            return objectContainer.Resolve<ICollection<IScenarioDependency>>(ScenarioDependenciesName);
        }

        public static void RegisterFeatureDependency<TType>(this IObjectContainer objectContainer)
            where TType : IFeatureDependency
        {
            var featureDependencies = GetFeatureDependencies(objectContainer);
            featureDependencies.Add(objectContainer.Resolve<TType>());
        }

        public static IEnumerable<IFeatureDependency> ResolveFeatureDependencies(this IObjectContainer objectContainer)
        {
            return GetFeatureDependencies(objectContainer);
        }

        public static void RegisterScenarioDependency<TType>(this IObjectContainer objectContainer)
            where TType : IScenarioDependency
        {
            var scenarioDependencies = GetScenarioDependencies(objectContainer);
            scenarioDependencies.Add(objectContainer.Resolve<TType>());
        }

        public static IEnumerable<IScenarioDependency> ResolveScenarioDependencies(this IObjectContainer objectContainer)
        {
            return GetScenarioDependencies(objectContainer);
        }

        public static EventConsumer<TEvent> CreateConsumer<TEvent>(this IObjectContainer objectContainer) where TEvent : class, IEvent
        {
            return new EventConsumer<TEvent>(
                objectContainer.Resolve<ILoggerService>(),
                objectContainer.Resolve<IIndex<IPayloadStorageProviderService>>(),
                objectContainer.Resolve<IMessageInfoRepository>(),
                objectContainer.Resolve<IGroupTransactionInfoRepository>(),
                objectContainer.Resolve<IMessageTypeService>());
        }

        internal class StubBatchingHandler<TEvent> : IBatchingHandler<TEvent> where TEvent : class, IEvent
        {
            private readonly IEventHandler<TEvent> _handler;

            public StubBatchingHandler(IEventHandler<TEvent> handler)
            {
                _handler = handler; 
            }

            public void Handle(MessageInfo info, TEvent command)
            {
                _handler.Handle(command);
            }
        }

        public static void AddHandler<TEvent, TEventHandler, TEventModel>(this IObjectContainer objectContainer) where TEventHandler : class, IEventHandler<TEvent> where TEvent : class, IEvent where TEventModel : TEvent
        {
            objectContainer.RegisterTypeAs<TEventHandler, IEventHandler<TEvent>>();
            objectContainer.RegisterTypeAs<PayloadStorageProviderService<TEvent, TEventModel>, IPayloadStorageProviderService>(typeof(TEvent).Name);
            objectContainer.RegisterTypeAs<EventHandlerDispatcher<TEvent>, IHandlerDispatcher>(typeof(TEvent).Name);
            objectContainer.RegisterTypeAs<StubBatchingHandler<TEvent>, IBatchingHandler<TEvent>>();
        }
    }
}
