using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using xggameplan.common.Extensions;

namespace xggameplan.common.Utilities
{
    public static class EnumUtilities
    {
        public static bool CanConvertToEnum(Type enumType, string value)
        {
            bool canConvert = false;

            try
            {
                object output = Enum.Parse(enumType, value);
                canConvert = true;
            }
            catch { }

            return canConvert;
        }

        public static IEnumerable<string> ToDescriptionList<T>()
        {
            if (!typeof(T).IsEnum)
            {
                return new string[0];
            }

            var fields = Enum.GetValues(typeof(T)).Cast<Enum>();

            if (fields.Any(field => !field.HasAttribute<DescriptionAttribute>()))
            {
                return new string[0];
            }

            return fields.Select(field => field.GetDescription());
        }

        /// <summary>
        /// To convert the supplied string value into an Enum of type TEnum
        /// </summary>
        /// <typeparam name="TEnum">
        /// The type of the Enum to convert the value to
        /// </typeparam>
        /// <param name="value">The value of type <see cref="string"/> to convert</param>
        /// <returns>The converted value in Enum of the supplied type TEnum</returns>
        public static TEnum? ConvertStringToEnum<TEnum>(string value) where TEnum : struct
        {
            TEnum? defaultResult = null;
            return (!string.IsNullOrWhiteSpace(value) &&
                    Enum.TryParse<TEnum>(value, true, out TEnum result)) ? result : defaultResult;
        }
    }
}
