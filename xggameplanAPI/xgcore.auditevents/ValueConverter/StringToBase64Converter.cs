using System;
using xggameplan.common.Utilities;

namespace xggameplan.AuditEvents.ValueConverter
{
    /// <summary>
    /// Converts between string and base 64 string, supports compression
    /// </summary>
    public class StringToBase64Converter : IValueConverter
    {
        private readonly bool _compression;

        public StringToBase64Converter(bool compression)
        {
            _compression = compression;
        }

        public bool CanConvert(Type fromType, Type toType)
        {
            return fromType == typeof(String) && toType == typeof(String);
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == typeof(String) && toType == typeof(String))
            {
                if (IsBase64String((String)value, _compression))    // Convert from base 64 to string
                {
                    return ConvertFromBase64((String)value, _compression);
                }
                else    // Convert from string to base 64
                {
                    return ConvertToBase64((String)value, _compression);
                }
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

        /// <summary>
        /// Returns whether string is a base 64 string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="compression"></param>
        /// <returns></returns>
        private static bool IsBase64String(string input, bool compression)
        {
            if (!String.IsNullOrEmpty(input))
            {
                try
                {
                    string result = ConvertFromBase64(input, compression);
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
