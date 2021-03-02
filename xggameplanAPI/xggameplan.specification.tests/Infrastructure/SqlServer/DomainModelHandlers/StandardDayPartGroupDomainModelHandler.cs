using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using StandardDayPartGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPartGroup;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class StandardDayPartGroupDomainModelHandler : SimpleDomainModelMappingHandler<StandardDayPartGroupEntity, StandardDayPartGroup>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public StandardDayPartGroupDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override StandardDayPartGroupEntity MapToEntity(StandardDayPartGroup model) =>
            Mapper.Map<StandardDayPartGroupEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<StandardDayPartGroupEntity> MapToEntity(IEnumerable<StandardDayPartGroup> models) =>
            Mapper.Map<IEnumerable<StandardDayPartGroupEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<StandardDayPartGroup> MapToModel(IEnumerable<StandardDayPartGroupEntity> entities) =>
            Mapper.Map<IEnumerable<StandardDayPartGroup>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
