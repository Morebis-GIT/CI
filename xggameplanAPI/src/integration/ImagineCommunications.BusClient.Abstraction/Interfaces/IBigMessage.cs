using System;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IBigMessage : IEvent
    {
        /// <summary>
        /// Returns the address of the big message.
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// Returns the type of the big message.
        /// </summary>
        Type Type { get; }
    }
}
