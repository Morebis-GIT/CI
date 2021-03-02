using System;
using System.Linq.Expressions;
using System.Reflection;
using xggameplan.common.Extensions;

namespace xggameplan.core.Extensions.AutoMapper
{
    public class MemberConfigurationEntityCacheExpression<TSource, TDestination, TMember, TKey, TEntity>
        where TEntity : class
    {
        private readonly EntityCacheMemberValueResolver<TSource, TDestination, TKey, TMember, TEntity> _resolver;

        public MemberConfigurationEntityCacheExpression(
            EntityCacheMemberValueResolver<TSource, TDestination, TKey, TMember, TEntity> resolver)
        {
            _resolver = resolver;
        }

        public void CheckNavigationPropertyFirst(Expression<Func<TSource, TEntity>> navigationPropertyMember)
        {
            if (!navigationPropertyMember.IsParameterMemberExpression(MemberTypes.Property))
            {
                throw new ArgumentException($"Expression must be a property.", nameof(navigationPropertyMember));
            }

            _resolver.NavigationPropertyAccessor = () => navigationPropertyMember;
        }
    }
}
