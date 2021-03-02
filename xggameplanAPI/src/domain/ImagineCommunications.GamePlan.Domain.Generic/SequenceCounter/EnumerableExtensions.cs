using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Applies the specified <see cref="counter"/> instance to each element of <see cref="IEnumerable{T}"/>
        /// </summary>
        public static IEnumerable<T> SequentiallyCount<T>(this IEnumerable<T> enumerable, ISequenceCounter<T> counter)
            where T : class, ICounterValue
        {
            if (enumerable is null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (counter is null)
            {
                throw new ArgumentNullException(nameof(counter));
            }

            foreach (var item in enumerable)
            {
                counter.Process(item);
                yield return item;
            }
        }
    }
}
