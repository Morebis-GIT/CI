using System;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class MessagePayload 
    {
        public Guid Id { get; set; }
        public byte[] Payload { get; set; }
    }
}
