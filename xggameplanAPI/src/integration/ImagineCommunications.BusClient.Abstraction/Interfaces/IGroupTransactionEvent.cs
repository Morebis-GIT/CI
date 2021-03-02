using System;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IGroupTransactionEvent : IEvent
    {
        int EventCount { get; set; }
        DateTime CreatedOn { get; set; }
    }
}
