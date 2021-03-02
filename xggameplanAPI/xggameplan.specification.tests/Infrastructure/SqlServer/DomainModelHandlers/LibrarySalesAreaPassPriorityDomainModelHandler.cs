using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using LibrarySalesAreaPassPrioritiesEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.LibrarySalesAreaPassPriority;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class LibrarySalesAreaPassPriorityDomainModelHandler : SimpleDomainModelMappingHandler<
        LibrarySalesAreaPassPrioritiesEntity, LibrarySalesAreaPassPriority>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public LibrarySalesAreaPassPriorityDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override LibrarySalesAreaPassPrioritiesEntity MapToEntity(LibrarySalesAreaPassPriority model) =>
            Mapper.Map<LibrarySalesAreaPassPrioritiesEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<LibrarySalesAreaPassPrioritiesEntity> MapToEntity(
            IEnumerable<LibrarySalesAreaPassPriority> models) =>
            Mapper.Map<IEnumerable<LibrarySalesAreaPassPrioritiesEntity>>(models,
                opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<LibrarySalesAreaPassPriority> MapToModel(
            IEnumerable<LibrarySalesAreaPassPrioritiesEntity> entities) =>
            Mapper.Map<IEnumerable<LibrarySalesAreaPassPriority>>(entities,
                opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
