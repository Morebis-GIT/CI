using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Queries;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenScenarioRepository : IScenarioRepository
    {
        private readonly IDocumentSession _session;

        public RavenScenarioRepository(IDocumentSession session)
        {
            _session = session;
        }

        public Scenario Get(Guid id)
        {
            lock (_session)
            {
                return _session.Load<Scenario>(id);
            }
        }

        public Scenario FindByName(string name, bool isLibraried)
        {
            name = name?.Trim();
            lock (_session)
            {
                var query = _session
                    .Query<Scenario>()
                    .Where(s => s.Name == name);

                if (isLibraried)
                {
                    return query.FirstOrDefault(s => s.IsLibraried == true);
                }
                else
                {
                    return query.FirstOrDefault();
                }
            }
        }

        ///<summary>Find all scenarios with the given ids.</summary>
        /// <remarks>RavenDB will throw an exception (Lucene.Net.Search.BooleanQuery.TooManyClauses)
        /// if more than 1,024 items are passed to its in() function.
        /// </remarks>
        public IEnumerable<Scenario> FindByIds(IEnumerable<Guid> ids)
        {
            const int BatchSize = 1000;

            if (!ids.Any())
            {
                return new List<Scenario>(0);
            }

            IReadOnlyCollection<Guid> sourceIds = ids.ToList();

            lock (_session)
            {
                if (sourceIds.Count < BatchSize)
                {
                    return GetBatch(sourceIds);
                }

                var result = new List<Scenario>();

                int pageNumber = 0;

                do
                {
                    var batch = sourceIds
                        .Skip(BatchSize * pageNumber)
                        .Take(BatchSize)
                        .ToList();

                    if (batch.Count == 0)
                    {
                        return result;
                    }

                    result.AddRange(
                        GetBatch(batch)
                        );

                    ++pageNumber;
                } while (true);

                //----------------------
                IReadOnlyCollection<Scenario> GetBatch(IEnumerable<Guid> batch) =>
                    _session.GetAll<Scenario>(
                        s => s.Id.In(batch),
                        indexName: Scenarios_Default.DefaultIndexName,
                        isMapReduce: false
                        );
            }
        }

        public IEnumerable<Scenario> GetLibraried()
        {
            lock (_session)
            {
                return _session.GetAll<Scenario>(x => x.IsLibraried == true);
            }
        }

        public IEnumerable<Scenario> GetAll()
        {
            lock (_session)
            {
                var dummyIds = new List<Guid>() { Guid.Empty };
                return _session.GetAll<Scenario>(s => !s.Id.In(dummyIds), indexName: Scenarios_Default.DefaultIndexName, isMapReduce: false);  // Dummy where clause so that streaming is used
            }
        }

        public IEnumerable<Scenario> GetByPassId(int passId)
        {
            lock (_session)
            {
                var scenariosWithPassId = _session.GetAllWithTransform<Scenario, ScenariosWithPassId_Transformer, ScenariosWithPassIdTransformerResult>(scenario => scenario.Id != Guid.Empty,
                                                   indexName: Scenarios_Default.DefaultIndexName, isMapReduce: false).ToList();

                var scenarioIdsForPassId = scenariosWithPassId.Where(swp => swp.PassId == passId).Select(swp => swp.ScenarioId).Distinct();
                if (scenarioIdsForPassId.Any())
                {
                    var scenarios = _session.Query<Scenario>().Where(s => s.Id.In(scenarioIdsForPassId));
                    return scenarios;
                }
                return new List<Scenario>();
            }
        }

        public IEnumerable<ScenariosWithPassIdTransformerResult> GetScenariosWithPassId()
        {
            lock (_session)
            {
                var scenariosWithPassId = _session.GetAllWithTransform<Scenario, ScenariosWithPassId_Transformer, ScenariosWithPassIdTransformerResult>(scenario => scenario.Id != Guid.Empty,
                                                  indexName: Scenarios_Default.DefaultIndexName, isMapReduce: false).ToList();
                return scenariosWithPassId;
            }
        }

        public IEnumerable<ScenariosWithPassIdTransformerResult> GetScenariosWithPassId(IEnumerable<Guid> scenarioIds)
        {
            lock (_session)
            {
                var scenariosWithPassId = _session.GetAllWithTransform<Scenario, ScenariosWithPassId_Transformer, ScenariosWithPassIdTransformerResult>(scenario => scenario.Id.In(scenarioIds),
                                                  indexName: Scenarios_Default.DefaultIndexName, isMapReduce: false).ToList();
                return scenariosWithPassId;
            }
        }

        public SearchResultModel<ScenarioDigestListItem> MinimalDataSearch(
            SearchQueryDto queryModel,
            bool isLibraried,
            IEnumerable<int> passesToInclude)
        {
            lock (_session)
            {
                IReadOnlyCollection<ScenarioDigestListItem> allRecords
                    = _session.GetAllUsingProjection
                        <Scenario, Scenarios_MinimalData, ScenarioDigestListItem>
                        (s => s.IsLibraried.HasValue && s.IsLibraried.Value == isLibraried);

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
            }

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

            bool DoesNotContainTitle(string title, string scenarioDigestName)
            {
                return scenarioDigestName.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) < 0;
            }
        }

        public void Add(Scenario scenario)
        {
            lock (_session)
            {
                AddAuditDateStamp(scenario);
                _session.Store(scenario);
            }
        }

        public void Add(IEnumerable<Scenario> scenarios)
        {
            if (scenarios != null && scenarios.Any())
            {
                lock (_session)
                {
                    using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
                    {
                        scenarios.Where(s => s != null).ToList().ForEach(scenario =>
                        {
                            AddAuditDateStamp(scenario);
                            _session.Store(scenario);
                        });
                    }
                }
            }
        }

        public void Update(Scenario scenario)
        {
            lock (_session)
            {
                AddAuditDateStamp(scenario);
                _session.Store(scenario);
            }
        }

        public void Update(IEnumerable<Scenario> scenarios)
        {
            if (scenarios != null && scenarios.Any())
            {
                lock (_session)
                {
                    scenarios.Where(s => s != null).ToList().ForEach(scenario =>
                    {
                        AddAuditDateStamp(scenario);
                        _session.Store(scenario);
                    });
                }
            }
        }

        [Obsolete("Use the Delete() method.")]
        public void Remove(Guid id) => Delete(id);

        public void Delete(Guid id)
        {
            lock (_session)
            {
                var scenario = Get(id);

                if (scenario is null)
                {
                    return;
                }

                _session.Delete(scenario);
            }
        }

        public void Remove(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                ids.ToList()
                    .ForEach(id => _session.Delete<Scenario>(id));
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
                WaitForIndexes();
            }
        }

        private void AddAuditDateStamp(Scenario scenario)
        {
            scenario.DateModified = DateTime.UtcNow;

            if (!scenario.DateCreated.HasValue)
            {
                scenario.DateCreated = DateTime.UtcNow;
            }
        }

        private void WaitForIndexes()
        {
            _session.WaitForIndexes<Scenario>(
                indexName: Scenarios_Default.DefaultIndexName,
                isMapReduce: false,
                testExpression: s => s.Id == Guid.Empty
                );
        }
    }
}
