using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.ValueConverters
{
    internal class StringDictionaryValueConverter<TDictionary, TKey, TValue> : ValueConverter<TDictionary, string>
        where TDictionary : class, IDictionary<TKey, TValue>
        where TKey : IConvertible
        where TValue : IConvertible
    {
        public const string KeyValueSeparator = "⁞";

        private static readonly StringCollectionValueConverter<List<TKey>, TKey> KeyCollectionValueConverter =
            new StringCollectionValueConverter<List<TKey>, TKey>();
        private static readonly StringCollectionValueConverter<List<TValue>, TValue> ValueCollectionValueConverter =
            new StringCollectionValueConverter<List<TValue>, TValue>();

        private static TDictionary ConvertToDictionary(string value)
        {
            var dictionaryType = typeof(TDictionary);
            if (!(dictionaryType.IsClass && !dictionaryType.IsAbstract))
            {
                dictionaryType = typeof(Dictionary<TKey, TValue>);
            }
            var res = Activator.CreateInstance(dictionaryType) as TDictionary;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var keyValues = value.Split(new[] { KeyValueSeparator }, StringSplitOptions.None).ToArray();
                if (keyValues.Length != 2)
                {
                    throw new InvalidDataException("The value is not key|value dictionary string.");
                }

                var keys = KeyCollectionValueConverter.ConvertFromProvider(keyValues[0]);
                var values = ValueCollectionValueConverter.ConvertFromProvider(keyValues[1]);
                if (keys.Count != values.Count)
                {
                    throw new InvalidDataException("The dictionary string value has different number of 'key' and 'value' values.");
                }

                for (var i = 0; i < keys.Count; i++)
                {
                    res.Add(keys[i], values[i]);
                }
            }

            return res;
        }

        private static string ConvertToString(TDictionary dictionary)
        {
            if (!dictionary.Any())
            {
                return string.Empty;
            }

            var keys = KeyCollectionValueConverter.ConvertToProvider(dictionary.Keys.ToList());
            var values = ValueCollectionValueConverter.ConvertToProvider(dictionary.Values.ToList());

            return $"{keys}{KeyValueSeparator}{values}";
        }

        public StringDictionaryValueConverter(ConverterMappingHints mappingHints = null) :
            base(
                toValue => ConvertToString(toValue),
                fromValue => ConvertToDictionary(fromValue),
                mappingHints)
        {
        }
    }
}
