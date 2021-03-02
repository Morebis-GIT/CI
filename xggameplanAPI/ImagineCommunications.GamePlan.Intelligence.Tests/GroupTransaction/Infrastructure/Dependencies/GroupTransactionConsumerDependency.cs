using System;
using BoDi;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Implementation.GroupTransactions;
using ImagineCommunications.BusClient.Implementation.PayloadStorage;
using ImagineCommunications.BusClient.Implementation.Services;
using ImagineCommunications.BusClient.MassTransit;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.GroupTransaction;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Handlers;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Extensions;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using MassTransit.Testing;
using Moq;
using xggameplan.common.Caching;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Dependencies
{
    public class GroupTransactionConsumerDependency : IScenarioDependency
    {
        public void Register(IObjectContainer objectContainer)
        {
            objectContainer.RegisterInstanceAs(Mock.Of<IServiceProvider>());
            objectContainer.RegisterInstanceAs(Mock.Of<IObjectStorage>());
            objectContainer.RegisterTypeAs<JsonObjectSerializer, IObjectSerializer>();
            objectContainer.RegisterTypeAs<GroupTransactionExecutionService, IGroupTransactionExecutionService>();
            RegisterHandlers(objectContainer);

            objectContainer.RegisterInstanceAs<ICache>(new InMemoryCache());
            objectContainer.RegisterTypeAs<MessageTypeService, IMessageTypeService>();

            InMemoryTestHarness bus = new InMemoryTestHarness();
            bus.AddConsumer<IGroupTransactionEvent>(objectContainer);
            bus.AddConsumer<IMockEventOne>(objectContainer);
            bus.AddConsumer<IMockEventTwo>(objectContainer);
            bus.AddConsumer<IMockEventThree>(objectContainer);
            bus.AddConsumer<IMockEventFour>(objectContainer);

            bus.Start().GetAwaiter().GetResult();
            objectContainer.RegisterInstanceAs<InMemoryTestHarness>(bus);
            objectContainer.RegisterInstanceAs<IServiceBus>(new ServiceBus(
                bus.BusControl,
                Mock.Of<IContractValidatorService>(),
                objectContainer.Resolve<IBigMessageService>(),
                objectContainer.Resolve<ObjectStorageConfiguration>()));
        }

        private void RegisterHandlers(IObjectContainer objectContainer)
        {
            objectContainer.AddHandler<IMockEventOne, MockEventHandler<IMockEventOne>, MockEventOne>();
            objectContainer.AddHandler<IMockEventTwo, MockEventHandler<IMockEventTwo>, MockEventTwo>();
            objectContainer.AddHandler<IMockEventThree, MockEventHandler<IMockEventThree>, MockEventThree>();
            objectContainer.AddHandler<IMockEventFour, MockEventHandler<IMockEventFour>, MockEventFour>();
            _ = objectContainer.AddResolver<IHandlerDispatcher>();
            _ = objectContainer.AddResolver<IPayloadStorageProviderService>();
            objectContainer.RegisterTypeAs<PayloadStorageProviderService<IGroupTransactionEvent, GroupTransactionEvent>, IPayloadStorageProviderService>(nameof(IGroupTransactionEvent));
        }
    }
}
