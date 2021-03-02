using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Extensions;
using BulkInsertOptions = ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext.BulkInsertOptions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenScenarioCampaignFailureRepository : IScenarioCampaignFailureRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenScenarioCampaignFailureRepository(IDocumentSession session) => _session = session;

        public void Add(ScenarioCampaignFailure scenarioCampaignFailure)
        {
            lock (_session)
            {
                _session.Store(scenarioCampaignFailure);
            }
        }

        public void AddRange(IEnumerable<ScenarioCampaignFailure> scenarioCampaignFailures, bool setIdentity = true)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    scenarioCampaignFailures.ToList().ForEach(item =>
                        bulkInsert.Store(item));
                }
            }
        }

        public void Delete(int Id)
        {
            lock (_session)
            {
                var item = Get(Id);
                if (item is null)
                {
                    return;
                }
                _session.Delete(item);
            }
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            lock (_session)
            {
                _session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(ScenarioCampaignFailures_BySearch.DefaultIndexName,
                  new IndexQuery()
                  {
                      Query = $"ScenarioId:{scenarioId}"
                  }).WaitForCompletion();
                _session.SaveChanges();
            }
        }

        public IEnumerable<ScenarioCampaignFailure> FindByScenarioId(Guid scenarioId) =>
            _session.GetAll<ScenarioCampaignFailure>(s => s.ScenarioId == scenarioId);

        public ScenarioCampaignFailure Get(int Id) =>
            _session.Load<ScenarioCampaignFailure>(Id);

        public IEnumerable<ScenarioCampaignFailure> GetAll() => _session.GetAll<ScenarioCampaignFailure>();

        public PagedQueryResult<ScenarioCampaignFailure> Search(ScenarioCampaignFailureSearchQueryModel searchQuery)
        {
            var where = new List<Expression<Func<ScenarioCampaignFailures_BySearch.IndexedFields, bool>>>();
            where.Add(p => p.ScenarioId == searchQuery.ScenarioId);
            if (searchQuery.SalesAreaGroupNames != null && searchQuery.SalesAreaGroupNames.Any())
            {
                where.Add(p => p.SalesAreaGroup.In(searchQuery.SalesAreaGroupNames));
            }

            if (searchQuery.ExternalCampaignIds != null && searchQuery.ExternalCampaignIds.Any())
            {
                where.Add(p => p.ExternalCampaignId.In(searchQuery.ExternalCampaignIds));
            }

            Expression<Func<ScenarioCampaignFailures_BySearch.IndexedFields, bool>> exp = e => false;

            if (searchQuery.StrikeWeights?.Count() > 0)
            {
                var first = true;
                foreach (var strikeWeight in searchQuery.StrikeWeights)
                {
                    Expression<Func<ScenarioCampaignFailures_BySearch.IndexedFields, bool>> e = x => false;

                    if (strikeWeight.StrikeWeightStartDate.HasValue)
                    {
                        e = scf => scf.StrikeWeightStartDate > strikeWeight.StrikeWeightStartDate.Value.Date.AddDays(-1);

                        if (strikeWeight.StrikeWeightEndDate.HasValue)
                        {
                            e = e.And(scf => scf.StrikeWeightEndDate <= strikeWeight.StrikeWeightEndDate.Value.Date);
                        }
                    }
                    else if (strikeWeight.StrikeWeightEndDate.HasValue)
                    {
                        e = scf => scf.StrikeWeightEndDate <= strikeWeight.StrikeWeightEndDate.Value.Date;
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

                where.Add(exp);
            }

            var items = DocumentSessionExtensions
                .GetAll<ScenarioCampaignFailures_BySearch.IndexedFields,
                ScenarioCampaignFailures_BySearch,
                ScenarioCampaignFailureTransformer_BySearch,
                ScenarioCampaignFailure>(
                    _session,
                    where.Any() ? where.AggregateAnd() : null,
                    out int totalResult,
                    null,
                    null,
                    searchQuery.Skip,
                    searchQuery.Top);

            return new PagedQueryResult<ScenarioCampaignFailure>(totalResult, items);
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Dispose()
        {
            _session?.Advanced.Clear();
            _session?.Dispose();
        }
    }
}
