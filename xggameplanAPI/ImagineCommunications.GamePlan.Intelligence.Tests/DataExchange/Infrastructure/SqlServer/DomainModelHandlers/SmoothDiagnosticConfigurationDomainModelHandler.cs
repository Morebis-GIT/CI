using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using SmoothDiagnosticConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothDiagnosticConfiguration;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class SmoothDiagnosticConfigurationDomainModelHandler : SimpleDomainModelMappingHandler<SmoothDiagnosticConfigurationEntity, SmoothDiagnosticConfiguration>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public SmoothDiagnosticConfigurationDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override SmoothDiagnosticConfigurationEntity MapToEntity(SmoothDiagnosticConfiguration model) =>
            Mapper.Map<SmoothDiagnosticConfigurationEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SmoothDiagnosticConfigurationEntity> MapToEntity(IEnumerable<SmoothDiagnosticConfiguration> models) =>
            Mapper.Map<IEnumerable<SmoothDiagnosticConfigurationEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<SmoothDiagnosticConfiguration> MapToModel(IEnumerable<SmoothDiagnosticConfigurationEntity> entities) =>
            Mapper.Map<IEnumerable<SmoothDiagnosticConfiguration>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
