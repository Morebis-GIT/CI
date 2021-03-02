using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ImagineCommunications.BusClient.Implementation.Decorators
{
    public class EventHandlerDispatcher<TEvent> : IHandlerDispatcher
        where TEvent : class, IEvent
    {
        private readonly IEventHandler<TEvent> _eventHandler;
        private readonly IContractValidatorService _contractValidatorService;
        private IServiceProvider _serviceProvider;
        private readonly ILoggerService _logger;

        public EventHandlerDispatcher(IEventHandler<TEvent> eventHandler,
            IContractValidatorService contractValidatorService, IServiceProvider serviceProvider,
            ILoggerService logger)
        {
            _eventHandler = eventHandler;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _contractValidatorService = contractValidatorService;
        }

        public void Dispatch(MessageInfo info, object message)
        {
            var messageData = (TEvent)message;

            _logger?.Info($"Event: EventHandlerDispatcher validate message is started; MessageId:{info.Id}");
            _contractValidatorService.Validate(messageData);
            _logger?.Info($"Event: EventHandlerDispatcher validate message is finished; MessageId:{info.Id}");

            _logger?.Info($"Event: EventHandlerDispatcher handle message is started; MessageId:{info.Id}");
            if (info.MessageType?.BatchSize != null && info.MessageType?.BatchSize != 0)
            {
                var batchingHandler = _serviceProvider.GetService<IBatchingHandler<TEvent>>();
                if (batchingHandler != null)
                {
                    batchingHandler.Handle(info, messageData);
                }
                else
                {
                    _eventHandler.Handle(messageData);
                }
            }
            else
            {
                _eventHandler.Handle(messageData);
            }
            _logger?.Info($"Event: EventHandlerDispatcher handle message is finished; MessageId:{info.Id}");
        }
    }
}
