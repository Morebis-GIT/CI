using System.Collections.Generic;

namespace xggameplan.common.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfNotNull<T>(this IList<T> list, T value)
        {
            if (value != null)
            {
                list.Add(value);
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> values)
        {
            if (list is null)
            {
                return;
            }

            foreach (var value in values)
            {
                list.Add(value);
            }
        }
    }
}
