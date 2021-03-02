using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSponsorshipRepository : ISponsorshipRepository
    {
        private readonly IDocumentSession _session;

        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenSponsorshipRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public IEnumerable<Sponsorship> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Sponsorship>(p =>
                   !p.Id.In(ForceRavenDbToStreamResults),
                   indexName: Sponsorship_ById.DefaultIndexName,
                   isMapReduce: false);
            }
        }

        public Sponsorship Get(string externalReferenceId)
        {
            var item = _session.Query<Sponsorship>().FirstOrDefault(currentItem => currentItem.ExternalReferenceId == externalReferenceId);
            return item;
        }

        /// <summary>
        /// An empty list of IDs is needed to force RavenDB to stream results.
        /// </summary>
        private static IEnumerable<int> ForceRavenDbToStreamResults => new List<int>() { 0 };

        public void Add(Sponsorship sponsorship)
        {
            lock (_session)
            {
                _session.Store(sponsorship);
            }
        }

        public void Update(Sponsorship sponsorship)
        {
            lock (_session)
            {
                _session.Store(sponsorship);
            }
        }

        public void Delete(string externalReferenceId)
        {
            var item = Get(externalReferenceId);

            if (item != null)
            {
                _session.Delete<Sponsorship>(item);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        // NOTE: If we use a RepositoryBase for our Repositories then we can move
        // Exists Method there to be available for re-use in all Repositories
        /// <summary>
        ///  Identifies whether any record exists for the supplied condition
        /// </summary>
        /// <param name="externalReferenceId"></param>
        /// <returns>True if any record exists for the supplied condition or False otherwise</returns>
        public bool Exists(string externalReferenceId)
        {
            var item = Get(externalReferenceId);
            
            return item != null;
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
                        .DeleteByIndexAsync<Sponsorship, Sponsorship_ById>(ForceRavenToDeleteTheDocument())
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

                    continue;
                }
                catch (Exception ex) when (remainingRetries == 0)
                {
                    throw new RepositoryException(
                        $"Deleting all Sponsorship documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Sponsorship documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
                        ex
                    );
                }
            } while (retry);

            bool IndexIsStale(Exception ex) => ex.Message.Contains("index is stale");

            bool MaximumTimeToWaitForIndexesExceeded() => DateTime.UtcNow - startTime > maximumSecondsWaitingForNonStaleIndexes;

            bool CanRetry(Exception ex) =>
                IndexIsStale(ex) &&
                remainingRetries > 0 &&
                !MaximumTimeToWaitForIndexesExceeded();

            static Expression<Func<Sponsorship, bool>> ForceRavenToDeleteTheDocument()
            {
                return sponsorship => sponsorship.Id != Int32.MinValue;
            }
        }
    }
}
