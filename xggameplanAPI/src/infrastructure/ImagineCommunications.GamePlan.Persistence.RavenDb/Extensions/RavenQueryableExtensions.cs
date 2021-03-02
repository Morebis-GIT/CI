using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using Raven.Client.Linq;
using xggameplan.core.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions
{
    public static class RavenQueryableExtensions
    {
        public static IRavenQueryable<T> ApplyDefaultPaging<T>(this IRavenQueryable<T> queryable, int? skip, int? take)
        {
            return (IRavenQueryable<T>)queryable.AsQueryable().ApplyDefaultPaging(skip, take);
        }

        public static IRavenQueryable<T> OrderByMultipleItems<T>(this IRavenQueryable<T> query, IList<Order<string>> orderByExpressions)
        {
            return (IRavenQueryable<T>) query.AsQueryable().OrderByMultipleItems(orderByExpressions);
        }
    }
}
