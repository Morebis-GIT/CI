using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class KPIComparisonConfigRepository : IKPIComparisonConfigRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public KPIComparisonConfigRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<KPIComparisonConfig> GetAll() =>
            _dbContext.Query<Entities.Tenant.KPIComparisonConfig>().ProjectTo<KPIComparisonConfig>(_mapper.ConfigurationProvider).ToList();
    }
}
