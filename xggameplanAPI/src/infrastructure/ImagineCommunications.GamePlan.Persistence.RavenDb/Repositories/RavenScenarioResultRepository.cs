using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenScenarioResultRepository : IScenarioResultRepository
    {
        private readonly IDocumentSession _session;

        public RavenScenarioResultRepository(IDocumentSession session)
        {
            _session = session;
        }

        public ScenarioResult Find(Guid scenarioId)
        {
            ScenarioResult scenarioResult = null;
            lock (_session)
            {
                scenarioResult = _session.Load<ScenarioResult>(scenarioId);
            }
            return scenarioResult;
        }

        public IEnumerable<ScenarioResult> Find(Guid[] scenarioIds) => _session.GetAll<ScenarioResult>(x => x.Id.In(scenarioIds)).ToList();

        public IEnumerable<ScenarioResult> GetAll()
        {
            List<ScenarioResult> scenarioResult = null;
            lock (_session)
            {
                scenarioResult = _session.Query<ScenarioResult>().Take(Int32.MaxValue).ToList();
            }
            return scenarioResult;
        }

        public void Add(ScenarioResult scenarioResult)
        {
            lock (_session)
            {
                _session.Store(scenarioResult);
            }
        }

        public void Update(ScenarioResult scenarioResult)
        {
            lock (_session)
            {
                _session.Store(scenarioResult);
            }
        }

        public void UpdateRange(IEnumerable<ScenarioResult> items)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    items.ToList().ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public void Remove(Guid scenarioId)
        {
            lock (_session)
            {
                _session.Delete<ScenarioResult>(scenarioId);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
