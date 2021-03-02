namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent command);
    }
}
