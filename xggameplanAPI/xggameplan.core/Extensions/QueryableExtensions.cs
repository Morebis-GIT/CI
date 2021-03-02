using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace xggameplan.core.Extensions
{
    public static class QueryableExtensions
    {
        public static int DefaultNumberOfRecordsToTake => 20;
        public static int DefaultNumberOfRecordsToSkip => 0;

        private static Expression<Func<T, object>> GetSortExpression<T>(string orderBy)
        {
            var param = Expression.Parameter(typeof(T), "item");
            var sortExpression = Expression.Lambda<Func<T, object>>
            (
                Expression.Convert
                (
                    Expression.Property(param, orderBy),
                    typeof(object)
                ),
                param
            );

            return sortExpression;
        }

        public static IQueryable<T> OrderBySingleItem<T>(this IQueryable<T> query, string orderBy, OrderDirection orderDirection)
        {
            if (orderBy == null || string.IsNullOrWhiteSpace(orderBy))
            {
                return query;
            }

            return OrderBySingleItem(query,
                new Order<string>
                {
                    OrderBy = orderBy,
                    OrderDirection = orderDirection
                });
        }

        public static IQueryable<T> OrderBySingleItem<T>(this IQueryable<T> query, Order<string> orderByExpression)
        {
            if (orderByExpression == null || string.IsNullOrWhiteSpace(orderByExpression.OrderBy))
            {
                return query;
            }

            return OrderByMultipleItems(query,
                new List<Order<string>>
                {
                    orderByExpression
                });
        }

        public static IQueryable<T> OrderByMultipleItemsWithCommonOrder<T>(this IQueryable<T> query, IList<string> orderBy, OrderDirection orderDirection)
        {
            if (orderBy == null || !orderBy.Any())
            {
                return query;
            }

            if (orderBy.Any(p => string.IsNullOrWhiteSpace(p)))
            {
                throw new ArgumentException("One or more property selectors is(are) invalid", nameof(orderBy));
            }

            return OrderByMultipleItems(query, orderBy.Select(item => new Order<string>
            {
                OrderBy = item,
                OrderDirection = orderDirection
            }).ToList());
        }

        public static IQueryable<T> OrderByMultipleItems<T>(this IQueryable<T> query, IList<Order<string>> orderByExpressions)
        {
            if (orderByExpressions == null)
            {
                return query;
            }

            IOrderedQueryable<T> orderedQuery = null;
            for (int i = 0; i < orderByExpressions.Count; i++)
            {
                var sortExpression = GetSortExpression<T>(orderByExpressions[i].OrderBy);

                switch (orderByExpressions[i].OrderDirection)
                {
                    case OrderDirection.Asc:
                        orderedQuery = (i == 0) ? query.OrderBy(sortExpression) : orderedQuery?.ThenBy(sortExpression);
                        break;

                    default:
                        orderedQuery = (i == 0) ? query.OrderByDescending(sortExpression) : orderedQuery?.ThenByDescending(sortExpression);
                        break;
                }
            }

            return orderedQuery ?? query;
        }

        public static IQueryable<T> ApplyDefaultPaging<T>(this IQueryable<T> queryable, int? skip, int? take)
        {
            return queryable
                .Skip(skip ?? DefaultNumberOfRecordsToSkip)
                .Take(take ?? DefaultNumberOfRecordsToTake);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int? skip, int? take)
        {
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }
    }
}
