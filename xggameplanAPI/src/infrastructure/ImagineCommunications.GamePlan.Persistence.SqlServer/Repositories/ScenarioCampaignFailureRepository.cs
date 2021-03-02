using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.core.Extensions;
using xggameplan.Extensions;
using ScenarioCampaignFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioCampaignFailure;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ScenarioCampaignFailureRepository : IScenarioCampaignFailureRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioCampaignFailureRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(ScenarioCampaignFailure scenarioCampaignFailure) =>
            _dbContext.Add(_mapper.Map<ScenarioCampaignFailureEntity>(scenarioCampaignFailure),
                post => post.MapTo(scenarioCampaignFailure), _mapper);

        public void AddRange(IEnumerable<ScenarioCampaignFailure> scenarioCampaignFailures, bool setIdentity = true)
        {
            var entities = _mapper.Map<ScenarioCampaignFailureEntity[]>(scenarioCampaignFailures);

            if (setIdentity)
            {
                _dbContext.AddRange(
                        entities,
                        post => post.MapToCollection(scenarioCampaignFailures),
                        _mapper
                    );
            }
            else
            {
                _dbContext.BulkInsertEngine.BulkInsert(
                        entities,
                        new BulkInsertOptions() { BatchSize = 400000, BulkCopyTimeout = 300 }
                    );
            }
        }

        public ScenarioCampaignFailure Get(int id)
            => _mapper.Map<ScenarioCampaignFailure>(_dbContext.Find<ScenarioCampaignFailureEntity>(id));

        public IEnumerable<ScenarioCampaignFailure> GetAll() =>
            _dbContext.Query<ScenarioCampaignFailureEntity>()
            .ProjectTo<ScenarioCampaignFailure>(_mapper.ConfigurationProvider)
            .ToList();

        public IEnumerable<ScenarioCampaignFailure> FindByScenarioId(Guid scenarioId) =>
            _dbContext.Query<ScenarioCampaignFailureEntity>()
            .Where(x => x.ScenarioId == scenarioId)
            .ProjectTo<ScenarioCampaignFailure>(_mapper.ConfigurationProvider)
            .ToList();

        public PagedQueryResult<ScenarioCampaignFailure> Search(ScenarioCampaignFailureSearchQueryModel searchQuery)
        {
            if (searchQuery == null)
            {
                throw new ArgumentNullException(nameof(searchQuery));
            }

            var query = _dbContext.Query<ScenarioCampaignFailureEntity>()
                .Where(p => p.ScenarioId == searchQuery.ScenarioId);

            if (searchQuery.SalesAreaGroupNames != null && searchQuery.SalesAreaGroupNames.Any())
            {
                query = query.Where(e => searchQuery.SalesAreaGroupNames.Contains(e.SalesAreaGroup));
            }

            if (searchQuery.ExternalCampaignIds != null && searchQuery.ExternalCampaignIds.Any())
            {
                query = query.Where(p => searchQuery.ExternalCampaignIds.Contains(p.ExternalCampaignId));
            }

            Expression<Func<ScenarioCampaignFailureEntity, bool>> exp = e => false;

            if (searchQuery.StrikeWeights?.Count() > 0)
            {
                var first = true;
                foreach (var strikeWeight in searchQuery.StrikeWeights)
                {
                    Expression<Func<ScenarioCampaignFailureEntity, bool>> e = x => false;

                    if (strikeWeight.StrikeWeightStartDate.HasValue)
                    {
                        e = scf => scf.StrikeWeightStartDate.Date > strikeWeight.StrikeWeightStartDate.Value.Date.AddDays(-1);

                        if (strikeWeight.StrikeWeightEndDate.HasValue)
                        {
                            e = e.And(scf => scf.StrikeWeightEndDate.Date <= strikeWeight.StrikeWeightEndDate.Value.Date);
                        }
                    }
                    else if (strikeWeight.StrikeWeightEndDate.HasValue)
                    {
                        e = scf => scf.StrikeWeightEndDate.Date <= strikeWeight.StrikeWeightEndDate.Value.Date;
                    }

                    if (first)
                    {
                        exp = e;
                        first = false;
                    }
                    else
                    {
                        exp = exp.Or(e);
                    }
                }

                query = query.Where(exp);
            }

            int totalCount = query.Count();

            var scenarioCampaignFailures = query.ApplyPaging(searchQuery.Skip, searchQuery.Top)
                .ProjectTo<ScenarioCampaignFailure>(_mapper.ConfigurationProvider)
                .ToList();

            return new PagedQueryResult<ScenarioCampaignFailure>(totalCount, scenarioCampaignFailures);
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<ScenarioCampaignFailureEntity>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            var entities = _dbContext.Query<ScenarioCampaignFailureEntity>()
                .Where(s => s.ScenarioId == scenarioId).ToArray();

            if (entities.Any())
            {
                _dbContext.RemoveRange(entities);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
