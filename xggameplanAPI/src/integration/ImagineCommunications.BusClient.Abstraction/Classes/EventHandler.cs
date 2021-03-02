using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Abstraction.Classes
{
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
    {
        public abstract void Handle(TEvent command);
    }
}
