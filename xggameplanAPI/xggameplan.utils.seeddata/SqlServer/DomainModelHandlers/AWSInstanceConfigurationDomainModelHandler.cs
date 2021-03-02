using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AWSInstanceConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AWSInstanceConfiguration;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AWSInstanceConfigurationDomainModelHandler : IDomainModelHandler<AWSInstanceConfiguration>
    {
        private readonly IAWSInstanceConfigurationRepository _awsInstanceConfigurationRepository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AWSInstanceConfigurationDomainModelHandler(
            IAWSInstanceConfigurationRepository awsInstanceConfigurationRepository,
            ISqlServerTenantDbContext dbContext,
            IMapper mapper)
        {
            _awsInstanceConfigurationRepository = awsInstanceConfigurationRepository ??
                throw new ArgumentNullException(nameof(awsInstanceConfigurationRepository));

            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public AWSInstanceConfiguration Add(AWSInstanceConfiguration model)
        {
            _ = _dbContext.Add(_mapper.Map<AWSInstanceConfigurationEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params AWSInstanceConfiguration[] models)
        {
            _awsInstanceConfigurationRepository.Add(models);
        }

        public int Count()
        {
            return _awsInstanceConfigurationRepository.GetAll().Count;
        }

        public void DeleteAll()
        {
            _dbContext.Truncate<AWSInstanceConfigurationEntity>();
        }

        public IEnumerable<AWSInstanceConfiguration> GetAll() => _awsInstanceConfigurationRepository.GetAll();
    }
}
