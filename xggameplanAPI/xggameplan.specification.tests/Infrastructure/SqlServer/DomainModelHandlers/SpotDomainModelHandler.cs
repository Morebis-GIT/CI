using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using SpotEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Spot;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SpotDomainModelHandler : SimpleDomainModelMappingHandler<SpotEntity, Spot>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public SpotDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override SpotEntity MapToEntity(Spot model) =>
            Mapper.Map<SpotEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SpotEntity> MapToEntity(IEnumerable<Spot> models) =>
            Mapper.Map<IEnumerable<SpotEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Spot> MapToModel(IEnumerable<SpotEntity> entities) =>
            Mapper.Map<IEnumerable<Spot>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
