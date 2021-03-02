using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using BreakEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks.Break;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class BreakDomainModelHandler : SimpleDomainModelMappingHandler<BreakEntity, Break>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public BreakDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override BreakEntity MapToEntity(Break model) =>
            Mapper.Map<BreakEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<BreakEntity> MapToEntity(IEnumerable<Break> models) =>
            Mapper.Map<IEnumerable<BreakEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Break> MapToModel(IEnumerable<BreakEntity> entities) =>
            Mapper.Map<IEnumerable<Break>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
