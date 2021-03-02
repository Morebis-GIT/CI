using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using ISRSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings.ISRSettings;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class ISRSettingsDomainModelHandler : SimpleDomainModelMappingHandler<ISRSettingsEntity, ISRSettings>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public ISRSettingsDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override ISRSettingsEntity MapToEntity(ISRSettings model) =>
            Mapper.Map<ISRSettingsEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<ISRSettingsEntity> MapToEntity(IEnumerable<ISRSettings> models) =>
            Mapper.Map<IEnumerable<ISRSettingsEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<ISRSettings> MapToModel(IEnumerable<ISRSettingsEntity> entities) =>
            Mapper.Map<IEnumerable<ISRSettings>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
