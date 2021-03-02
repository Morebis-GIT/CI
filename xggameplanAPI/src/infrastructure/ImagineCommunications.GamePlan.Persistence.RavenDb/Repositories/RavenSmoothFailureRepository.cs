using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
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
                        _ = bulkInsert.Store(item);
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
            const int maximumTimeoutSeconds = 180;
            const int retryMillisecondDelay = 100;
            const int maximumNumberOfRetries = 100;

            var maximumSecondsWaitingForNonStaleIndexes = TimeSpan.FromSeconds(maximumTimeoutSeconds);
            int remainingRetries = maximumNumberOfRetries;
            bool retry = false;
            var startTime = DateTime.UtcNow;

            do
            {
                retry = false;

                try
                {
                    _ = _session.Advanced.DocumentStore.DatabaseCommands
                            .DeleteByIndex(SmoothFailures_ByRunId.DefaultIndexName,
                                new IndexQuery()
                                {
                                    Query = $"RunId:{runId.ToString()}"
                                })
                            .WaitForCompletion();
                }
                catch (Exception ex) when (CanRetry(ex))
                {
                    remainingRetries--;
                    retry = true;

                    Task.Delay(retryMillisecondDelay).Wait();
                }
                catch (Exception ex) when (remainingRetries == 0)
                {
                    throw new RepositoryException(
                        $"Deleting all {nameof(SmoothFailure)} documents stopped after {maximumNumberOfRetries.ToString()} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all {nameof(SmoothFailure)} documents timed out after {maximumTimeoutSeconds.ToString()} seconds as the index is stale. Wait for a few minutes and try again.",
                        ex
                    );
                }
            } while (retry);

            bool IndexIsStale(Exception ex) => ex.Message.Contains("index is stale");

            bool MaximumTimeToWaitForIndexesExceeded() =>
                DateTime.UtcNow - startTime > maximumSecondsWaitingForNonStaleIndexes;

            bool CanRetry(Exception ex) =>
                IndexIsStale(ex) &&
                remainingRetries > 0 &&
                !MaximumTimeToWaitForIndexesExceeded();

            Expression<Func<SmoothFailure, bool>> ForceDelete() =>
                obj => obj.RunId != Guid.Empty;
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
