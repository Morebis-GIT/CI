using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace xggameplan.common.Extensions
{
    public static class EnumExtension
    {
        public static bool HasAttribute<T>(this Enum value) where T : Attribute
        {
            FieldInfo field = GetField(value);

            var attribute = GetAttribute<T>(field);

            return attribute != null;
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = GetField(value);

            var attribute = GetAttribute<DescriptionAttribute>(field);

            return attribute == null ? string.Empty : attribute.Description;
        }

        public static bool TryGetValueFromDescription<T>(this string value, out T result) where T : struct
        {
            var fields = EnumTypeToValuesList<T>();

            if (!fields.Any())
            {
                return FailedResult(out result);
            }

            var lookup = fields.FirstOrDefault(field => field.GetDescription().ToUpper() == value.ToUpper());

            if (lookup != null)
            {
                return Enum.TryParse<T>(lookup.ToString(), out result);
            }
            else
            {
                return FailedResult(out result);
            }
        }

        public static bool TryGetValueFromString<T>(this string value, out T result) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                return FailedResult(out result);
            }

            return Enum.TryParse(value, out result);
        }

        private static FieldInfo GetField(Enum value)
        {
            return value.GetType().GetField(value.ToString());
        }

        private static IEnumerable<Enum> EnumTypeToValuesList<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                return new Enum[0];
            }

            return Enum.GetValues(typeof(T)).Cast<Enum>();
        }

        private static T GetAttribute<T>(FieldInfo field) where T : Attribute
        {
            return Attribute.GetCustomAttribute(field, typeof(T)) as T;
        }

        private static bool FailedResult<T>(out T result)
        {
            result = default(T);
            return false;
        }
    }
}
