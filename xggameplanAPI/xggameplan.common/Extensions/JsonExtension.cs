using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace xggameplan.Extensions
{
    public static class Conversion
    {
        public static T JsonDeserialize<T>(this string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException();
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string JsonSerialize<T>(this List<T> value)
        {
            if (value is null)
            {
                throw new ArgumentNullException();
            }

            return JsonConvert.SerializeObject(value);
        }

        public static object GetValue(this object obj, string propertyname)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (String.IsNullOrWhiteSpace(propertyname))
            {
                throw new ArgumentNullException(nameof(propertyname));
            }

            var propertyInfo = obj.GetType().GetProperty(propertyname);
            return propertyInfo?.GetValue(obj, null);
        }
    }
}
