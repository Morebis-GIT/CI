using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.MethodRewriters;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions
{
    
    public static class DbSearchExtenstions
    {
        public static IQueryable<T> MakeCaseInsensitive<T>(this IQueryable<T> query)
            => query.Provider.CreateQuery<T>(new StringMethodRewriter()
                .Rewrite(query.Expression));

        public static IQueryable<T> MakeContainsAll<T>(this IQueryable<T> query)
            => query.Provider.CreateQuery<T>(new ContainsAllMethodRewriter()
                .Rewrite(query.Expression));

        public static IQueryable<T> MakeContainsAny<T>(this IQueryable<T> query)
            => query.Provider.CreateQuery<T>(new ContainsAnyMethodRewriter()
                .Rewrite(query.Expression));
    }

}
