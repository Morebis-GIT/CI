using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using SponsorshipEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.Sponsorship;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SponsorshipDomainModelHandler : SimpleDomainModelMappingHandler<SponsorshipEntity, Sponsorship>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public SponsorshipDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override SponsorshipEntity MapToEntity(Sponsorship model) =>
            Mapper.Map<SponsorshipEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SponsorshipEntity> MapToEntity(IEnumerable<Sponsorship> models) =>
            Mapper.Map<IEnumerable<SponsorshipEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Sponsorship> MapToModel(IEnumerable<SponsorshipEntity> entities) =>
            Mapper.Map<IEnumerable<Sponsorship>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
