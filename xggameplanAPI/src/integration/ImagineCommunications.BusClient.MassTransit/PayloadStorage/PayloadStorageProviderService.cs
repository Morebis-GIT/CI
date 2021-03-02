using System;
using System.IO;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Implementation.PayloadStorage
{
    public class PayloadStorageProviderService<TEvent, TEventModel> : IPayloadStorageProviderService
        where TEvent : class, IEvent
        where TEventModel : TEvent
    {
        private readonly IMessagePayloadRepository _messagePayloadRepository;
        private readonly IObjectSerializer _serializer;

        public PayloadStorageProviderService(
            IMessagePayloadRepository messagePayloadRepository,
            IObjectSerializer serializer
            )
        {
            _messagePayloadRepository = messagePayloadRepository;
            _serializer = serializer;
        }

        public void SaveMessageData(Guid messageId, object data)
        {
            using (var payloadStream = new MemoryStream())
            {
                _serializer.Serialize(payloadStream, data);

                _messagePayloadRepository.Add(new MessagePayload(messageId, payloadStream.GetBuffer()));
            }
        }

        public IEvent GetMessageData(Guid messageId)
        {
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException(nameof(messageId));
            }

            var messagePayload = _messagePayloadRepository.GetById(messageId)
                ?? throw new ArgumentException($"{nameof(MessagePayload)} with {nameof(MessagePayload.Id)}: {messageId} doesn't exist.");

            using (var payloadStream = new MemoryStream(messagePayload.Payload))
            {
                return (_serializer.Deserialize(payloadStream, typeof(TEventModel)) as TEvent);
            }
        }

        public void DeleteMessage(Guid messageId)
        {
            if (messageId == Guid.Empty)
            {
                throw new ArgumentException(nameof(messageId));
            }

            _messagePayloadRepository.DeleteById(messageId);
        }
    }
}
