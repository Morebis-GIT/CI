using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Reducers;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Abstractions.Util;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.core.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRunRepository : IRunRepository
    {
        private readonly IDocumentSession _session;

        private readonly ExternalScenarioStatus[] _triggeredInLandmarkStatuses =
        {
            ExternalScenarioStatus.Accepted,
            ExternalScenarioStatus.Scheduled,
            ExternalScenarioStatus.Running
        };

        public RavenRunRepository(IDocumentSession session) => _session = session;

        public Run Find(Guid id)
        {
            Run run = null;

            lock (_session)
            {
                run = _session.Load<Run>(id);
            }

            return run;
        }

        public Guid GetRunIdForScenario(Guid scenarioId) =>
            _session
                .GetAllWithTransform<Run, RunsWithScenarioId_Transformer, RunsWithScenarioIdTransformerResult>(
                    run => run.Id != Guid.Empty, Runs_BySearch.DefaultIndexName, false)
                .FirstOrDefault(rws => rws.ScenarioId == scenarioId)?.RunId ?? Guid.Empty;

        public IEnumerable<Run> FindByIds(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                return _session.GetAll<Run>(s => s.Id.In(ids));
            }
        }

        public Run FindByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var runsForScenarioId = _session
                    .GetAllWithTransform<Run, RunsWithScenarioId_Transformer, RunsWithScenarioIdTransformerResult>(
                        run => run.Id != Guid.Empty,
                        indexName: Runs_BySearch.DefaultIndexName,
                        isMapReduce: false)
                    .Where(rws => rws.ScenarioId == scenarioId);

                return runsForScenarioId.Any()
                    ? _session.Load<Run>(runsForScenarioId.First().RunId)
                    : null;
            }
        }

        public IEnumerable<Run> GetByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                var runsWithScenarioId = _session.GetAllWithTransform<Run, RunsWithScenarioId_Transformer, RunsWithScenarioIdTransformerResult>(
                    run => run.Id != Guid.Empty,
                    indexName: Runs_BySearch.DefaultIndexName,
                    isMapReduce: false);

                var runIds = runsWithScenarioId
                    .Where(r => r.ScenarioId == scenarioId)
                    .Select(r => r.RunId)
                    .Distinct();

                return runIds.Any()
                    ? _session
                        .GetAll<Run>(
                            r => r.Id.In(runIds),
                            indexName: Runs_BySearch.DefaultIndexName,
                            isMapReduce: false)
                        .ToList()
                    : new List<Run>();
            }
        }

        public IEnumerable<RunsWithScenarioIdTransformerResult> GetRunsWithScenarioId()
        {
            lock (_session)
            {
                return _session.GetAllWithTransform<Run, RunsWithScenarioId_Transformer, RunsWithScenarioIdTransformerResult>(
                    run => run.Id != Guid.Empty,
                    indexName: Runs_BySearch.DefaultIndexName,
                    isMapReduce: false);
            }
        }

        public Run FindByExternalRunId(Guid externalRunId)
        {
            lock (_session)
            {
                return _session.GetAll<Run>(c => c.Scenarios.Any(c => c.ExternalRunInfo != null && c.ExternalRunInfo.ExternalRunId == externalRunId))
                    .FirstOrDefault();
            }
        }

        public IEnumerable<Run> FindTriggeredInLandmark()
        {
            lock (_session)
            {
                return _session.GetAll<Run>(c => c.Scenarios.Any(c =>
                    c.ExternalRunInfo != null &&
                    _triggeredInLandmarkStatuses.Contains(c.ExternalRunInfo.ExternalStatus)));
            }
        }

        public IEnumerable<Run> FindLandmarkRuns()
        {
            lock (_session)
            {
                return _session.GetAll<Run>(c => c.Scenarios.Any(c => c.ExternalRunInfo != null));
            }
        }

        public IEnumerable<Run> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Run>();
            }
        }

        public IEnumerable<Run> GetAllActive()
        {
            lock (_session)
            {
                DateTime recent = DateTime.UtcNow.AddHours(-48);

                return _session
                    .GetAll<Run>(r =>
                        r.ExecuteStartedDateTime != null && r.ExecuteStartedDateTime >= recent,
                        indexName: Runs_ByExecuteStartedDateTimeSortByExecuteStartedDateTime.DefaultIndexName,
                        isMapReduce: false
                        )
                    .Where(r => r.Scenarios.Any(s => s.IsScheduledOrRunning));
            }
        }

        public void Add(Run run)
        {
            lock (_session)
            {
                _session.Store(run);
            }
        }

        public void Update(Run run)
        {
            lock (_session)
            {
                _session.Store(run);
            }
        }

        public void Remove(Guid id)
        {
            lock (_session)
            {
                var run = Find(id);
                if (run is null)
                {
                    return;
                }

                _session.Delete(run);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public PagedQueryResult<RunExtendedSearchModel> Search(RunSearchQueryModel queryModel, StringMatchRules freeTextMatchRules)
        {
            lock (_session)
            {
                var query = _session.Query<RunReducedResult, Runs_BySearch>();
                query = QueryFreeText(query, queryModel);  // Basic filtering
                query = QueryRunPeriod(query, queryModel);
                query = QueryExecutedStartDate(query, queryModel);
                query = QueryAuthors(query, queryModel);
                query = QueryStatus(query, queryModel);
                query = query.OrderByMultipleItems(queryModel.Orderby);

                // If there is a valid description then do not apply paging now,
                // since we need to filter further by applying free text matching rules
                var shouldApplyFreeTextMatchRules = !string.IsNullOrWhiteSpace(queryModel.Description);
                query = shouldApplyFreeTextMatchRules ?
                        query :
                        query.ApplyDefaultPaging(queryModel.Skip, queryModel.Top);

                // query run with additional fields for "ApplyFreeTextMatchRules" method
                var extendedSearchQuery = query.TransformWith<RunsExtendedSearch_Transformer, RunExtendedSearchModel>();

                int totalCount = query.Count();
                IList<RunExtendedSearchModel> runs = extendedSearchQuery.ToList();

                if (shouldApplyFreeTextMatchRules && totalCount > 0)
                {
                    // Filter runs, necessary because Raven cannot filter sufficiently. This is nasty
                    // but necessary because Raven cannot filter the data as required
                    runs = ApplyFreeTextMatchRules(runs, freeTextMatchRules, queryModel.Description);
                    totalCount = runs.Count;
                    runs = runs.ApplyDefaultPaging(queryModel.Skip, queryModel.Top).ToList();
                }

                return new PagedQueryResult<RunExtendedSearchModel>(totalCount, runs);
            }
        }

        private IList<RunExtendedSearchModel> ApplyFreeTextMatchRules(IList<RunExtendedSearchModel> runs, StringMatchRules freeTextMatchRules, string description)
        {
            if (string.IsNullOrWhiteSpace(description)) { return runs; }

            return runs.Where(r => freeTextMatchRules.IsMatches(r.Description, description) ||
                                   (!string.IsNullOrWhiteSpace(r.Author?.Name) &&
                                    freeTextMatchRules.IsMatches(r.Author.Name, description)) ||
                                   freeTextMatchRules.IsMatches(r.Id.ToString(), description) ||
                                   r.ScenarioIds.Any(id => freeTextMatchRules.IsMatches(id, description)) ||
                                   r.ScenarioNames.Any(name => freeTextMatchRules.IsMatches(name, description)) ||
                                   r.PassNames.Any(name => freeTextMatchRules.IsMatches(name, description)) ||
                                   r.PassIds.Any(id => freeTextMatchRules.IsMatches(id, description))).ToList();
        }

        private IRavenQueryable<RunReducedResult> QueryFreeText(IRavenQueryable<RunReducedResult> query, RunSearchQueryModel queryModel)
        {
            if (string.IsNullOrEmpty(queryModel.Description))   // No filter
            {
                return query;
            }

            // Returns runs containing any of the search words, allows further local filtering depending on string match rules
            var escapedTerm = "*" + RavenQuery.Escape(queryModel.Description.Replace("-", Runs_BySearch.DashReplacer), true, false) + "*";
            query = query.Search(r => r.Query, escapedTerm, options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);
            return query;
        }

        private IRavenQueryable<RunReducedResult> QueryRunPeriod(IRavenQueryable<RunReducedResult> query, RunSearchQueryModel queryModel)
        {
            return (queryModel.RunPeriodStartDate.HasValue && queryModel.RunPeriodEndDate.HasValue) ?
                    query.Where(x => (x.StartDate <= queryModel.RunPeriodEndDate) &&
                                     (x.EndDate >= queryModel.RunPeriodStartDate)) :
                    query;
        }

        private IRavenQueryable<RunReducedResult> QueryExecutedStartDate(IRavenQueryable<RunReducedResult> query, RunSearchQueryModel queryModel)
        {
            return (queryModel.ExecutedStartDate.HasValue && queryModel.ExecutedEndDate.HasValue) ?
                    query.Where(x => (x.ExecuteStartedDateTime >= queryModel.ExecutedStartDate) &&
                                     (x.ExecuteStartedDateTime <= queryModel.ExecutedEndDate)) :
                    query;
        }

        private IRavenQueryable<RunReducedResult> QueryAuthors(IRavenQueryable<RunReducedResult> query, RunSearchQueryModel queryModel)
        {
            return (queryModel.Users?.Count() > 0) ?
                    query.Where(x => x.AuthorId.In(queryModel.Users.ToArray())) :
                    query;
        }

        private IRavenQueryable<RunReducedResult> QueryStatus(IRavenQueryable<RunReducedResult> query, RunSearchQueryModel queryModel)
        {
            return (queryModel.Status?.Count() > 0) ?
                    query.Where(x => x.RunStatus.In(queryModel.Status)) :
                    query;
        }

        public IEnumerable<Run> GetRunsByCampaignExternalIdsAndStatus(IEnumerable<string> externalIds, RunStatus runStatus)
        {
            lock (_session)
            {
                return _session
                    .GetAll<Run>()
                    .Where(run =>
                        run.RunStatus == runStatus && (
                            run.Campaigns.Count == 0 ||
                            run.Campaigns.Any(c => externalIds.Contains(c.ExternalId))
                        ));
            }
        }

        public IEnumerable<Run> GetRunsByDeliveryCappingGroupId(int id) =>
            _session.GetAll<Run>(x => x.CampaignsProcessesSettings.Any(y => y.DeliveryCappingGroupId == id));

        public void UpdateRange(IEnumerable<Run> runs)
        {
            if (runs != null && runs.Any())
            {
                lock (_session)
                {
                    foreach (var run in runs)
                    {
                        _session.Store(run);
                    }
                }
            }
        }

        public bool Exists(Guid id) => Find(id) != null;
    }
}
