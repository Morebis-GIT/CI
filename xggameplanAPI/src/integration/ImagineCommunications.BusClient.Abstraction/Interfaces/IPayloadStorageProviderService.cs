using System;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IPayloadStorageProviderService
    {
        void SaveMessageData(Guid messageId, object data);
        IEvent GetMessageData(Guid messageId);
        void DeleteMessage(Guid messageId);
    }
}
