using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSmoothFailureRepository : ISmoothFailureRepository
    {
        private readonly IDocumentSession _session;

        public RavenSmoothFailureRepository(IDocumentSession session) => _session = session;

        public void AddRange(IEnumerable<SmoothFailure> items)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    foreach (var item in items)
                    {
                        bulkInsert.Store(item);
                    }
                }
            }
        }

        public IEnumerable<SmoothFailure> GetByRunId(Guid runId)
        {
            lock (_session)
            {
                var smoothFailures = _session
                    .GetAll<SmoothFailure>(
                        sf => sf.RunId == runId,
                        indexName: SmoothFailures_ByRunId.DefaultIndexName,
                        isMapReduce: false
                        )
                    .OrderBy(sf => sf.SalesArea)
                    .ThenBy(sf => sf.TypeId)
                    .ThenBy(sf => sf.ExternalSpotRef)
                    .ThenBy(sf => sf.ExternalBreakRef)
                    .ToList();

                return smoothFailures;
            }
        }

        public void RemoveByRunId(Guid runId)
        {
            lock (_session)
            {
                _session.Advanced.DocumentStore.DatabaseCommands
                    .DeleteByIndex(
                        SmoothFailures_ByRunId.DefaultIndexName,
                        new IndexQuery()
                        {
                            Query = $"RunId:{runId}"
                        })
                    .WaitForCompletion();

                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Truncate() => throw new NotImplementedException();
    }
}
