using System;
using System.Threading.Tasks;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Extensions.DependencyInjection;
using MassTransit;

namespace ImagineCommunications.BusClient.MassTransit.Decorators
{
    public class EventConsumer<T> : IEventHandler<T>, IConsumer<T> where T : class, IEvent
    {
        public const string GroupTransactionHeaderKey = "GroupTransactionHeaderKey";

        private readonly ILoggerService _logger;
        private readonly IPayloadStorageProviderService _payloadStorageProviderService;
        private readonly IMessageInfoRepository _messageInfoRepository;
        private readonly IGroupTransactionInfoRepository _groupTransactionRepository;
        private readonly IMessageTypeService _priorityService;

        public EventConsumer(
            ILoggerService logger,
            IIndex<IPayloadStorageProviderService> payloadStorageProviderResolver,
            IMessageInfoRepository messageInfoRepository,
            IGroupTransactionInfoRepository groupTransactionRepository,
            IMessageTypeService priorityService)
        {
            _logger = logger;
            _payloadStorageProviderService = payloadStorageProviderResolver.Resolve(typeof(T).Name);
            _messageInfoRepository = messageInfoRepository;
            _groupTransactionRepository = groupTransactionRepository;
            _priorityService = priorityService;
        }

        public EventConsumer() { }

        public Task Consume(ConsumeContext<T> context)
        {
            try
            {
                if (context.Message is IGroupTransactionEvent groupTransaction)
                {
                    var transactionInfo = new GroupTransactionInfo()
                    {
                        Id = context.MessageId ?? throw new ArgumentNullException(nameof(context.MessageId)),
                        EventCount = groupTransaction.EventCount,
                        CreatedDate = groupTransaction.CreatedOn,
                        ReceivedDate = DateTime.Now
                    };

                    _groupTransactionRepository.Add(transactionInfo);
                }
                else
                {
                    if (!context.Headers.TryGetHeader(GroupTransactionHeaderKey, out var value))
                    {
                        throw new InvalidOperationException($"Event: {typeof(T).Name}; MessageId: {context.MessageId} isn't attached to any transaction. " +
                            $"Please, put groupTransactionId in '{GroupTransactionHeaderKey}' header.");
                    }

                    var messageInfo = new MessageInfo()
                    {
                        Id = context.MessageId ?? throw new ArgumentNullException(nameof(context.MessageId)),
                        GroupTransactionId = Guid.Parse(value as string),
                        State = MessageState.Pending,
                        ReceivedDate = DateTime.Now
                    };

                    string key;

                    if (context.Message is IBigMessage payloadReference)
                    {
                        key = payloadReference.Type.Name;
                        messageInfo.Type = MessageBodyType.BigMessage;
                    }
                    else
                    {
                        key = typeof(T).Name;
                        messageInfo.Type = MessageBodyType.NormalMessage;
                    }

                    var type = _priorityService.GetMessageType(key);

                    messageInfo.Name = key;
                    messageInfo.Priority = type?.Priority ?? 0;

                    _messageInfoRepository.Add(messageInfo);

                    _payloadStorageProviderService.SaveMessageData(messageInfo.Id, context.Message);
                }

                _logger?.Info($"Event: {typeof(T).Name} saved successfully; MessageId: {context.MessageId} ");
            }
            catch (Exception e)
            {
                _logger?.Error($"Exception was thrown during processing Event: {typeof(T).Name}; MessageId: {context.MessageId} ", e);
                throw;
            }

            return Task.CompletedTask;
        }

        public void Handle(T command)
        {
        }
    }
}
