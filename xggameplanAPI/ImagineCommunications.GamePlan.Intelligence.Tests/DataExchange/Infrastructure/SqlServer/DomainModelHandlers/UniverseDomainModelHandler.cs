using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using UniverseEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Universe;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class UniverseDomainModelHandler : SimpleDomainModelMappingHandler<UniverseEntity, Universe>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public UniverseDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override UniverseEntity MapToEntity(Universe model) =>
            Mapper.Map<UniverseEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<UniverseEntity> MapToEntity(IEnumerable<Universe> models) =>
            Mapper.Map<IEnumerable<UniverseEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Universe> MapToModel(IEnumerable<UniverseEntity> entities) =>
            Mapper.Map<IEnumerable<Universe>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
