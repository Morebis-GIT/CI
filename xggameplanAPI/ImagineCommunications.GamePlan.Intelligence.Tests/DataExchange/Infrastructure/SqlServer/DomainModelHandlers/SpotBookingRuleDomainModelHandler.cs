using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using SpotBookingRuleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules.SpotBookingRule;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SpotBookingRuleDomainModelHandler : SimpleDomainModelMappingHandler<SpotBookingRuleEntity, SpotBookingRule>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public SpotBookingRuleDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override SpotBookingRuleEntity MapToEntity(SpotBookingRule model) =>
            Mapper.Map<SpotBookingRuleEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SpotBookingRuleEntity> MapToEntity(IEnumerable<SpotBookingRule> models) =>
            Mapper.Map<IEnumerable<SpotBookingRuleEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SpotBookingRule> MapToModel(IEnumerable<SpotBookingRuleEntity> entities) =>
            Mapper.Map<IEnumerable<SpotBookingRule>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
