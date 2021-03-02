using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Domain;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class GroupTransactionInfo
    {
        public Guid Id { get; set; }
        public int EventCount { get; set; }
        public MessageState State { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public ICollection<MessageInfo> Messages { get; set; }
    }
}
