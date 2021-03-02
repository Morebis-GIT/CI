using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class RSSettingsDomainModelHandler : SimpleDomainModelMappingHandler<RSSettingsEntity, RSSettings>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public RSSettingsDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper) : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override RSSettingsEntity MapToEntity(RSSettings model) =>
            Mapper.Map<RSSettingsEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<RSSettingsEntity> MapToEntity(IEnumerable<RSSettings> models) =>
            Mapper.Map<IEnumerable<RSSettingsEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<RSSettings> MapToModel(IEnumerable<RSSettingsEntity> entities) =>
            Mapper.Map<IEnumerable<RSSettings>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
