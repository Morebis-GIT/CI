using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Implementation.Services
{
    public class JsonObjectSerializer : IObjectSerializer
    {
        private const int BufferSize = 16 * 1024;

        public void Serialize(Stream stream, object value)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, BufferSize, true))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public object Deserialize(Stream s, Type type)
        {
            using (var reader = new StreamReader(s))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();

                return ser.Deserialize(jsonReader, type);
            }
        }
    }
}
