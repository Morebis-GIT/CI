using System;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IEventHandlerDispatcher
    {
        void Handle(object command, Type type);
    }
}
