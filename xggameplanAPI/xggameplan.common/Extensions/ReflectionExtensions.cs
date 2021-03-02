using System;
using System.Linq.Expressions;
using System.Reflection;

namespace xggameplan.common.Extensions
{
    public static class ReflectionExtensions
    {
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression, bool genericDefinition = true)
        {
            MethodInfo res = null;
            if (expression.Body is MethodCallExpression methodCallExpression)
            {
                res = methodCallExpression.Method;
                if (genericDefinition && res.IsGenericMethod && !res.IsGenericMethodDefinition)
                {
                    res = res.GetGenericMethodDefinition();
                }
            }
            return res;
        }

        public static bool IsParameterMemberExpression(this Expression expression, MemberTypes? memberType = null)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                expression = lambdaExpression.Body;
            }

            return expression != null && expression is MemberExpression memberExpression &&
                   (memberType == null || (memberExpression.Member.MemberType | memberType) != 0) &&
                   memberExpression.Expression is ParameterExpression;
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            if (expression != null)
            {
                if (expression is LambdaExpression lambdaExpression)
                {
                    expression = lambdaExpression.Body;
                }

                if (expression is MemberExpression memberExpression)
                {
                    return memberExpression.Member;
                }
            }
            return null;
        }
    }
}
