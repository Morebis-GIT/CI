using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class KPIPriorityRepository : IKPIPriorityRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public KPIPriorityRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<KPIPriority> GetAll()
        {
            return _dbContext
                .Query<Entities.Tenant.BRS.KPIPriority>()
                .ProjectTo<KPIPriority>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}
