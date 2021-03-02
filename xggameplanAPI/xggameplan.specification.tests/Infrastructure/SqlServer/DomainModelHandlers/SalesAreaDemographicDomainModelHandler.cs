using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using SalesAreaDemographicEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesAreaDemographic;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SalesAreaDemographicDomainModelHandler : SimpleDomainModelMappingHandler<SalesAreaDemographicEntity, SalesAreaDemographic>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public SalesAreaDemographicDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper) : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override SalesAreaDemographicEntity MapToEntity(SalesAreaDemographic model) =>
            Mapper.Map<SalesAreaDemographicEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SalesAreaDemographicEntity> MapToEntity(IEnumerable<SalesAreaDemographic> models) =>
            Mapper.Map<IEnumerable<SalesAreaDemographicEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SalesAreaDemographic> MapToModel(IEnumerable<SalesAreaDemographicEntity> entities) =>
            Mapper.Map<IEnumerable<SalesAreaDemographic>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
