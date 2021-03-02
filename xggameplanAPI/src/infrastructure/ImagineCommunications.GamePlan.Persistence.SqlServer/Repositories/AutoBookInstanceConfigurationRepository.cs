using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using AutoBookInstanceConfigurationCriteria = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookInstanceConfigurationCriteria;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutoBookInstanceConfigurationRepository : IAutoBookInstanceConfigurationRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AutoBookInstanceConfigurationRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(IEnumerable<AutoBookInstanceConfiguration> instanceConfigurations)=>
            _dbContext.AddRange(_mapper.Map<IEnumerable<Entities.Tenant.AutoBookApi.AutoBookInstanceConfiguration>>(instanceConfigurations).ToArray());

        public AutoBookInstanceConfiguration Get(int id) =>
            _dbContext.Query<Entities.Tenant.AutoBookApi.AutoBookInstanceConfiguration>()
                .ProjectTo<AutoBookInstanceConfiguration>(_mapper.ConfigurationProvider).FirstOrDefault(x => x.Id == id);

        public List<AutoBookInstanceConfiguration> GetAll() => _dbContext
            .Query<Entities.Tenant.AutoBookApi.AutoBookInstanceConfiguration>()
            .ProjectTo<AutoBookInstanceConfiguration>(_mapper.ConfigurationProvider)
            .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
