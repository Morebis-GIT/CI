using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios;
using Microsoft.EntityFrameworkCore;
using Scenario = ImagineCommunications.GamePlan.Domain.Scenarios.Objects.Scenario;
using ScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.Scenario;
using ScenarioPassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.ScenarioPassReference;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ScenarioRepository : IScenarioRepository
    {
        private const int DeleteBatchSize = 10000;

        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Scenario scenario)
        {
            var entity = _mapper.Map<ScenarioEntity>(scenario);
            ApplyPassesOrder(entity.PassReferences);

            _dbContext.Add(
                entity,
                post => post.MapTo(scenario),
                _mapper);
        }

        public void Add(IEnumerable<Scenario> scenarios)
        {
            if (scenarios == null || !scenarios.Any())
            {
                return;
            }

            var entities = _mapper.Map<ScenarioEntity[]>(scenarios);
            foreach (var entity in entities)
            {
                ApplyPassesOrder(entity.PassReferences);
            }

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                _dbContext.BulkInsertEngine.BulkInsert(entities, new BulkInsertOptions { PreserveInsertOrder = true });

                BulkInsertNestedEntities(entities);
                transaction.Commit();

                var actionBuilder = new BulkInsertActionBuilder<ScenarioEntity>(entities, _mapper);
                actionBuilder.TryToUpdate(scenarios);
                actionBuilder.Build()?.Execute();
            }
        }

        public void Delete(Guid id)
        {
            var entity = _dbContext.Find<ScenarioEntity>(id);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public IEnumerable<Scenario> FindByIds(IEnumerable<Guid> ids) => _dbContext
            .Query<ScenarioEntity>()
            .Where(e => ids.Contains(e.Id))
            .ProjectTo<Scenario>(_mapper.ConfigurationProvider)
            .ToArray();

        public Scenario FindByName(string name, bool isLibraried)
        {
            name = name?.Trim();

            var query = _dbContext
                    .Query<ScenarioEntity>()
                    .Where(s => s.Name == name)
                    .ProjectTo<Scenario>(_mapper.ConfigurationProvider);

            return isLibraried ? query.FirstOrDefault(s => s.IsLibraried == true) : query.FirstOrDefault();
        }

        public Scenario Get(Guid id) => _dbContext
            .Query<ScenarioEntity>()
            .ProjectTo<Scenario>(_mapper.ConfigurationProvider)
            .FirstOrDefault(e => e.Id == id);

        public IEnumerable<Scenario> GetAll() => _dbContext
            .Query<ScenarioEntity>()
            .ProjectTo<Scenario>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Scenario> GetByPassId(int passId) => _dbContext
            .Query<ScenarioEntity>()
            .Include(e => e.PassReferences)
            .Where(e => e.PassReferences.Any(x => x.PassId == passId))
            .ProjectTo<Scenario>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Scenario> GetLibraried() => _dbContext
            .Query<ScenarioEntity>()
            .Where(e => e.IsLibraried.Value)
            .ProjectTo<Scenario>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<ScenariosWithPassIdTransformerResult> GetScenariosWithPassId() => _dbContext
            .Query<ScenarioEntity>()
            .SelectMany(scenario => scenario.PassReferences.Select(pass => new ScenariosWithPassIdTransformerResult
            {
                Id = $"scenarios/{scenario.Id}",
                PassId = pass.PassId
            }))
            .ToArray();

        public IEnumerable<ScenariosWithPassIdTransformerResult> GetScenariosWithPassId(IEnumerable<Guid> scenarioIds) => _dbContext
            .Query<ScenarioEntity>()
            .Where(e => scenarioIds.Contains(e.Id))
            .SelectMany(scenario => scenario.PassReferences.Select(pass => new ScenariosWithPassIdTransformerResult
            {
                Id = $"scenarios/{scenario.Id}",
                PassId = pass.PassId
            }))
            .ToArray();

        public void Remove(IEnumerable<Guid> ids)
        {
            var entities = _dbContext.Query<ScenarioEntity>()
                .Where(e => ids.Contains(e.Id))
                .ToArray();

            if (entities.Any())
            {
                _dbContext.RemoveRange(entities);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public SearchResultModel<ScenarioDigestListItem> MinimalDataSearch(
            SearchQueryDto queryModel,
            bool isLibraried,
            IEnumerable<int> passesToInclude)
        {
            IReadOnlyCollection<ScenarioDigestListItem> allRecords = _dbContext.Query<ScenarioEntity>()
                .Where(s => s.IsLibraried.HasValue && s.IsLibraried.Value == isLibraried)
                .ProjectTo<ScenarioDigestListItem>(_mapper.ConfigurationProvider)
                .ToList();

            List<ScenarioDigestListItem> searchQuery = allRecords.ToList();

            if (!string.IsNullOrWhiteSpace(queryModel.Title))
            {
                searchQuery.RemoveAll(x => DoesNotContainTitle(queryModel.Title, x.Name));
            }

            if (passesToInclude.Any())
            {
                IEnumerable<ScenarioDigestListItem> scenariosWithPassesToInclude = allRecords
                     .Where(scenario =>
                         scenario.Passes.Any(pass => passesToInclude.Contains(pass.Id)) && !searchQuery.Contains(scenario));
                searchQuery.AddRange(scenariosWithPassesToInclude);
            }

            IOrderedEnumerable<ScenarioDigestListItem> orderQuery =
                               queryModel.OrderDirection == OrderDirection.Desc
                               ? searchQuery.OrderByDescending(OrderByProperty(queryModel.OrderBy))
                               : searchQuery.OrderBy(OrderByProperty(queryModel.OrderBy));

            var searchResults = new SearchResultModel<ScenarioDigestListItem>();
            searchResults.Items = orderQuery
                .Skip(queryModel.Skip)
                .Take(queryModel.Top)
                .ToList();
            searchResults.TotalCount = orderQuery.Count();

            return searchResults;

            Func<ScenarioDigestListItem, object> OrderByProperty(OrderBy orderBy)
            {
                if (OrderBy.Date == orderBy)
                {
                    return x => x.DateUserModified;
                }
                else
                {
                    return x => x.Name;
                }
            }

            bool DoesNotContainTitle(string title, string scenarioDigestName) =>
                scenarioDigestName.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) < 0;
        }

        public void Update(Scenario scenario)
        {
            var entity = GetScenarioQueryWithAllIncludes().FirstOrDefault(e => e.Id == scenario.Id);

            if (entity != null)
            {
                _mapper.Map(scenario, entity);
                ApplyPassesOrder(entity.PassReferences);

                _dbContext.Update(entity, post => post.MapTo(scenario), _mapper);
            }
        }

        public void Update(IEnumerable<Scenario> scenarios)
        {
            if (scenarios == null || !scenarios.Any())
            {
                return;
            }

            var entities = _mapper.Map<ScenarioEntity[]>(scenarios);
            foreach (var entity in entities)
            {
                ApplyPassesOrder(entity.PassReferences);
            }

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                _dbContext.BulkInsertEngine.BulkUpdate(entities, new BulkInsertOptions { PreserveInsertOrder = true });

                // remove existing entities
                var scenarioIds = entities.Select(x => x.Id).ToList();
                var roundsToDelete = _dbContext.Query<ScenarioCampaignPriorityRoundCollection>()
                    .Where(x => scenarioIds.Contains(x.ScenarioId)).Select(x => x.Id).ToArray();
                var passReferencesToDelete = _dbContext.Query<ScenarioPassReference>()
                    .Where(x => scenarioIds.Contains(x.ScenarioId)).Select(x => x.Id).ToArray();
                var passPrioritiesToDelete = _dbContext.Query<ScenarioCampaignPassPriority>()
                    .Where(x => scenarioIds.Contains(x.ScenarioId)).Select(x => x.Id).ToArray();

                if (roundsToDelete.Any())
                {
                    for (int i = 0; i <= roundsToDelete.Length / DeleteBatchSize; i++)
                    {
                        var roundIds = roundsToDelete.Skip(i * DeleteBatchSize).Take(DeleteBatchSize).ToArray();
                        _dbContext.Specific.RemoveByIdentityIds<ScenarioCampaignPriorityRoundCollection>(roundIds);
                    }
                }

                if (passReferencesToDelete.Any())
                {
                    for (int i = 0; i <= passReferencesToDelete.Length / DeleteBatchSize; i++)
                    {
                        var passIds = passReferencesToDelete.Skip(i * DeleteBatchSize).Take(DeleteBatchSize).ToArray();
                        _dbContext.Specific.RemoveByIdentityIds<ScenarioPassReference>(passIds);
                    }
                }

                if (passPrioritiesToDelete.Any())
                {
                    for (int i = 0; i <= passPrioritiesToDelete.Length / DeleteBatchSize; i++)
                    {
                        var priorityIds = passPrioritiesToDelete.Skip(i * DeleteBatchSize).Take(DeleteBatchSize).ToArray();
                        _dbContext.Specific.RemoveByIdentityIds<ScenarioCampaignPassPriority>(priorityIds);
                    }
                }

                BulkInsertNestedEntities(entities);

                transaction.Commit();

                var actionBuilder = new BulkInsertActionBuilder<ScenarioEntity>(entities, _mapper);
                actionBuilder.TryToUpdate(scenarios);
                actionBuilder.Build()?.Execute();
            }
        }

        private IQueryable<ScenarioEntity> GetScenarioQueryWithAllIncludes() =>
            _dbContext.Query<ScenarioEntity>()
                .Include(e => e.PassReferences)
                .Include(e => e.CampaignPriorityRounds)
                    .ThenInclude(e => e.Rounds)
                .Include(e => e.CampaignPassPriorities)
                    .ThenInclude(e => e.PassPriorities)
                .Include(e => e.CampaignPassPriorities)
                    .ThenInclude(e => e.Campaign)
                        .ThenInclude(e => e.CampaignPaybacks);

        private static void ApplyPassesOrder(IEnumerable<ScenarioPassEntity> passReferences)
        {
            var order = 1;
            foreach (var pr in passReferences)
            {
                pr.Order = order++;
            }
        }

        private void BulkInsertNestedEntities(IReadOnlyCollection<ScenarioEntity> entities)
        {
            var scenarioCampaignPriorityRounds = entities
                .Where(x => x.CampaignPriorityRounds != null)
                .Select(x =>
                {
                    x.CampaignPriorityRounds.ScenarioId = x.Id;
                    return x.CampaignPriorityRounds;
                }).ToList();

            if (scenarioCampaignPriorityRounds.Any())
            {
                _dbContext.BulkInsertEngine.BulkInsert(scenarioCampaignPriorityRounds, new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                var nestedRounds = scenarioCampaignPriorityRounds
                    .Where(x => x.Rounds != null && x.Rounds.Any())
                    .SelectMany(s => s.Rounds.Select(r =>
                    {
                        r.ScenarioCampaignPriorityRoundCollectionId = s.Id;
                        return r;
                    })).ToList();

                if (nestedRounds.Any())
                {
                    _dbContext.BulkInsertEngine.BulkInsert(nestedRounds, new BulkInsertOptions { PreserveInsertOrder = true });
                }
            }

            var passReferences = entities
                .Where(x => x.PassReferences != null && x.PassReferences.Any())
                .SelectMany(s => s.PassReferences.Select(r =>
                {
                    r.ScenarioId = s.Id;
                    return r;
                })).ToList();

            if (passReferences.Any())
            {
                _dbContext.BulkInsertEngine.BulkInsert(passReferences, new BulkInsertOptions { PreserveInsertOrder = true });
            }

            var campPassPriorities = entities
                .Where(x => x.CampaignPassPriorities != null && x.CampaignPassPriorities.Any())
                .SelectMany(s => s.CampaignPassPriorities.Select(r =>
                {
                    r.ScenarioId = s.Id;
                    return r;
                })).ToList();

            if (campPassPriorities.Any())
            {
                _dbContext.BulkInsertEngine.BulkInsert(campPassPriorities, new BulkInsertOptions { BatchSize = 30000, PreserveInsertOrder = true, SetOutputIdentity = true });

                var passPriorities = campPassPriorities
                    .Where(x => x.PassPriorities != null && x.PassPriorities.Any())
                    .SelectMany(s => s.PassPriorities.Select(r =>
                    {
                        r.ScenarioCampaignPassPriorityId = s.Id;
                        return r;
                    })).ToList();

                if (passPriorities.Any())
                {
                    _dbContext.BulkInsertEngine.BulkInsert(passPriorities, new BulkInsertOptions { BatchSize = 400000, PreserveInsertOrder = true });
                }

                var compactCampaigns = campPassPriorities
                    .Where(x => x.Campaign != null)
                    .Select(s =>
                    {
                        s.Campaign.ScenarioCampaignPassPriorityId = s.Id;
                        return s.Campaign;
                    }).ToList();

                if (compactCampaigns.Any())
                {
                    _dbContext.BulkInsertEngine.BulkInsert(compactCampaigns, new BulkInsertOptions { BatchSize = 30000, PreserveInsertOrder = true, SetOutputIdentity = true });

                    var compactCampaignPaybacks = compactCampaigns
                        .Where(x => x.CampaignPaybacks != null && x.CampaignPaybacks.Any())
                        .SelectMany(s => s.CampaignPaybacks.Select(p =>
                        {
                            p.ScenarioCompactCampaignId = s.Id;
                            return p;
                        })).ToList();

                    if (compactCampaignPaybacks.Any())
                    {
                        _dbContext.BulkInsertEngine.BulkInsert(compactCampaignPaybacks, new BulkInsertOptions { BatchSize = 400000, PreserveInsertOrder = true });
                    }
                }
            }
        }
    }
}
