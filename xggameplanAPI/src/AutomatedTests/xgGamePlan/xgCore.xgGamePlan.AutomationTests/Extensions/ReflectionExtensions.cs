using System;
using System.Linq.Expressions;
using System.Reflection;

namespace xgCore.xgGamePlan.AutomationTests.Extensions
{
    public static class ReflectionExtensions
    {
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

        public static void SetMemberValue<T, TResult>(this T instance, Expression<Func<T, TResult>> memberExpression, TResult value)
            where T : class
        {
            if (!IsParameterMemberExpression(memberExpression, MemberTypes.Property | MemberTypes.Field))
            {
                throw new ArgumentException(
                    $"The specified parameter expression should be property or field of the '{typeof(T).Name}' instance type.",
                    nameof(memberExpression));
            }

            var memberInfo = GetMemberInfo(memberExpression);
            if (memberInfo is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(instance, value);
            }
            else
            if (memberInfo is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(instance, value);
            }
        }
    }
}
