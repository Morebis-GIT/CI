using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.GroupTransaction
{
    public class GroupTransactionEvent : IGroupTransactionEvent
    {
        public int EventCount { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public GroupTransactionEvent(int eventCount)
        {
            EventCount = eventCount;
        }
    }
}
