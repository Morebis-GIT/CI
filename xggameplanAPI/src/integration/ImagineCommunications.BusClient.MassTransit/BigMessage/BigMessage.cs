using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Implementation.BigMessage
{
    public class BigMessage : IBigMessage
    {
        public BigMessage(Uri address, Type type)
        {
            Address = address;
            Type = type;
        }

        public Uri Address { get; }

        public Type Type { get; }
    }
}
