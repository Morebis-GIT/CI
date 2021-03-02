using System;
using System.Linq.Expressions;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportHelper
{
    public class ReportHelper
    {
        public static Func<TMember, string> CreateFormatFunc<TMember>(TMember value, Expression expression)
        {
            var parameter = Expression.Parameter(typeof(TMember), "value");
            Expression expr = Expression.Invoke(expression, parameter);

            var lambda = Expression.Lambda<Func<TMember, string>>(expr, parameter);
            Func<TMember, string> compiled = lambda.Compile();
            return compiled;
        }
    }
}
