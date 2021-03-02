using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using ScenarioResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioResult;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ScenarioResultRepository : IScenarioResultRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioResultRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ScenarioResult Find(Guid scenarioId) =>
            _dbContext.Query<ScenarioResultEntity>()
                .ProjectTo<ScenarioResult>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == scenarioId);

        public IEnumerable<ScenarioResult> Find(Guid[] scenarioIds) =>
            _dbContext.Query<ScenarioResultEntity>()
                .Where(x => scenarioIds.Contains(x.ScenarioId))
                .ProjectTo<ScenarioResult>(_mapper.ConfigurationProvider);

        public IEnumerable<ScenarioResult> GetAll() =>
            _dbContext.Query<ScenarioResultEntity>()
                .ProjectTo<ScenarioResult>(_mapper.ConfigurationProvider)
                .ToList();

        public void Add(ScenarioResult scenarioResult)
        {
            var entity = GetQueryWithAllIncludes()
                .FirstOrDefault(x => x.ScenarioId == scenarioResult.Id);

            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<ScenarioResultEntity>(scenarioResult),
                    post => post.MapTo(scenarioResult), _mapper);
            }
            else
            {
                _mapper.Map(scenarioResult, entity);
                _dbContext.Update(entity, post => post.MapTo(scenarioResult), _mapper);
            }
        }

        public void Update(ScenarioResult scenarioResult)
        {
            var entity = GetQueryWithAllIncludes()
                .FirstOrDefault(x => x.ScenarioId == scenarioResult.Id);

            if (entity != null)
            {
                _mapper.Map(scenarioResult, entity);
                _dbContext.Update(entity, post => post.MapTo(scenarioResult), _mapper);
            }
        }

        public void Remove(Guid id)
        {
            var entity = _dbContext.Query<ScenarioResultEntity>()
                .FirstOrDefault(x => x.ScenarioId == id);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void UpdateRange(IEnumerable<ScenarioResult> items)
        {
            var scenarioIds = items.Select(y => y.Id).ToList();

            var entities = GetQueryWithAllIncludes()
                .Where(x => scenarioIds.Contains(x.ScenarioId))
                .ToArray();

            foreach (var entity in entities)
            {
                var item = items.FirstOrDefault(x => x.Id == entity.ScenarioId);
                if (item != null)
                {
                    _mapper.Map(item, entity);
                }
            }

            _dbContext.UpdateRange(entities, post => post.MapToCollection(items), _mapper);
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        private IQueryable<ScenarioResultEntity> GetQueryWithAllIncludes() =>
            _dbContext.Query<ScenarioResultEntity>()
                .Include(x => x.Metrics)
                .Include(x => x.LandmarkMetrics)
                .Include(x => x.AnalysisGroupMetrics);
    }
}
