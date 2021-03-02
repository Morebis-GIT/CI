using System;
using System.Linq.Expressions;
using AutoMapper;

namespace xggameplan.core.Extensions.AutoMapper
{
    public class MappingConfigurationEntityCacheKeyExpression<TSource, TDestination, TKey>
    {
        private readonly IMappingExpression<TSource, TDestination> _mappingExpression;
        private readonly Expression<Func<TSource, TKey>> _sourceKeyMember;

        public MappingConfigurationEntityCacheKeyExpression(
            IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TSource, TKey>> sourceKeyMember)
        {
            _mappingExpression = mappingExpression;
            _sourceKeyMember = sourceKeyMember;
        }

        public MemberConfigurationEntityCacheExpression<TSource, TDestination, TDestination, TKey, TEntity>
            Entity<TEntity>(
                Expression<Func<TEntity, TDestination>> entityValueExpression) where TEntity : class
        {
            var resolver =
                new EntityCacheMemberValueResolver<TSource, TDestination, TKey, TDestination, TEntity>(
                    entityValueExpression);

            var action = _sourceKeyMember.Compile();
            _mappingExpression.ConvertUsing((src, dest, rc) => resolver.Resolve(src, dest, action(src), dest, rc));

            return new MemberConfigurationEntityCacheExpression<TSource, TDestination, TDestination, TKey, TEntity>(
                resolver);
        }

        public MemberConfigurationEntityCacheExpression<TSource, TDestination, TDestination, TKey, TEntity>
            Entity<TEntity>() where TEntity : class, TDestination
        {
            var resolver =
                new EntityCacheMemberValueResolver<TSource, TDestination, TKey, TDestination, TEntity>(
                    x => x);

            var action = _sourceKeyMember.Compile();
            _mappingExpression.ConvertUsing((src, dest, rc) => resolver.Resolve(src, dest, action(src), dest, rc));

            return new MemberConfigurationEntityCacheExpression<TSource, TDestination, TDestination, TKey, TEntity>(
                resolver);
        }
    }
}
