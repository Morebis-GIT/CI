using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ScenarioCampaignResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignResult;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ScenarioCampaignResultRepository : IScenarioCampaignResultRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioCampaignResultRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddOrUpdate(ScenarioCampaignResult scenarioCampaignResults)
        {
            List<ScenarioCampaignResultEntity> externalEntities = _mapper.Map<List<ScenarioCampaignResultEntity>>(scenarioCampaignResults);

            List<ScenarioCampaignResultEntity> entities = _dbContext.Query<ScenarioCampaignResultEntity>()
                .Where(e => e.ScenarioId == scenarioCampaignResults.Id)
                .ToList();

            if (!entities.Any())
            {
                _dbContext.BulkInsertEngine.BulkInsert(externalEntities, new BulkInsertOptions() { BatchSize = 600000, BulkCopyTimeout = 300 });
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
            var entities = _dbContext.Query<ScenarioCampaignResultEntity>().Where(e => e.ScenarioId == scenarioId).ToArray();

            if (entities.Any())
            {
                _dbContext.RemoveRange(entities);
            }
        }

        public ScenarioCampaignResult Get(Guid scenarioId)
        {
            var entities = _dbContext.Query<ScenarioCampaignResultEntity>()
                .Where(e => e.ScenarioId == scenarioId)
                .ToList();

            return _mapper.Map<ScenarioCampaignResult>(entities);
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
