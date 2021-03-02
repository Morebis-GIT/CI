using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Models;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Abstractions.Util;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.RavenDB.Index;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenPassRepository : IPassRepository
    {
        private readonly IDocumentSession _session;

        public RavenPassRepository(IDocumentSession session) =>
            _session = session;

        public Pass Get(int id)
        {
            lock (_session)
            {
                return _session.Load<Pass>(id);
            }
        }

        public Pass FindByName(string name, bool isLibraried)
        {
            name = name?.Trim();
            lock (_session)
            {
                var query = _session
                    .Query<Pass>()
                    .Where(p => p.Name == name);

                if (isLibraried)
                {
                    return query.FirstOrDefault(p => p.IsLibraried == true);
                }
                else
                {
                    return query.FirstOrDefault();
                }
            }
        }

        ///<summary>Find all passes with the given ids.</summary>
        /// <remarks>RavenDB will throw an exception (Lucene.Net.Search.BooleanQuery.TooManyClauses)
        /// if more than 1,024 items are passed to its in() function.
        /// </remarks>
        public IEnumerable<Pass> FindByIds(IEnumerable<int> ids)
        {
            const int BatchSize = 1000;

            if (!ids.Any())
            {
                return new List<Pass>(0);
            }

            IReadOnlyCollection<int> sourceIds = ids.ToList();

            lock (_session)
            {
                if (sourceIds.Count < BatchSize)
                {
                    return GetBatch(sourceIds);
                }

                var result = new List<Pass>();

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
                IReadOnlyCollection<Pass> GetBatch(IEnumerable<int> batch) =>
                    _session.GetAll<Pass>(
                        p => p.Id.In(batch),
                        indexName: Passes_Default.DefaultIndexName,
                        isMapReduce: false
                        );
            }
        }

        /// <summary>
        /// An empty list of IDs is needed to force RavenDB to stream results.
        /// </summary>
        private static IEnumerable<int> ForceRavenDbToStreamResults => new List<int>() { 0 };

        public IEnumerable<Pass> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Pass>(p =>
                   !p.Id.In(ForceRavenDbToStreamResults),
                   indexName: Passes_Default.DefaultIndexName,
                   isMapReduce: false);
            }
        }

        public IEnumerable<int> GetLibraryIds()
        {
            return RavenPassRepository.GetLibraryIds(_session);
        }

        internal static IEnumerable<int> GetLibraryIds(IDocumentSession session)
        {
            lock (session)
            {
                return session
                    .GetAllUsingProjection<Pass, Passes_IsLibrariedOnly, PassId>(
                        p => !p.Id.In(ForceRavenDbToStreamResults)
                    )
                    .Select(c => c.Id);
            }
        }

        public IEnumerable<Pass> FindByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var scenariosWithPassId = _session
                    .GetAllWithTransform<Scenario, ScenariosWithPassId_Transformer, ScenariosWithPassIdTransformerResult>(scenario =>
                        scenario.Id == scenarioId,
                        indexName: Scenarios_Default.DefaultIndexName,
                        isMapReduce: false)
                    .ToList();

                var scenarioPassIds = scenariosWithPassId
                    .Where(swp => swp.ScenarioId == scenarioId)
                    .Select(swp => swp.PassId)
                    .ToList()
                    .Distinct();

                return _session
                    .Query<Pass>()
                    .Where(p => p.Id.In(scenarioPassIds))
                    .ToList();
            }
        }

        public SearchResultModel<PassDigestListItem> MinimalDataSearch(
            SearchQueryDto queryModel,
            bool isLibraried)
        {
            lock (_session)
            {
                List<PassDigestListItem> allRecords
                    = _session.GetAllUsingProjection
                    <Pass, Passes_MinimalData, PassDigestListItem>
                    (p => p.IsLibraried.HasValue && p.IsLibraried.Value == isLibraried)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(queryModel.Title))
                {
                    allRecords.RemoveAll(x => DoesNotContainTitle(queryModel.Title, x.Name));
                }

                IOrderedEnumerable<PassDigestListItem> orderQuery =
                    queryModel.OrderDirection == OrderDirection.Desc
                    ? allRecords.OrderByDescending(OrderByProperty(queryModel.OrderBy))
                    : allRecords.OrderBy(OrderByProperty(queryModel.OrderBy));

                var searchResults = new SearchResultModel<PassDigestListItem>();
                searchResults.Items = orderQuery
                    .Skip(queryModel.Skip)
                    .Take(queryModel.Top)
                    .ToList();
                searchResults.TotalCount = orderQuery.Count();

                return searchResults;
            }

            Func<PassDigestListItem, object> OrderByProperty(OrderBy orderBy)
            {
                if (OrderBy.Date == orderBy)
                {
                    return x => x.DateModified;
                }
                else
                {
                    return x => x.Name;
                }
            }

            bool DoesNotContainTitle(string title, string passDigestName) =>
                passDigestName.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) < 0;
        }

        public PagedQueryResult<Pass> Search(PassSearchQueryModel queryModel, StringMatchRules freeTextMatchRules)
        {
            lock (_session)
            {
                // Get passes containing name, freeTextMatchRules is required to perform additional filtering
                var passes = new List<Pass>();
                RavenQueryStatistics stats;

                // Get library passes
                List<int> libraryPassIds = queryModel.IsLibraried == null ? null : RavenPassRepository.GetLibraryIds(_session).ToList();

                var passQuery = _session.Query<Pass, Passes_Default>();
                passQuery = QueryFreeText(passQuery, queryModel.Name);  // Basic filtering
                if (queryModel.IsLibraried != null && libraryPassIds.Count <= 1024) // Filter server-side if possible
                {
                    passQuery = queryModel.IsLibraried.Value ? passQuery.Where(p => p.Id.In(libraryPassIds)) : passQuery.Where(p => !p.Id.In(libraryPassIds));
                }
                if (queryModel.OrderBy != null && queryModel.OrderBy.Any())
                {
                    passQuery = passQuery.OrderByMultipleItems(queryModel.OrderBy);
                }
                else    // Default order
                {
                    passQuery = passQuery.OrderBy(p => p.Name);
                }
                passQuery.Statistics(out stats);
                passes = passQuery.As<Pass>().Take(Int32.MaxValue).ToList().Where(p => freeTextMatchRules.IsMatches(p.Name, queryModel.Name)).ToList();

                // Filter libraried/non-libraried, only really necessary where we couldn't do server-side filtering above
                if (queryModel.IsLibraried != null)
                {
                    passes = passes.Where(p => queryModel.IsLibraried.Value ? libraryPassIds.Contains(p.Id) : !libraryPassIds.Contains(p.Id)).ToList();
                }

                var passResults = queryModel.Top == null || queryModel.Top == 0 ? passes.ToList() :
                                    passes.Skip(queryModel.Skip.GetValueOrDefault(0)).Take(queryModel.Top.Value).ToList();

                return new PagedQueryResult<Pass>(passes.Count, passResults);
            }
        }

        private IRavenQueryable<Pass> QueryFreeText(IRavenQueryable<Pass> query, string name)
        {
            if (String.IsNullOrEmpty(name))   // No filter
            {
                return query;
            }

            // Returns scenarios containing all of the search words
            //name.Trim().Split(' ').ToList().ForEach(element => query = query.Search(s => s.Name, $"*{element}*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards));

            // Returns scenarios containing any of the search words, allows further local filtering depending on string match rules
            query = query.Search(s => s.Name, $"*" + RavenQuery.Escape(name, true, false) + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);
            return query;
        }

        public void Add(Pass pass)
        {
            lock (_session)
            {
                AddAuditDateStamp(pass);
                _session.Store(pass);
            }
        }

        public void Add(IEnumerable<Pass> items)
        {
            lock (_session)
            {
                items
                    .Where(p => p != null)
                    .ToList()
                    .ForEach(pass =>
                    {
                        AddAuditDateStamp(pass);
                        _session.Store(pass);
                    });
            }
        }

        public void Update(Pass pass)
        {
            lock (_session)
            {
                AddAuditDateStamp(pass);
                _session.Store(pass);
            }
        }

        public void Update(IEnumerable<Pass> passes)
        {
            lock (_session)
            {
                passes
                    .Where(p => p != null)
                    .ToList()
                    .ForEach(pass =>
                    {
                        AddAuditDateStamp(pass);
                        _session.Store(pass);
                    });
            }
        }

        private void AddAuditDateStamp(Pass pass)
        {
            pass.DateModified = DateTime.UtcNow;

            if (!pass.DateCreated.HasValue)
            {
                pass.DateCreated = DateTime.UtcNow;
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                _session.Delete(Get(id));
            }
        }

        public void Remove(IEnumerable<int> ids)
        {
            lock (_session)
            {
                ids.ToList().ForEach(id => _session.Delete<Pass>(id));
            }
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var scenariosWithPassId = _session
                    .GetAllWithTransform<Scenario, ScenariosWithPassId_Transformer, ScenariosWithPassIdTransformerResult>(scenario =>
                        scenario.Id == scenarioId,
                        indexName: Scenarios_Default.DefaultIndexName,
                        isMapReduce: false)
                    .ToList();

                var scenarioPassIds = scenariosWithPassId
                    .Where(swp => swp.ScenarioId == scenarioId)
                    .Select(swp => swp.PassId)
                    .ToList()
                    .Distinct();

                scenarioPassIds
                    .ToList()
                    .ForEach(passId => _session.Delete<Pass>(passId));
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

        private void WaitForIndexes()
        {
            _session.WaitForIndexes<Pass>(
                indexName: Passes_Default.DefaultIndexName,
                isMapReduce: false,
                testExpression: p => p.Id == 0);
        }
    }
}
