using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace xggameplan.core.Extensions.AutoMapper
{
    public class EntityCacheMemberValueResolver<TSource, TDestination, TSourceMember, TDestMember, TEntity> : IMemberValueResolver<TSource, TDestination, TSourceMember, TDestMember>
        where TEntity : class
    {
        private readonly Func<TEntity, TDestMember> _cacheValueGetterDelegate;
        private Func<TSource, TEntity> _navigationPropertyGetter;

        private readonly IDictionary<ResolutionContext, IEntityCacheAccessor<TSourceMember, TEntity>> _cacheAccessors =
            new Dictionary<ResolutionContext, IEntityCacheAccessor<TSourceMember, TEntity>>();

        private IEntityCacheAccessor<TSourceMember, TEntity> GetCacheAccessor(ResolutionContext resolutionContext)
        {
            if (!_cacheAccessors.TryGetValue(resolutionContext, out var cacheAccessor))
            {
                cacheAccessor = EntityCacheResolutionContextHelper.Get<TSourceMember, TEntity>(resolutionContext.Items);

                if (cacheAccessor is null)
                {
                    throw new AutoMapperMappingException(
                        $"{nameof(ResolutionContext)} does not contain '{nameof(IEntityCacheAccessor<TSourceMember, TEntity>)}' entity cache type.");
                }

                _cacheAccessors.Add(resolutionContext, cacheAccessor);
            }

            return cacheAccessor;
        }

        private TEntity GetNavigationPropertyValue(TSource source)
        {
            if (_navigationPropertyGetter is null && !(NavigationPropertyAccessor is null))
            {
                _navigationPropertyGetter = NavigationPropertyAccessor().Compile();
            }

            return _navigationPropertyGetter?.Invoke(source);
        }

        public EntityCacheMemberValueResolver(Expression<Func<TEntity, TDestMember>> cacheValueGetter)
        {
            if (cacheValueGetter is null)
            {
                throw new ArgumentNullException(nameof(cacheValueGetter));
            }

            _cacheValueGetterDelegate = cacheValueGetter.Compile();
        }

        public TDestMember Resolve(TSource source, TDestination destination, TSourceMember sourceMember,
            TDestMember destMember,
            ResolutionContext context)
        {
            var value = GetNavigationPropertyValue(source) ?? GetCacheAccessor(context).Get(sourceMember);

            return value is null ? destMember : _cacheValueGetterDelegate(value);
        }

        public Func<Expression<Func<TSource, TEntity>>> NavigationPropertyAccessor { get; set; }
    }
}
