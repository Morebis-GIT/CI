using System;
using System.IO;
using System.Threading.Tasks;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Implementation.BigMessage
{
    /// <inheritdoc />
    public class BigMessageService : IBigMessageService
    {
        private readonly IObjectStorage _storage;
        private readonly IObjectSerializer _serializer;

        public BigMessageService(IObjectStorage storage, IObjectSerializer serializer)
        {
            _storage = storage;
            _serializer = serializer;
        }

        /// <inheritdoc />
        public async Task<IBigMessage> StoreMessageAsync<T>(Stream message) where T : IEvent
        {
            Uri uri = await _storage.StoreAsync(message).ConfigureAwait(false);

            return new BigMessage(uri, typeof(T));
        }

        /// <inheritdoc />
        public Stream SerializeMessage<T>(T message) where T : IEvent
        {
            var stream = new MemoryStream();
            _serializer.Serialize(stream, message);

            return stream;
        }

        /// <inheritdoc />
        public async Task<object> GetMessageAsync(IBigMessage bigMessage) =>
                _serializer.Deserialize(
                    await _storage.GetAsync(bigMessage.Address).ConfigureAwait(false),
                    bigMessage.GetPayloadType());

    }
}
