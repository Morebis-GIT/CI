using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using LengthFactorEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.LengthFactor;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class LengthFactorDomainModelHandler : SimpleDomainModelMappingHandler<LengthFactorEntity, LengthFactor>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public LengthFactorDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override LengthFactorEntity MapToEntity(LengthFactor model) =>
            Mapper.Map<LengthFactorEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<LengthFactorEntity> MapToEntity(IEnumerable<LengthFactor> models) =>
            Mapper.Map<IEnumerable<LengthFactorEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<LengthFactor> MapToModel(IEnumerable<LengthFactorEntity> entities) =>
            Mapper.Map<IEnumerable<LengthFactor>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
