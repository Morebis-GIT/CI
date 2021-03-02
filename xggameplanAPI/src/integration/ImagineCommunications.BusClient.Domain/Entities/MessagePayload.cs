using System;

namespace ImagineCommunications.BusClient.Domain.Entities
{
    public class MessagePayload
    {
        public Guid Id { get; set; }
        public byte[] Payload { get; set; }
        public MessagePayload(Guid id, byte[] payload)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(nameof(id));
            }

            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            Id = id;
            Payload = payload;
        }
    }
}
