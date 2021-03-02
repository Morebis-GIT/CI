using System.Collections.Generic;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IBulkEvent<out T> : IEvent where T : IEvent
    {
        IEnumerable<T> Data { get; }
    }
}
