using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using xggameplan.Common;

namespace xggameplan.core.Extensions
{
    public static class EnumerableExtensions
    {
        public static int DefaultNumberOfRecordsToTake => 20;
        public static int DefaultNumberOfRecordsToSkip => 0;

        private static Func<T, object> GetSortFunction<T>(string orderBy)
        {
            var param = Expression.Parameter(typeof(T), "item");
            return Expression.Lambda<Func<T, object>>
            (
                Expression.Convert
                (
                    Expression.PropertyOrField(param, orderBy),
                    typeof(object)
                ),
                param
            ).Compile();
        }

        public static IEnumerable<T> OrderByDateTimeSingleItem<T>(this IEnumerable<T> source, string orderBy, OrderDirection orderDirection)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var sortFunc = GetSortFunction<T>(orderBy);

            switch (orderDirection)
            {
                case OrderDirection.Asc:
                    return source.OrderBy(sortFunc, new DateTimeComparer());
                case OrderDirection.Desc:
                    return source.OrderByDescending(sortFunc, new DateTimeComparer());
                default:
                    return source;
            }
        }

        public static IEnumerable<T> OrderByDoubleSingleItem<T>(this IEnumerable<T> query, string orderBy, OrderDirection orderDirection)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return query;
            }

            var sortFunc = GetSortFunction<T>(orderBy);

            switch (orderDirection)
            {
                case OrderDirection.Asc:
                    return query.OrderBy(sortFunc, new DoubleComparer());
                case OrderDirection.Desc:
                    return query.OrderByDescending(sortFunc, new DoubleComparer());
                default:
                    return query;
            }
        }

        public static IEnumerable<T> OrderBySingleItem<T>(this IEnumerable<T> source, string orderBy, OrderDirection orderDirection)
        {
            if (orderBy == null || string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            return OrderBySingleItem(source,
                new Order<string>
                {
                    OrderBy = orderBy,
                    OrderDirection = orderDirection
                });
        }

        public static IEnumerable<T> OrderBySingleItem<T>(this IEnumerable<T> source, Order<string> orderByExpression)
        {
            if (orderByExpression == null || string.IsNullOrWhiteSpace(orderByExpression.OrderBy))
            {
                return source;
            }

            return OrderByMultipleItems(source,
                new List<Order<string>>
                {
                    orderByExpression
                });
        }

        public static IEnumerable<T> OrderByMultipleItems<T>(this IEnumerable<T> source, IList<Order<string>> orderByExpressions)
        {
            if (orderByExpressions == null)
            {
                return source;
            }

            IOrderedEnumerable<T> orderedEnumerable = null;
            for (int i = 0; i < orderByExpressions.Count; i++)
            {
                var sortFunc = GetSortFunction<T>(orderByExpressions[i].OrderBy);

                switch (orderByExpressions[i].OrderDirection)
                {
                    case OrderDirection.Asc:
                        orderedEnumerable = (i == 0) ? source.OrderBy(sortFunc) : orderedEnumerable?.ThenBy(sortFunc);
                        break;

                    default:
                        orderedEnumerable = (i == 0) ? source.OrderByDescending(sortFunc) : orderedEnumerable?.ThenByDescending(sortFunc);
                        break;
                }
            }

            return orderedEnumerable ?? source;
        }

        public static IEnumerable<T> OrderByAlphaNumericSingleItem<T>(this IEnumerable<T> source, string orderBy, OrderDirection orderDirection)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var sortFunc = GetSortFunction<T>(orderBy);

            switch (orderDirection)
            {
                case OrderDirection.Asc:
                    return source.OrderBy(sortFunc, new AlphaNumericComparer());
                case OrderDirection.Desc:
                    return source.OrderByDescending(sortFunc, new AlphaNumericComparer());
                default:
                    return source;
            }
        }

        public static IEnumerable<T> ApplyDefaultPaging<T>(this IEnumerable<T> list, int? skip, int? take)
        {
            return list.Skip(skip ?? DefaultNumberOfRecordsToSkip)
                       .Take(take ?? DefaultNumberOfRecordsToTake);
        }

        public static IEnumerable<T> ApplyPaging<T>(this IEnumerable<T> source, int? skip, int? take)
        {
            if (skip.HasValue)
            {
                source = source.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                source = source.Take(take.Value);
            }

            return source;
        }
    }
}
