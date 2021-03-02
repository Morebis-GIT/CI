using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.common.Utilities;

namespace xggameplan.AuditEvents.ValueConverter
{
    /// <summary>
    /// Converts between object and binary base 64, supports compression
    /// </summary>
    public class ObjectToBase64BinaryConverter : IValueConverter
    {
        private List<Type> _types = new List<Type>();
        private List<bool> _includeSubTypes = new List<bool>();
        private bool _compression = false;

        public ObjectToBase64BinaryConverter(List<Type> objectTypes, List<bool> includeSubTypes, bool compression)
        {
            _types = objectTypes;
            _includeSubTypes = includeSubTypes;
            _compression = compression;
        }

        public bool CanConvert(Type fromType, Type toType)
        {
            if (fromType == typeof(String)) // Convert from binary to object
            {
                for (int index = 0; index < _types.Count; index++)
                {
                    Type type = _types[index];
                    if ((toType == type) || (toType.IsSubclassOf(type) && _includeSubTypes[index]))
                    {
                        return true;
                    }
                }
            }
            else if (toType == typeof(String))  // Convert from object to binary
            {
                for (int index = 0; index < _types.Count; index++)
                {
                    Type type = _types[index];
                    if ((fromType == type) || (fromType.IsSubclassOf(type) && _includeSubTypes[index]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == typeof(String)) // Convert from binary to object
            {
                for (int index = 0; index < _types.Count; index++)
                {
                    Type type = _types[index];
                    if ((toType == type) || (toType.IsSubclassOf(type) && _includeSubTypes[index]))
                    {
                        byte[] bytes = System.Convert.FromBase64String((String)value);
                        if (_compression)
                        {
                            MemoryStream stream = new MemoryStream(DeflateStreamCompression.DecompressBytes(bytes));
                            return BinarySerializationUtilities.DeserializeFromMemoryStream(stream);
                        }
                        else
                        {
                            MemoryStream stream = new MemoryStream(bytes);
                            return BinarySerializationUtilities.DeserializeFromMemoryStream(stream);
                        }
                    }
                }
            }
            else if (toType == typeof(String))  // Convert from object to binary
            {
                for (int index = 0; index < _types.Count; index++)
                {
                    Type type = _types[index];
                    if ((fromType == type) || (fromType.IsSubclassOf(type) && _includeSubTypes[index]))
                    {
                        MemoryStream stream = BinarySerializationUtilities.SerializeToMemoryStream(value);
                        if (_compression)
                        {
                            byte[] bytes = stream.GetBuffer();
                            return System.Convert.ToBase64String(DeflateStreamCompression.CompressBytes(bytes));
                        }
                        else
                        {
                            byte[] bytes = stream.GetBuffer();
                            return System.Convert.ToBase64String(bytes);
                        }
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}
