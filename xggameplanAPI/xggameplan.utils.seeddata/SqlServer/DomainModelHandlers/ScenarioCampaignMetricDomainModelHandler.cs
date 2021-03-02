using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ScenarioCampaignMetricEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignMetric;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ScenarioCampaignMetricDomainModelHandler : IDomainModelHandler<ScenarioCampaignMetric>
    {
        private readonly IScenarioCampaignMetricRepository _repository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioCampaignMetricDomainModelHandler(
            IScenarioCampaignMetricRepository repository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public ScenarioCampaignMetric Add(ScenarioCampaignMetric model)
        {
            _repository.AddOrUpdate(model);

            return model;
        }

        public void AddRange(params ScenarioCampaignMetric[] models)
        {
            foreach (var model in models)
            {
                _repository.AddOrUpdate(model);
            }
        }

        public int Count() => _dbContext
            .Query<ScenarioCampaignMetricEntity>()
            .Count();

        public void DeleteAll() => _dbContext
            .Truncate<ScenarioCampaignMetricEntity>();

        public IEnumerable<ScenarioCampaignMetric> GetAll() => _dbContext
            .Query<ScenarioCampaignMetricEntity>()
            .GroupBy(e => e.ScenarioId)
            .Select(e => new ScenarioCampaignMetric
            {
                Id = e.Key,
                Metrics = _mapper.Map<List<ScenarioCampaignMetricItem>>(e.ToList())
            })
            .ToArray();
    }
}
