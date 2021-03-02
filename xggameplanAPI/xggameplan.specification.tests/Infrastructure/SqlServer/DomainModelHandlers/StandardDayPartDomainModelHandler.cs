using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using StandardDayPartEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPart;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class StandardDayPartDomainModelHandler : SimpleDomainModelMappingHandler<StandardDayPartEntity, StandardDayPart>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public StandardDayPartDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override StandardDayPartEntity MapToEntity(StandardDayPart model) =>
            Mapper.Map<StandardDayPartEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<StandardDayPartEntity> MapToEntity(IEnumerable<StandardDayPart> models) =>
            Mapper.Map<IEnumerable<StandardDayPartEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<StandardDayPart> MapToModel(IEnumerable<StandardDayPartEntity> entities) =>
            Mapper.Map<IEnumerable<StandardDayPart>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
