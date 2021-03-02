using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.ValueConverters
{
    internal class StringCollectionValueConverter<TCollection, T> : ValueConverter<TCollection, string>
        where TCollection : class, ICollection<T>
        where T : IConvertible
    {
        public const string ItemSeparator = "‖";

        private static TCollection ConvertToCollection(string value)
        {
            var collectionType = typeof(TCollection);
            if (!(collectionType.IsClass && !collectionType.IsAbstract))
            {
                collectionType = typeof(List<T>);
            }
            var res = Activator.CreateInstance(collectionType) as TCollection;

            if (!string.IsNullOrWhiteSpace(value))
            {
                foreach (var item in value.Split(new[] { ItemSeparator }, StringSplitOptions.None)
                    .Select(x => (T)((IConvertible)x).ToType(typeof(T), CultureInfo.InvariantCulture)))
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private static string ConvertToString(TCollection collection)
        {
            return collection?.Select(x => x.ToString(CultureInfo.InvariantCulture)).Join(ItemSeparator) ??
                string.Empty;
        }

        public StringCollectionValueConverter(ConverterMappingHints mappingHints = null) :
            base(toValue => ConvertToString(toValue),
                fromValue => ConvertToCollection(fromValue),
                mappingHints)
        {
        }

        public string ConvertToProvider(TCollection collection) => ConvertToString(collection);
        public TCollection ConvertFromProvider(string value) => ConvertToCollection(value);
    }
}
