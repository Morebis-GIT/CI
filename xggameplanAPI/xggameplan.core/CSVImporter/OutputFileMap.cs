using System;
using CsvHelper.Configuration;

namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Base class for AutoBook output files
    /// </summary>
    public abstract class OutputFileMap<T1> : ClassMap<T1>
    {
        private const string _nullValue = "#";

        /// <summary>
        /// Converts value (typically string) to specified type. This is typically only necessary for nullable types because they appear in 
        /// output file with value #
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected T ConvertValue<T>(object value)
        {
            var type = typeof(T);
            bool isNullable = Nullable.GetUnderlyingType(type) != null;
            if (isNullable)
            {
                if ((value == null) || (value.ToString() == "") || (value != null && value.ToString() == _nullValue))
                {
                    return default(T);
                }
            }
            else if ((value == null) || (type == value.GetType()))
            {
                return (T)value;
            }

            object result = null;
            if (type == typeof(Int16?) || type == typeof(Int16))
            {
                result = Convert.ToInt16(value);
            }
            else if (type == typeof(Int32?) || type == typeof(Int32))
            {
                result = Convert.ToInt32(value);
            }
            else if (type == typeof(Int64?) || type == typeof(Int64))
            {
                result = Convert.ToInt64(value);
            }
            else if (type == typeof(Double?) || type == typeof(Double))
            {
                result = Convert.ToDouble(value);
            }
            else if (type == typeof(UInt16?) || type == typeof(UInt16))
            {
                result = Convert.ToUInt16(value);
            }
            else if (type == typeof(UInt32?) || type == typeof(UInt32))
            {
                result = Convert.ToUInt32(value);
            }
            else if (type == typeof(UInt64?) || type == typeof(UInt64))
            {
                result = Convert.ToUInt64(value);
            }
            else if (type == typeof(String))
            {
                result = Convert.ToString(value);
            }
            else if (type == typeof(Byte?) || type == typeof(Byte))
            {
                result = Convert.ToByte(value);
            }
            else if (type == typeof(Single?) || type == typeof(Single))
            {
                result = Convert.ToSingle(value);                
            }
            else if (type == typeof(Decimal?) || type == typeof(Decimal))
            {
                result = Convert.ToDecimal(value);
            }
            else if (type == typeof(Boolean?) || type == typeof(Boolean))
            {
                result = (!String.IsNullOrEmpty(value.ToString()) && Array.IndexOf(new string[] { "Y", "T", "1" }, value.ToString().Substring(0, 1).ToUpper()) != -1);
            }
            return (T)result;
        }
    }
}
