using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Helpers;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSpotRepository : ISpotRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenSpotRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public void Add(IEnumerable<Spot> items)
        {
            var options = new BulkInsertOptions() { OverwriteExisting = true };
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
            {
                items
                    .ToList()
                    .ForEach(item => bulkInsert.Store(item));
            }
        }

        public void Add(Spot item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Update(Spot item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public IEnumerable<Spot> FindByExternal(string campref) =>
            _session
                .GetAll((Expression<Func<Spot, bool>>)(currentItem => currentItem.ExternalCampaignNumber == campref))
                .ToList();

        public Spot FindByExternalSpotRef(string externalSpotRef) =>
            _session
                .Query<Spot>()
                .FirstOrDefault(currentItem => currentItem.ExternalSpotRef == externalSpotRef);

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, string salesarea)
        {
            lock (_session)
            {
                var items = _session.GetAll<Spot>(currentItem =>
                    currentItem.SalesArea == salesarea &&
                    currentItem.StartDateTime >= datefrom &&
                    currentItem.StartDateTime <= dateto,
                    indexName: Spots_ByManyFields.DefaultIndexName,
                    isMapReduce: false);

                return items.ToList();
            }
        }

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, List<string> salesareas)
        {
            lock (_session)
            {
                var items = _session
                    .GetAll<Spot>(s =>
                         s.StartDateTime >= datefrom && s.StartDateTime <= dateto && s.SalesArea.In(salesareas),
                         indexName: Spots_ByManyFields.DefaultIndexName,
                         isMapReduce: false)
                    .ToList();

                return items;
            }
        }

        public Spot Find(Guid id) =>
            _session
                .Query<Spot>()
                .FirstOrDefault(currentItem => currentItem.Uid == id);

        public IEnumerable<Spot> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Spot>();
            }
        }

        public IEnumerable<Spot> GetAllMultipart() =>
            _session
                .GetAll<Spot>(s => s.MultipartSpot.In(MultipartSpotTypes.All))
                .ToList();

        public IEnumerable<Spot> GetAllByCampaign(string campaignExternalId) =>
            _session
                .GetAll<Spot>(s => s.ExternalCampaignNumber == campaignExternalId)
                .ToList();

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<Spot>();
                }
            }
        }

        public int Count(Expression<Func<Spot, bool>> query)
        {
            lock (_session)
            {
                return _session
                    .Query<Spot>()
                    .Where(query)
                    .Count();
            }
        }

        public decimal GetNominalPriceByCampaign(string campaignExternalId) =>
            _session
                .GetAll<Spot>(s =>
                    s.ExternalCampaignNumber == campaignExternalId &&
                    s.ClientPicked == false &&
                    s.ExternalBreakNo != null &&
                    s.ExternalBreakNo != string.Empty &&
                    !s.ExternalBreakNo.Equals(Globals.UnplacedBreakString, StringComparison.InvariantCultureIgnoreCase))
                .Sum(s => s.NominalPrice);

        public void Remove(Guid uid)
        {
            lock (_session)
            {
                Spot spotToDelete = Find(uid);
                if (spotToDelete is null)
                {
                    return;
                }

                _session.Delete(spotToDelete);
            }
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                var spots = new List<Spot>();
                var groups = RavenRepositoryHelper.GroupElementsForInClause(ids.Distinct());

                foreach (var group in groups)
                {
                    spots.AddRange(_session.GetAll<Spot>(s => s.Uid.In(group.ToList())));
                }

                foreach (var spot in spots)
                {
                    _session.Delete(spot);
                }
            }
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "Spots");
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
                        .DeleteByIndexAsync<Spot, Spots_ByManyFields>(ForceDelete())
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
                        $"Deleting all Spot documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                        );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Spot documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
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

            Expression<Func<Spot, bool>> ForceDelete() =>
                spot => spot.Uid != Guid.Empty;
        }

        public IEnumerable<Spot> FindByExternal(List<string> externalRef) =>
            _session
                .GetAll<Spot>(s => s.ExternalSpotRef.In(externalRef))
                .ToList();

        public IEnumerable<Spot> FindByExternalBreakNumbers(IEnumerable<string> externalBreakNumbers)
        {
            var spots = new List<Spot>();
            var groups = RavenRepositoryHelper.GroupElementsForInClause(externalBreakNumbers);

            foreach (var itemGroup in groups)
            {
                spots.AddRange(
                    _session.GetAll<Spot>(s =>
                            s.ExternalBreakNo.In(itemGroup.ToList()
                                ),
                        Spots_ByManyFields.DefaultIndexName,
                        isMapReduce: false
                        )
                    );
            }

            return spots;
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
            string fakeSalesAreaNameToForceIndexLookup = Guid.NewGuid().ToString();

            _session.WaitForIndexes<Spot>(
                Spots_ByManyFields.DefaultIndexName,
                isMapReduce: false,
                testExpression: spot => spot.SalesArea != fakeSalesAreaNameToForceIndexLookup
                );
        }

        public void InsertOrReplace(IEnumerable<Spot> items)//HOTFIX TO PASS RAVEN TESTS DURING BUILD!!!
        {
            lock (_session)
            {
                var spotsToReplace = FindByExternal(items.Select(e => e.ExternalSpotRef).ToList());
                foreach (var spotToReplace in spotsToReplace)
                {
                    _session.Delete(spotToReplace);
                }

                foreach (var spot in items)
                {
                    _session.Store(spot);
                }
            }
        }
    }
}
