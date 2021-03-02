using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class LandmarkRunQueueRepository : ILandmarkRunQueueRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public LandmarkRunQueueRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<LandmarkRunQueue> GetAll()
        {
            return _dbContext
                .Query<Entities.Tenant.Runs.LandmarkRunQueue>()
                .ProjectTo<LandmarkRunQueue>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}
