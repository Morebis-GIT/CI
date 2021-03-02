using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.common.Utilities;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenBreakRepository : IBreakRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenBreakRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        [Obsolete("Use the Delete() method")]
        public void Remove(Guid id) => Delete(id);

        [Obsolete("Use the Get() method")]
        public Break Find(Guid id) => Get(id);

        public void Add(Break item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        [Obsolete("Should be called AddRange() to match .NET conventions")]
        public void Add(IEnumerable<Break> items)
        {
            var options = new BulkInsertOptions() { OverwriteExisting = true };
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
            {
                items
                    .ToList()
                    .ForEach(item => bulkInsert.Store(item));
            }
        }

        public IEnumerable<Break> FindByExternal(string externalref)
        {
            lock (_session)
            {
                List<Break> results = _session.GetAll<Break>(x =>
                    x.ExternalBreakRef == externalref,
                        Breaks_ByManyFields.DefaultIndexName,
                        isMapReduce: false
                    );

                return results;
            }
        }

        public IEnumerable<Break> FindByExternal(List<string> externalRef)
        {
            lock (_session)
            {
                var breaks = new List<Break>();
                int page = 0;

                do
                {
                    List<string> pageExternalRefs = ListUtilities.GetPageItems(externalRef, 1000, page++);

                    if (pageExternalRefs.Count > 0)
                    {
                        breaks.AddRange(_session.GetAll<Break>(x =>
                            x.ExternalBreakRef.In(pageExternalRefs),
                            Breaks_ByManyFields.DefaultIndexName,
                            isMapReduce: false
                            )
                        );
                    }
                    else
                    {
                        return breaks;
                    }
                } while (true);
            }
        }

        public IEnumerable<Break> Search(DateTime dateFrom, DateTime dateTo, string salesArea)
        {
            lock (_session)
            {
                var items = _session.GetAll<Break>(currentItem =>
                        currentItem.SalesArea == salesArea &&
                        currentItem.ScheduledDate >= dateFrom &&
                        currentItem.ScheduledDate <= dateTo,
                    indexName: Breaks_ByManyFields.DefaultIndexName,
                    isMapReduce: false);

                return items;
            }
        }

        public IEnumerable<Break> Search(
            DateTimeRange scheduledDatesRange,
            IEnumerable<string> salesAreaNames
            )
        {
            var (dateFrom, dateTo) = scheduledDatesRange;

            lock (_session)
            {
                List<Break> items = _session.GetAll<Break>(
                    currentItem => currentItem.ScheduledDate >= dateFrom.ToUniversalTime()
                        && currentItem.ScheduledDate <= dateTo.ToUniversalTime()
                        && currentItem.SalesArea.In(salesAreaNames),
                    Breaks_ByManyFields.DefaultIndexName, false);

                return items;
            }
        }

        public IEnumerable<Break> Search(DateTime scheduledDate, string externalBreakRef, string salesArea)
        {
            var items = _session.GetAll<Break>(currentItem =>
                    currentItem.SalesArea == salesArea &&
                    currentItem.ScheduledDate == scheduledDate &&
                    currentItem.ExternalBreakRef == externalBreakRef,
                indexName: Breaks_ByManyFields.DefaultIndexName,
                isMapReduce: false);

            return items;
        }

        public IEnumerable<Break> SearchByBroadcastDateRange(DateTime dateFrom, DateTime dateTo, IEnumerable<string> salesAreaNames)
        {
            lock (_session)
            {
                var items = _session.GetAll<Break>(currentItem =>
                        currentItem.BroadcastDate >= dateFrom &&
                        currentItem.BroadcastDate <= dateTo &&
                        currentItem.SalesArea.In(salesAreaNames),
                    indexName: Breaks_ByManyFields.DefaultIndexName,
                    isMapReduce: false);

                return items;
            }
        }

        public Break Get(Guid id)
        {
            lock (_session)
            {
                return _session.Load<Break>(id);
            }
        }

        public IEnumerable<Break> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Break>();
            }
        }

        [Obsolete("Use Count()")]
        public int CountAll => Count();

        public int Count()
        {
            lock (_session)
            {
                return _session.CountAll<Break>();
            }
        }

        public int Count(Expression<Func<Break, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<Break>().Where(query).Count();
            }
        }

        public void Delete(Guid id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                foreach (var id in ids)
                {
                    var foundBreak = Get(id);
                    if (foundBreak != null)
                    {
                        _session.Delete(foundBreak);
                    }
                }
            }
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "Breaks");
        }

        public async Task TruncateAsync()
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
                    var operation = await _sessionAsync.Advanced
                        .DeleteByIndexAsync<Break, Breaks_ByManyFields>(ForceDelete())
                        .ConfigureAwait(false);

                    await operation
                        .WaitForCompletionAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (CanRetry(ex))
                {
                    remainingRetries--;
                    retry = true;

                    await Task.Delay(retryMillisecondDelay)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (remainingRetries == 0)
                {
                    throw new RepositoryException(
                        $"Deleting all Break documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Break documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
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

            Expression<Func<Break, bool>> ForceDelete() =>
                oneBreak => oneBreak.Id != Guid.Empty;
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public async Task SaveChangesAsync() =>
            await _sessionAsync.SaveChangesAsync()
                .ConfigureAwait(false);
    }
}
