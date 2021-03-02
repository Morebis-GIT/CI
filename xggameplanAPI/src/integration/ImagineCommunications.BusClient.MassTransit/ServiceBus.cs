using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MassTransit;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Implementation.BigMessage;
using ImagineCommunications.BusClient.Abstraction.Models;

namespace ImagineCommunications.BusClient.MassTransit
{
    public class ServiceBus : IServiceBus
    {
        private const string GroupTransactionHeaderKey = "GroupTransactionHeaderKey";

        private readonly IBusControl _massTransitBus;
        private readonly IContractValidatorService _contractValidatorService;
        private readonly IBigMessageService _bigMessageService;
        private readonly ObjectStorageConfiguration _config;

        public ServiceBus(
            IBusControl massTransitBus,
            IContractValidatorService contractValidatorService,
            IBigMessageService bigMessageService = null,
            ObjectStorageConfiguration config = null)
        {
            _massTransitBus = massTransitBus;
            _contractValidatorService = contractValidatorService;
            _bigMessageService = bigMessageService;
            _config = config;
        }

        public async Task<Guid> PublishAsync<T>(T eventMessage, Guid? groupTransactionId = null, Dictionary<string, object> headers = null) where T : class, IEvent
        {
            Guid messageId = Guid.Empty;
            ValidateContract<T>((T)eventMessage);

            object messageToSend = eventMessage;
            if (_bigMessageService != null && eventMessage.IsBulkEvent())
            {
                var payload = _bigMessageService.SerializeMessage(eventMessage);
                if (payload.Length > (_config?.SerializedSizeThreshold ?? 0))
                {
                    messageToSend = await _bigMessageService.StoreMessageAsync<T>(payload).ConfigureAwait(false);
                    await _massTransitBus.Publish<IBigMessage>((IBigMessage)messageToSend, pc =>
                    {
                        messageId = ConfigurePublish(groupTransactionId, headers, pc);
                    }).ConfigureAwait(false);

                    return messageId;
                }
            }

            await _massTransitBus.Publish<T>((T)messageToSend, pc =>
            {
                messageId = ConfigurePublish(groupTransactionId, headers, pc);
            }).ConfigureAwait(false);

            return messageId;
        }

        private static Guid ConfigurePublish(Guid? groupTransactionId, Dictionary<string, object> headers, PublishContext pc)
        {
            if (groupTransactionId.HasValue && groupTransactionId != Guid.Empty)
            {
                pc.Headers.Set(GroupTransactionHeaderKey, groupTransactionId.ToString());
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    pc.Headers.Set(header.Key, header.Value);
                }
            }

            return pc.MessageId ?? Guid.Empty;
        }

        public async Task SendAsync<T>(T commandMessage) where T : class, ICommand
        {
            ValidateContract<T>((T)commandMessage);
            await _massTransitBus.Send(commandMessage).ConfigureAwait(false);
        }

        public void Start()
        {
            _massTransitBus.Start();
        }

        public void Stop()
        {
            _massTransitBus.Stop();
        }

        public async Task StartAsync()
        {
            await _massTransitBus.StartAsync().ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            await _massTransitBus.StopAsync().ConfigureAwait(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _massTransitBus.Stop();
            }
        }

        private void ValidateContract<T>(T message)
        {
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException("Only interfaces are allowed as a generic parameters");
            }

            _contractValidatorService.Validate<T>((T)message);
            if (message == null)
            {
                throw new InvalidDataException("Event message can't be null");
            }
        }
    }
}
