using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.ValueConverters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<DateTime> AsUtc(this PropertyBuilder<DateTime> propertyBuilder)
        {
            return propertyBuilder?.HasConversion(
                new ValueConverter<DateTime, DateTime>(
                    dateTo => dateTo,
                    dateFrom => new DateTime(dateFrom.Ticks, DateTimeKind.Utc)));
        }

        public static PropertyBuilder<DateTime?> AsUtc(this PropertyBuilder<DateTime?> propertyBuilder)
        {
            return propertyBuilder?.HasConversion(
                new ValueConverter<DateTime?, DateTime?>(
                    dateTo => dateTo,
                    dateFrom => dateFrom.HasValue
                        ? new DateTime(dateFrom.Value.Ticks, DateTimeKind.Utc)
                        : (DateTime?) null));
        }

        public static PropertyBuilder<TimeSpan> AsTicks(this PropertyBuilder<TimeSpan> propertyBuilder)
        {
            return propertyBuilder?.HasConversion(new ValueConverter<TimeSpan, long>(
                toValue => toValue.Ticks,
                fromValue => TimeSpan.FromTicks(fromValue)));
        }

        public static PropertyBuilder<TimeSpan?> AsTicks(this PropertyBuilder<TimeSpan?> propertyBuilder)
        {
            return propertyBuilder?.HasConversion(new ValueConverter<TimeSpan?, long?>(
                toValue => toValue.HasValue ? toValue.Value.Ticks : (long?) null,
                fromValue => fromValue.HasValue ? TimeSpan.FromTicks(fromValue.Value) : (TimeSpan?) null));
        }

        public static PropertyBuilder<SortedSet<DayOfWeek>> AsStringPattern(this PropertyBuilder<SortedSet<DayOfWeek>> propertyBuilder, DayOfWeek startsFrom)
        {
            return propertyBuilder?.HasMaxLength(7).HasConversion(new ValueConverter<SortedSet<DayOfWeek>, string>(
                toValue => toValue.GetStringDowPattern(startsFrom),
                fromValue => fromValue.GetSortedSetDowPattern(startsFrom)
            ));
        }

        public static PropertyBuilder<ICollection<T>> AsDelimitedString<T>(
            this PropertyBuilder<ICollection<T>> propertyBuilder)
            where T : IConvertible
        {
            return propertyBuilder?.HasConversion(new StringCollectionValueConverter<ICollection<T>, T>());
        }

        public static PropertyBuilder<List<T>> AsDelimitedString<T>(
            this PropertyBuilder<List<T>> propertyBuilder)
            where T : IConvertible
        {
            return propertyBuilder?.HasConversion(new StringCollectionValueConverter<List<T>, T>());
        }

        public static PropertyBuilder<HashSet<T>> AsDelimitedString<T>(
            this PropertyBuilder<HashSet<T>> propertyBuilder)
            where T : IConvertible
        {
            return propertyBuilder?.HasConversion(new StringCollectionValueConverter<HashSet<T>, T>());
        }

        public static PropertyBuilder<IDictionary<TKey, TValue>> AsDelimitedString<TKey, TValue>(this PropertyBuilder<IDictionary<TKey, TValue>> propertyBuilder)
            where TKey : IConvertible
            where TValue : IConvertible
        {
            return propertyBuilder?.HasConversion(
                new StringDictionaryValueConverter<IDictionary<TKey, TValue>, TKey, TValue>());
        }

        public static PropertyBuilder<Dictionary<TKey, TValue>> AsDelimitedString<TKey, TValue>(this PropertyBuilder<Dictionary<TKey, TValue>> propertyBuilder)
            where TKey : IConvertible
            where TValue : IConvertible
        {
            return propertyBuilder?.HasConversion(
                new StringDictionaryValueConverter<Dictionary<TKey, TValue>, TKey, TValue>());
        }
        /// <summary>
        /// Annotation key for marshalling field names to migration 
        /// </summary>
        public const string SearchFieldAnnotationName = "MySql:SearchSourceFields";

        /// <summary>
        /// Creates a property for full text search
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="fieldsList">List of fields which will be cumputed for full-text search</param>
        /// <returns>The same instance of PropertyBuilder for further configurations</returns>
        public static PropertyBuilder<string> AsFts(this PropertyBuilder<string> propertyBuilder, IReadOnlyList<string> fieldsList)
        {
            return propertyBuilder.HasAnnotation(SearchFieldAnnotationName,string.Join(",",fieldsList.ToArray()));
        }

    }
}
