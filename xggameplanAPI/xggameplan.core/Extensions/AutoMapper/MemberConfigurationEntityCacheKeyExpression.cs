using System;
using System.Linq.Expressions;
using AutoMapper;

namespace xggameplan.core.Extensions.AutoMapper
{
    public class MemberConfigurationEntityCacheKeyExpression<TSource, TDestination, TMember, TKey>
    {
        private readonly IMemberConfigurationExpression<TSource, TDestination, TMember> _memberConfigurationExpression;
        private readonly Expression<Func<TSource, TKey>> _sourceKeyMember;

        public MemberConfigurationEntityCacheKeyExpression(
            IMemberConfigurationExpression<TSource, TDestination, TMember> memberConfigurationExpression,
            Expression<Func<TSource, TKey>> sourceKeyMember)
        {
            _memberConfigurationExpression = memberConfigurationExpression;
            _sourceKeyMember = sourceKeyMember;
        }

        public MemberConfigurationEntityCacheExpression<TSource, TDestination, TMember, TKey, TEntity> Entity<TEntity>(
            Expression<Func<TEntity, TMember>> entityValueExpression) where TEntity : class
        {
            var resolver =
                new EntityCacheMemberValueResolver<TSource, TDestination, TKey, TMember, TEntity>(
                    entityValueExpression);

            _memberConfigurationExpression.MapFrom(resolver, _sourceKeyMember);

            return new MemberConfigurationEntityCacheExpression<TSource, TDestination, TMember, TKey, TEntity>(
                resolver);
        }

        public MemberConfigurationEntityCacheExpression<TSource, TDestination, TMember, TKey, TEntity> Entity<TEntity>()
            where TEntity : class, TMember
        {
            var resolver =
                new EntityCacheMemberValueResolver<TSource, TDestination, TKey, TMember, TEntity>(x => x);

            _memberConfigurationExpression.MapFrom(resolver, _sourceKeyMember);

            return new MemberConfigurationEntityCacheExpression<TSource, TDestination, TMember, TKey, TEntity>(
                resolver);
        }
    }
}
