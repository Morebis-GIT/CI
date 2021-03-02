using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;
        private const int Size = 10000;

        public RecommendationRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Insert(IEnumerable<Recommendation> recommendations, bool setIdentity = true)
        {
            var bulkInsertOptions = new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true, BatchSize = 50000, BulkCopyTimeout = 300 };

            var entities = _mapper.Map<List<Entities.Tenant.Recommendation>>(recommendations);

            if (setIdentity)
            {
                _dbContext.BulkInsertEngine.BulkInsert(
                        entities,
                        bulkInsertOptions,
                        post => post.TryToUpdate(recommendations),
                        _mapper
                    );
            }
            else
            {
                _dbContext.BulkInsertEngine.BulkInsert(entities, new BulkInsertOptions() { BatchSize = 200000, BulkCopyTimeout = 300 });
            }
        }

        public IEnumerable<Recommendation> GetByScenarioId(Guid scenarioId)
        {
            return _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(x => x.ScenarioId == scenarioId)
                .ProjectTo<Recommendation>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<Recommendation> GetByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            return _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(x => x.ScenarioId == scenarioId && processors.Contains(x.Processor))
                .ProjectTo<Recommendation>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            return _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(x => x.ScenarioId == scenarioId && processors.Contains(x.Processor))
                .ProjectTo<RecommendationSimple>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdsAndProcessors(List<Guid> scenarioIds, IEnumerable<string> processors)
        {
            return _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(x => scenarioIds.Contains(x.ScenarioId) && processors.Contains(x.Processor))
                .ProjectTo<RecommendationSimple>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<RecommendationsByScenarioReduceResult> GetMetrics(Guid scenarioId, string campaignId)
        {
            return GetReducedGroups(x => x.ScenarioId == scenarioId && x.ExternalCampaignNumber == campaignId);
        }

        public IEnumerable<RecommendationsByScenarioReduceResult> GetCampaigns(Guid scenarioId)
        {
            return GetReducedGroups(x => x.ScenarioId == scenarioId);
        }

        private IEnumerable<RecommendationsByScenarioReduceResult> GetReducedGroups(Expression<Func<Entities.Tenant.Recommendation, bool>> @where)
        {
            return _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(@where)
                .GroupBy(x => new { x.ScenarioId, x.ExternalCampaignNumber, x.Action })
                .Select(agg => new RecommendationsByScenarioReduceResult
                {
                    ScenarioId = agg.Key.ScenarioId,
                    ExternalCampaignNumber = agg.Key.ExternalCampaignNumber,
                    Action = agg.Key.Action,
                    SpotRating = agg.Sum(x => x.SpotRating),
                    Count = agg.Count()
                })
                .ToList();
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            var ids = _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(x => x.ScenarioId == scenarioId)
                .Select(e => e.Id)
                .ToList();

            if (!ids.Any())
            {
                return;
            }

            using var transaction = _dbContext.Specific.Database.BeginTransaction();

            for (int i = 0, page = 0; i < ids.Count; i += Size, page++)
            {
                _dbContext.Specific.RemoveByIdentityIds<Entities.Tenant.Recommendation>(ids.Skip(Size * page).Take(Size)
                    .ToArray());
            }

            transaction.Commit();
        }

        public void RemoveByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            IEnumerable<string> cleanProcessors = processors
                .Where(p => !String.IsNullOrWhiteSpace(p));

            var ids = _dbContext.Query<Entities.Tenant.Recommendation>()
                .Where(x => x.ScenarioId == scenarioId && cleanProcessors.Contains(x.Processor))
                .Select(e => e.Id)
                .ToList();

            if (ids.Count == 0)
            {
                return;
            }

            using var transaction = _dbContext.Specific.Database.BeginTransaction();

            for (int i = 0, page = 0; i < ids.Count; i += Size, page++)
            {
                _dbContext.Specific.RemoveByIdentityIds<Entities.Tenant.Recommendation>(
                    ids.Skip(Size * page)
                        .Take(Size)
                        .ToArray()
                    );
            }

            transaction.Commit();
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
