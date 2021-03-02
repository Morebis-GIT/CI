using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using CampaignEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class CampaignDomainModelHandler : SimpleDomainModelMappingHandler<CampaignEntity, Campaign>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public CampaignDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override CampaignEntity MapToEntity(Campaign model) =>
            Mapper.Map<CampaignEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<CampaignEntity> MapToEntity(IEnumerable<Campaign> models) =>
            Mapper.Map<IEnumerable<CampaignEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Campaign> MapToModel(IEnumerable<CampaignEntity> entities) =>
            Mapper.Map<IEnumerable<Campaign>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
