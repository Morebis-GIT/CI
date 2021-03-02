using System;

namespace xggameplan.model.Internal.Landmark
{
    public class GroupTransactionInfoModel
    { 
        public Guid Id { get; set; }
        public int EventCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public MessageState State { get; set; }
    }
}
