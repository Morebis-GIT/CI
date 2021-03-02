using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ScenarioCampaignMetricEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignMetric;


namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ScenarioCampaignMetricRepository : IScenarioCampaignMetricRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioCampaignMetricRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddOrUpdate(ScenarioCampaignMetric scenarioCampaignMetrics)
        {
            List<ScenarioCampaignMetricEntity> externalEntities = _mapper.Map<List<ScenarioCampaignMetricEntity>>(scenarioCampaignMetrics);

            List<ScenarioCampaignMetricEntity> entities = _dbContext.Query<ScenarioCampaignMetricEntity>()
                .Where(e => e.ScenarioId == scenarioCampaignMetrics.Id)
                .ToList();

            if (!entities.Any())
            {
                _dbContext.BulkInsertEngine.BulkInsert(externalEntities, new BulkInsertOptions() { BatchSize = 600000 });
            }
            else
            {
                var addedEntities = externalEntities.Where(x => !entities.Contains(x)).ToArray();
                var deletedEntities = entities.Where(x => !externalEntities.Contains(x)).ToArray();

                _dbContext.RemoveRange(deletedEntities);
                _dbContext.AddRange(addedEntities);
            }
        }

        public void Delete(Guid scenarioId)
        {
            var entities = _dbContext.Query<ScenarioCampaignMetricEntity>().Where(e => e.ScenarioId == scenarioId).ToArray();

            if (entities.Any())
            {
                _dbContext.RemoveRange(entities);
            }
        }

        public ScenarioCampaignMetric Get(Guid scenarioId)
        {
            var entities = _dbContext.Query<ScenarioCampaignMetricEntity>()
                .Where(e => e.ScenarioId == scenarioId)
                .ToList();

            return _mapper.Map<ScenarioCampaignMetric>(entities);
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
