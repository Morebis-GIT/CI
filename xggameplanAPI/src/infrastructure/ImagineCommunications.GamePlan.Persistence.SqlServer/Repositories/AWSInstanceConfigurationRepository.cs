using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AWSInstanceConfigurationRepository : IAWSInstanceConfigurationRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AWSInstanceConfigurationRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(IEnumerable<AWSInstanceConfiguration> items)
        {
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                _mapper.Map<List<Entities.Tenant.AWSInstanceConfiguration>>(items));
        }

        public List<AWSInstanceConfiguration> GetAll()
        {
            return _dbContext.Query<Entities.Tenant.AWSInstanceConfiguration>()
                .ProjectTo<AWSInstanceConfiguration>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public AWSInstanceConfiguration Get(int id)
        {
            return _mapper.Map<AWSInstanceConfiguration>(_dbContext.Find<Entities.Tenant.AWSInstanceConfiguration>(id));
        }        

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
