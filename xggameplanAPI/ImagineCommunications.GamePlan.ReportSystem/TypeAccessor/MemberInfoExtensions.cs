using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ImagineCommunications.GamePlan.ReportSystem.TypeAccessor
{
    public static class MemberInfoExtensions
    {
        public static Expression<Func<TSource, TMember>> GetGetAccessor<TSource, TMember>(
            this PropertyInfo propertyInfo,
            bool nonPublic = false)
        {

            var getter = propertyInfo.GetGetMethod(nonPublic);
            if (getter == null || propertyInfo.GetIndexParameters().Any())
            {
                return null;
            }

            var instance = Expression.Parameter(typeof(TSource), "instance");
            var memberExpression = Expression.Property(instance, propertyInfo);

            return Expression.Lambda<Func<TSource, TMember>>(Expression.Convert(memberExpression, typeof(TMember)), instance);
        }

        public static Expression<Func<TSource, TMember>> GetGetAccessor<TSource, TMember>(
            this FieldInfo fieldInfo,
            bool nonPublic = false)
        {
            var instance = Expression.Parameter(typeof(TSource), "instance");
            var memberExpression = Expression.Field(instance, fieldInfo);

            return Expression.Lambda<Func<TSource, TMember>>(Expression.Convert(memberExpression, typeof(TMember)), instance);
        }

        public static Func<TSource, object> GetGetAccessor<TSource>(this PropertyInfo propertyInfo,
            bool includeNonPublic = false)
        {
            return propertyInfo.GetGetAccessor<TSource, object>(includeNonPublic)?.Compile();
        }

        public static Func<TSource, object> GetGetAccessor<TSource>(this FieldInfo propertyInfo,
            bool includeNonPublic = false)
        {
            return propertyInfo.GetGetAccessor<TSource, object>(includeNonPublic)?.Compile();
        }
    }
}
