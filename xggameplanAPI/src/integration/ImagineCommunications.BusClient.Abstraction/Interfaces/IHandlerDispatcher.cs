using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IHandlerDispatcher
    {
        void Dispatch(MessageInfo info, object message);
    }
}
