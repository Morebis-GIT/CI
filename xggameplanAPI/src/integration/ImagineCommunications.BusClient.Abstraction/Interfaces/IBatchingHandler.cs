using System.Collections.Generic;
using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IBatchingHandler<TEvent> where TEvent : class, IEvent
    {
        void Handle(MessageInfo info, TEvent command);
    }

    public interface IBatchingEnumerateHandler<in TBulkEvent, in TEvent>
        where TBulkEvent : IBulkEvent<TEvent>
        where TEvent : IEvent
    {
        void Handle(IEnumerator<TEvent> eventEnumerator);
    }
}
