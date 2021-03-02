using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions
{
    public static class DocumentQueryExtensions
    {
        public static IDocumentQuery<T> OrderByMultipleItemsWithCommonOrder<T>(this IDocumentQuery<T> query, OrderDirection orderDirection, params string[] propertySelectors)
        {
            if (propertySelectors == null || !propertySelectors.Any())
            {
                return query;
            }

            if (propertySelectors.Any(p => string.IsNullOrWhiteSpace(p)))
            {
                throw new ArgumentException("One or more property selectors is(are) invalid", nameof(propertySelectors));
            }

            query = orderDirection == OrderDirection.Asc
                ? query.OrderBy(propertySelectors)
                : query.OrderByDescending(propertySelectors);

            return query;
        }
    }
}
