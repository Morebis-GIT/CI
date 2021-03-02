using System;
using System.Linq.Expressions;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace xggameplan.core.Extensions.AutoMapper
{
    public static class EntityCacheAutomapperExtensions
    {
        public static IMappingOperationOptions UseEntityCache<TKey, TEntity>(
            this IMappingOperationOptions options,
            IEntityCacheAccessor<TKey, TEntity> entityCache)
            where TEntity : class
        {
            EntityCacheResolutionContextHelper.Add(options.Items, entityCache);
            return options;
        }

        public static void FromEntityCache<TSource, TDestination, TMember, TKey>(
            this IMemberConfigurationExpression<TSource, TDestination, TMember> memberConfigurationExpression,
            Expression<Func<TSource, TKey>> sourceKeyMember,
            Action<MemberConfigurationEntityCacheKeyExpression<TSource, TDestination, TMember, TKey>> cacheOptions
        )
        {
            if (cacheOptions is null)
            {
                throw new ArgumentNullException(nameof(cacheOptions));
            }

            var cacheKeyExpression = new MemberConfigurationEntityCacheKeyExpression<TSource, TDestination, TMember, TKey>(
                memberConfigurationExpression, sourceKeyMember);

            cacheOptions(cacheKeyExpression);
        }

        public static void FromEntityCache<TSource, TDestination, TMember>(
            this IMemberConfigurationExpression<TSource, TDestination, TMember> memberConfigurationExpression,
            Action<MemberConfigurationEntityCacheKeyExpression<TSource, TDestination, TMember, TSource>> cacheOptions
        )
        {
            FromEntityCache(memberConfigurationExpression, x => x, cacheOptions);
        }

        public static void FromEntityCache<TSource, TDestination, TKey>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TSource, TKey>> sourceKeyMember,
            Action<MappingConfigurationEntityCacheKeyExpression<TSource, TDestination, TKey>> cacheOptions
        )
        {
            if (cacheOptions is null)
            {
                throw new ArgumentNullException(nameof(cacheOptions));
            }

            var cacheKeyExpression =
                new MappingConfigurationEntityCacheKeyExpression<TSource, TDestination, TKey>(mappingExpression, sourceKeyMember);

            cacheOptions(cacheKeyExpression);
        }

        public static void FromEntityCache<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Action<MappingConfigurationEntityCacheKeyExpression<TSource, TDestination, TSource>> cacheOptions
        )
        {
            FromEntityCache(mappingExpression, x => x, cacheOptions);
        }
    }
}
