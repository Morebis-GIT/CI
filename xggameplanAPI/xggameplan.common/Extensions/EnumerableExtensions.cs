using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace xggameplan.common.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ReducePropertyToCsv<T, TResult>(
            this IEnumerable<T> enumerable,
            Expression<Func<T, TResult>> propertyExpression)
            where T : class
        {
            if (propertyExpression is null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (!propertyExpression.IsParameterMemberExpression(MemberTypes.Property | MemberTypes.Field))
            {
                throw new ArgumentException(
                    $"The value should be an expression of a property or field which belongs to the '{typeof(T).Name}' type.",
                    nameof(propertyExpression));
            }

            var compiledDelegate = propertyExpression.Compile();

            return string.Join(", ",
                enumerable?.Where(x => !(x is null)).Select(x => $"{compiledDelegate(x)}") ??
                Enumerable.Empty<string>());
        }
    }
}
