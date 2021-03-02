using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using xggameplan.common.Model;
using DynamicExpressionParser = System.Linq.Dynamic.Core.DynamicExpressionParser;

namespace xggameplan.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements,
            string totalcolumn,
            params Func<TElement, object>[] groupSelectors)

        {
            if (groupSelectors.Length == 0)
            {
                return null;
            }

            var selector = groupSelectors[0];

            var nextSelectors = groupSelectors.Skip(1).ToArray();

            return
                elements.GroupBy(selector).Select(
                    g => new GroupResult
                    {
                        Key = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(x => (double)x.GetValue(totalcolumn)),
                        SubGroups = g.GroupByMany(totalcolumn, nextSelectors)
                    });
        }

        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements, string totalcolumn, params string[] groupSelectors)
        {
            var selectors =
                new List<Func<TElement, object>>(groupSelectors.Length);

            foreach (var selector in groupSelectors)
            {
                LambdaExpression l =
                    DynamicExpressionParser.ParseLambda(
                        typeof(TElement), typeof(object), selector);
                selectors.Add((Func<TElement, object>)l.Compile());
            }

            return elements.GroupByMany(totalcolumn, selectors.ToArray());
        }
    }
}
