using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutoBookInstanceConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBookInstanceConfiguration;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AutoBookInstanceConfigurationDomainModelHandler : IDomainModelHandler<AutoBookInstanceConfiguration>
    {
        private readonly IAutoBookInstanceConfigurationRepository _autoBookInstanceConfigurationRepository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AutoBookInstanceConfigurationDomainModelHandler(
            IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository,
            ISqlServerTenantDbContext dbContext,
            IMapper mapper)
        {
            _autoBookInstanceConfigurationRepository = autoBookInstanceConfigurationRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public AutoBookInstanceConfiguration Add(AutoBookInstanceConfiguration model)
        {
            _autoBookInstanceConfigurationRepository.Add(new[] {model});
            return model;
        }

        public void AddRange(params AutoBookInstanceConfiguration[] models) =>
            _autoBookInstanceConfigurationRepository.Add(models);

        public int Count() => _dbContext.Query<AutoBookInstanceConfigurationEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<AutoBookInstanceConfigurationEntity>();

        public IEnumerable<AutoBookInstanceConfiguration> GetAll() => _dbContext.Query<AutoBookInstanceConfigurationEntity>()
            .ProjectTo<AutoBookInstanceConfiguration>(_mapper.ConfigurationProvider)
            .ToArray();
    }
}
