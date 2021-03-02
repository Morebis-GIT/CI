using System;
using Newtonsoft.Json;
using xggameplan.common.Utilities;

namespace xggameplan.AuditEvents.ValueConverter
{
    /// <summary>
    /// Converts between object and base 64 JSON string, supports compression
    /// </summary>
    public class ObjectToBase64JSONConverter : IValueConverter
    {
        private bool _compression;

        public ObjectToBase64JSONConverter(bool compression)
        {
            _compression = compression;
        }

        public bool CanConvert(Type fromType, Type toType)
        {
            return (fromType == typeof(String) || toType == typeof(String));
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == typeof(String))
            {
                string json = ConvertFromBase64((string)value, _compression);
                object theObject = JsonConvert.DeserializeObject(json, toType);
                return theObject;
            }
            else if (toType == typeof(String))
            {
                string json = JsonConvert.SerializeObject(value, Formatting.Indented);
                return ConvertToBase64(json, _compression);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts to base 64 string, compresses if required
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="compression"></param>
        /// <returns></returns>
        private static string ConvertToBase64(string plainText, bool compression)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return compression ? System.Convert.ToBase64String(DeflateStreamCompression.CompressBytes(bytes)) : System.Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts from base 64 string, decompresses if required
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <param name="compression"></param>
        /// <returns></returns>
        private static string ConvertFromBase64(string base64EncodedData, bool compression)
        {
            var bytes = System.Convert.FromBase64String(base64EncodedData);
            return compression ? System.Text.Encoding.UTF8.GetString(DeflateStreamCompression.DecompressBytes(bytes)) : System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
