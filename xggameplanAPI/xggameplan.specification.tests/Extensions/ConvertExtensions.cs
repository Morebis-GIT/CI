using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist;

namespace xggameplan.specification.tests.Extensions
{
    public static class ConvertExtensions
    {
        private const string NullStringValue = "null";

        private static object ConvertBySpecflowRetrievers(KeyValuePair<string, string> pair, Type targetType, Type toType)
        {
            foreach (var retriever in Service.Instance.ValueRetrievers)
            {
                if (retriever.CanRetrieve(pair, targetType, toType))
                {
                    return retriever.Retrieve(pair, targetType, toType);
                }
            }

            throw new Exception($"'{pair.Key}' value can't be converted to '{toType.Name}' type.");
        }

        public static T SpecflowConvert<T>(this string value)
        {
            return (T) SpecflowConvert(value, typeof(T));
        }

        public static object SpecflowConvert(this string value, Type toType)
        {
            if (value == NullStringValue || value == null)
            {
                return null;
            }
            return ConvertBySpecflowRetrievers(new KeyValuePair<string, string>(value, value), null, toType);
        }

        public static object GetBySpecflowService(this IDictionary<string, string> dictionary, string key, Type toType)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (toType == null)
            {
                throw new ArgumentNullException(nameof(toType));
            }

            if (dictionary.TryGetValue(key, out var value))
            {
                if (value == NullStringValue || value == null)
                {
                    return null;
                }

                return ConvertBySpecflowRetrievers(new KeyValuePair<string, string>(key, value), null, toType);
            }

            throw new Exception($"'{key}' key doesn't exist.");
        }

        [Obsolete("Define parameters of repository adapter method explicitly instead of using dictionary of parameters")]
        public static T GetBySpecflowService<T>(this IDictionary<string, string> dictionary, string key)
        {
            return (T)GetBySpecflowService(dictionary, key, typeof(T));
        }
    }
}
