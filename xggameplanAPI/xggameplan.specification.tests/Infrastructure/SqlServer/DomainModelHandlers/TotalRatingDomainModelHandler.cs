using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using TotalRatingEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TotalRating;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class TotalRatingDomainModelHandler : SimpleDomainModelMappingHandler<TotalRatingEntity, TotalRating>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public TotalRatingDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper) : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override TotalRatingEntity MapToEntity(TotalRating model) =>
            Mapper.Map<TotalRatingEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<TotalRatingEntity> MapToEntity(IEnumerable<TotalRating> models) =>
            Mapper.Map<IEnumerable<TotalRatingEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<TotalRating> MapToModel(IEnumerable<TotalRatingEntity> entities) =>
            Mapper.Map<IEnumerable<TotalRating>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
