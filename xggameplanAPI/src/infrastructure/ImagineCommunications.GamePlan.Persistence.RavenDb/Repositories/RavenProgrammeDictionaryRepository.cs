using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenProgrammeDictionaryRepository : IProgrammeDictionaryRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        /// <summary>
        /// Load Raven session value
        /// </summary>
        /// <param name="session">Raven DocumentSession</param>
        public RavenProgrammeDictionaryRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public ProgrammeDictionary Find(int id)
        {
            return _session.Load<ProgrammeDictionary>(id);
        }

        public IEnumerable<ProgrammeDictionary> FindByExternal(List<string> externalRefs)
        {
            return _session.GetAll<ProgrammeDictionary>().Where(p => p.ExternalReference.In(externalRefs)).ToList();
        }

        public IEnumerable<ProgrammeDictionary> GetAll()
        {
            return _session.GetAll<ProgrammeDictionary>().ToList();
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<ProgrammeDictionary>();
                }
            }
        }

        //public async Task TruncateAsync()
        //{
        //    const int maximumTimeoutSeconds = 180;
        //    const int retryMillisecondDelay = 100;
        //    const int maximumNumberOfRetries = 100;

        //    var maximumSecondsWaitingForNonStaleIndexes = TimeSpan.FromSeconds(maximumTimeoutSeconds);
        //    int remainingRetries = maximumNumberOfRetries;
        //    bool retry = false;
        //    var startTime = DateTime.UtcNow;

        //    do
        //    {
        //        retry = false;

        //        try
        //        {
        //            var operation = await _sessionAsync.Advanced
        //                .DeleteByIndexAsync<ProgrammeDictionary, ProgrammeDictionaries_ByExternalReferenceAndId>(ForceDelete())
        //                .ConfigureAwait(false);

        //            await operation
        //                .WaitForCompletionAsync()
        //                .ConfigureAwait(false);
        //        }
        //        catch (Exception ex) when (CanRetry(ex))
        //        {
        //            remainingRetries--;
        //            retry = true;

        //            await Task.Delay(retryMillisecondDelay)
        //                .ConfigureAwait(false);

        //            continue;
        //        }
        //        catch (Exception ex) when (remainingRetries == 0)
        //        {
        //            throw new RepositoryException(
        //                $"Deleting all ProgrammeDictionary documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
        //                ex
        //            );
        //        }
        //        catch (Exception ex) when (IndexIsStale(ex))
        //        {
        //            throw new RepositoryException(
        //                $"Deleting all ProgrammeDictionary documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
        //                ex
        //            );
        //        }
        //    } while (retry);

        //    bool IndexIsStale(Exception ex) => ex.Message.Contains("index is stale");

        //    bool MaximumTimeToWaitForIndexesExceeded() =>
        //        DateTime.UtcNow - startTime > maximumSecondsWaitingForNonStaleIndexes;

        //    bool CanRetry(Exception ex) =>
        //        IndexIsStale(ex) &&
        //        remainingRetries > 0 &&
        //        !MaximumTimeToWaitForIndexesExceeded();

        //    Expression<Func<ProgrammeDictionary, bool>> ForceDelete() =>
        //        programmeDictionary => programmeDictionary.Id != int.MinValue;
        //}
    }
}
