using System;
using System.ComponentModel;

namespace xggameplan.AuditEvents.ValueConverter
{
    /// <summary>
    /// Converts between base types
    /// </summary>
    public class BaseTypeConverter : IValueConverter
    {
        public BaseTypeConverter()
        {

        }

        public bool CanConvert(Type fromType, Type toType)
        {
            return IsBaseType(fromType) && IsBaseType(toType);
        }

        private static bool IsBaseType(Type type)
        {
            return Array.IndexOf(new Type[] { typeof(byte), typeof(char), typeof(bool), typeof(decimal), typeof(double), typeof(short),
                                typeof(int), typeof(long), typeof(Guid), typeof(float), typeof(DateTime), typeof(Single), typeof(string),
                                typeof(ushort), typeof(uint), typeof(ulong), typeof(sbyte) }, type) != -1;
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == toType)
            {
                return value;
            }
            if (toType == typeof(String))
            {
                return value.ToString();
            }
            TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(toType);
            return typeConverter.ConvertFrom(value);
        }
    }
}
