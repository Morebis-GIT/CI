using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using xggameplan.specification.tests.Interfaces;
using ScenarioCampaignMetricEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignMetric;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class ScenarioCampaignMetricDomainModelHandler : SimpleDomainModelMappingHandler<ScenarioCampaignMetricEntity, ScenarioCampaignMetric>
    {
        private readonly ITenantDbContext _dbContext;

        public ScenarioCampaignMetricDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override ScenarioCampaignMetric Add(ScenarioCampaignMetric model)
        {
            _dbContext.AddRange(Mapper.Map<List<ScenarioCampaignMetricEntity>>(model));

            return model;
        }

        public override void AddRange(params ScenarioCampaignMetric[] models)
        {
            var entities = models.SelectMany(x => Mapper.Map<List<ScenarioCampaignMetricEntity>>(x)).ToArray();

            _dbContext.AddRange(entities);
        }

        public override IEnumerable<ScenarioCampaignMetric> GetAll() => _dbContext
            .Query<ScenarioCampaignMetricEntity>()
            .GroupBy(e => e.ScenarioId)
            .Select(e => new ScenarioCampaignMetric
            {
                Id = e.Key,
                Metrics = Mapper.Map<List<ScenarioCampaignMetricItem>>(e.ToList())
            })
            .ToArray();
    }
}
