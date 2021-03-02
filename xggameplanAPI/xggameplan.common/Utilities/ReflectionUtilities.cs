using System;
using System.Linq.Expressions;

namespace xggameplan.common.Utilities
{
    public static class ReflectionUtilities
    {
        public static string PropertyName<T>(Expression<Func<T, object>> expression) where T : class
        {
            if (expression.Body is MemberExpression expressionBody)
            {
                return expressionBody.Member.Name;
            }
            else
            {
                var op = ((UnaryExpression)expression.Body).Operand;
                return ((MemberExpression)op).Member.Name;
            }
        }
    }
}
