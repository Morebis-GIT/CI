using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using KPIComparisonConfigEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.KPIComparisonConfig;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class KPIComparisonConfigDomainModelHandler : IDomainModelHandler<KPIComparisonConfig>
    {
        private readonly IKPIComparisonConfigRepository _kpiComparisonConfigRepository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public KPIComparisonConfigDomainModelHandler(
            IKPIComparisonConfigRepository kpiComparisonConfigRepository,
            ISqlServerTenantDbContext dbContext,
            IMapper mapper)
        {
            _kpiComparisonConfigRepository =
                kpiComparisonConfigRepository ?? throw new ArgumentNullException(nameof(kpiComparisonConfigRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public KPIComparisonConfig Add(KPIComparisonConfig model)
        {
            _ = _dbContext.Add(_mapper.Map<KPIComparisonConfigEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params KPIComparisonConfig[] models) =>
            _dbContext.AddRange(_mapper.Map<KPIComparisonConfigEntity[]>(models), post => post.MapToCollection(models), _mapper);

        public int Count() => _dbContext.Query<KPIComparisonConfigEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<KPIComparisonConfigEntity>();

        public IEnumerable<KPIComparisonConfig> GetAll() => _kpiComparisonConfigRepository.GetAll();
    }
}
