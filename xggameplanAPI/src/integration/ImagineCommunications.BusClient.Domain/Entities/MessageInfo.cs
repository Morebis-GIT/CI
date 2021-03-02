using System;

namespace ImagineCommunications.BusClient.Domain.Entities
{
    public class MessageInfo
    {
        public Guid Id { get; set; }
        public Guid GroupTransactionId { get; set; }
        public MessageState State { get; set; }
        public int Priority { get; set; }
        public MessageBodyType Type { get; set; }
        public int RetryCount { get; set; }
        public string Name { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? TotalBatchCount { get; set; }
        public int? ProcessedBatchCount { get; set; }
        public MessageType MessageType { get; set; }
    }
}
