using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SmoothConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothConfiguration;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SmoothConfigurationDomainModelHandler : IDomainModelHandler<SmoothConfiguration>
    {
        private readonly ISmoothConfigurationRepository _smoothConfigurationRepository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public SmoothConfigurationDomainModelHandler(
            ISmoothConfigurationRepository smoothConfigurationRepository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _smoothConfigurationRepository = smoothConfigurationRepository ??
                                       throw new ArgumentNullException(nameof(smoothConfigurationRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public SmoothConfiguration Add(SmoothConfiguration model)
        {
            _smoothConfigurationRepository.Add(model);
            return model;
        }

        public void AddRange(params SmoothConfiguration[] models)
        {
            foreach (var smoothConfiguration in models)
            {
                _ = Add(smoothConfiguration);
            }
        }

        public int Count() => _dbContext.Query<SmoothConfigurationEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<SmoothConfigurationEntity>();

        public IEnumerable<SmoothConfiguration> GetAll() =>
            _dbContext.Query<SmoothConfigurationEntity>().ProjectTo<SmoothConfiguration>(_mapper.ConfigurationProvider).ToList();
    }
}
