using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class PassDomainModelHandler : SimpleDomainModelMappingHandler<PassEntity, Pass>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public PassDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override PassEntity MapToEntity(Pass model) =>
            Mapper.Map<PassEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<PassEntity> MapToEntity(IEnumerable<Pass> models) =>
            Mapper.Map<IEnumerable<PassEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Pass> MapToModel(IEnumerable<PassEntity> entities) =>
            Mapper.Map<IEnumerable<Pass>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
