using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace xggameplan.common.Utilities
{
    public static class BinarySerializationUtilities
    {
        /// <summary>
        /// Serialized object to memory stream
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static MemoryStream SerializeToMemoryStream(object objectType)
        {
            _ = objectType.GetType();
            var stream = new MemoryStream();
            _ = stream.Capacity;
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectType);

            return stream;
        }

        /// <summary>
        /// Deserializes object from memory stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object DeserializeFromMemoryStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            _ = stream.Seek(0, SeekOrigin.Begin);

            return formatter.Deserialize(stream);
        }
    }
}
