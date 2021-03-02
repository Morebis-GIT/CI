using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.Extensions
{
    public static class GenericSort
    {
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source, string fieldName, string sortDirection, IComparer<object> comparer = null)
        {
            Func<T, object> orderByExpression = item => item.GetType()
                   .GetProperty(fieldName)
                   .GetValue(item, null);
            return sortDirection.ToLower() == "asc" ? source.OrderBy(orderByExpression, comparer) : source.OrderByDescending(orderByExpression, comparer);
        }

        /// <summary>
        /// 1. The sortExpressions is a list of Tuples, the first item of the
        /// tuples is the field name, the second item of the tuples is the
        /// sorting order (asc/desc) case sensitive. the third item of the
        /// tuples is custom Comparer
        /// 2. If the field name (case sensitive) provided for sorting does not exist
        /// in the object, exception is thrown
        /// 3. If a property name shows up more than once in the "sortExpressions",
        /// only the first takes effect. </summary> <typeparam
        /// name="T"></typeparam> <param name="data"></param> <param
        /// name="sortExpressions"></param> <returns></returns>List<(string,
        /// string, IComparer<object>)>
        public static IEnumerable<T> MultipleSort<T>(this IEnumerable<T> data,
           List<(string, string, IComparer<object>)> sortExpressions)
        {
            // No sorting needed
            if ((sortExpressions == null) || (sortExpressions.Count == 0))
            {
                return data;
            }

            // Let us sort it
            IOrderedEnumerable<T> orderedQuery = null;
            for (int i = 0; i < sortExpressions.Count; i++)
            {
                // We need to keep the loop index, not sure why it is altered by
                // the Linq.
                var index = i;
                var currentSortOption = sortExpressions[index];
                Func<T, object> expression = item => item.GetType()
                    .GetProperty(currentSortOption.Item1)
                    .GetValue(item, null);

                if (currentSortOption.Item2 == "asc")
                {
                    orderedQuery = (index == 0)
                        ? data.OrderBy(expression, currentSortOption.Item3)
                        : orderedQuery.ThenBy(expression, currentSortOption.Item3);
                }
                else
                {
                    orderedQuery = (index == 0)
                        ? data.OrderByDescending(expression, currentSortOption.Item3)
                        : orderedQuery.ThenByDescending(expression, currentSortOption.Item3);
                }
            }
            return orderedQuery;
        }
    }
}
