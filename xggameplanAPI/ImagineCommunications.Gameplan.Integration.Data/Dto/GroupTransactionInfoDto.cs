using System;
using ImagineCommunications.BusClient.Domain;

namespace ImagineCommunications.Gameplan.Integration.Data.Dto
{
    internal class GroupTransactionInfoDto
    {
        public Guid Id { get; set; }
        public int EventCount { get; set; }
        public MessageState State { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int MessageCount { get; set; }
    }
}
