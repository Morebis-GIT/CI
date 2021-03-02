using System;
using System.IO;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface IObjectSerializer
    {
        void Serialize(Stream stream, object value);
        object Deserialize(Stream stream, Type toType);
    }
}
