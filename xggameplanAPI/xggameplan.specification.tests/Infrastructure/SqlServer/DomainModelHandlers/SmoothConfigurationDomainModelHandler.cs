using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.specification.tests.Interfaces;
using SmoothConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothConfiguration;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SmoothConfigurationDomainModelHandler : SimpleDomainModelMappingHandler<SmoothConfigurationEntity, SmoothDiagnosticConfiguration>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public SmoothConfigurationDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override SmoothConfigurationEntity MapToEntity(SmoothDiagnosticConfiguration model) =>
            Mapper.Map<SmoothConfigurationEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SmoothConfigurationEntity> MapToEntity(IEnumerable<SmoothDiagnosticConfiguration> models) =>
            Mapper.Map<IEnumerable<SmoothConfigurationEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SmoothDiagnosticConfiguration> MapToModel(IEnumerable<SmoothConfigurationEntity> entities) =>
            Mapper.Map<IEnumerable<SmoothDiagnosticConfiguration>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
