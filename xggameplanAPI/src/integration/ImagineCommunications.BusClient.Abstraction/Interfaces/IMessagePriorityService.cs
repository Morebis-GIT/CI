using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IMessageTypeService
    {
        MessageType GetMessageType(string name);
    }
}
