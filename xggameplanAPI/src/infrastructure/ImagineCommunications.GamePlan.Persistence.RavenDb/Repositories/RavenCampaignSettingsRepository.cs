using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenCampaignSettingsRepository : ICampaignSettingsRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenCampaignSettingsRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public void Add(CampaignSettings item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void AddRange(IEnumerable<CampaignSettings> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public int Count()
        {
            lock (_session)
            {
                return _session.CountAll<CampaignSettings>();
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item != null)
                {
                    _session.Delete(item);
                }
            }
        }

        public void DeleteByExternal(string externalId)
        {
            lock (_session)
            {
                var campaignSettings = GetByExternalId(externalId);

                _session.Delete(campaignSettings);
            }
        }

        public bool Exists(string externalId) => _session.Query<CampaignSettings>()
            .Any(cs => cs.CampaignExternalId == externalId);

        public IEnumerable<CampaignSettings> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<CampaignSettings>().ToList();
            }
        }

        public CampaignSettings Get(int id)
        {
            lock (_session)
            {
                return _session.Load<CampaignSettings>(id);
            }
        }

        public CampaignSettings GetByExternalId(string externalId)
        {
            lock (_session)
            {
                var item = _session.Query<CampaignSettings>().FirstOrDefault(cs => cs.CampaignExternalId == externalId);
                return item;
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(CampaignSettings item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "Clashes");
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
                        .DeleteByIndexAsync<CampaignSettings, CampaignSettings_ById>(ForceDelete())
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
                        $"Deleting all CampaignSettings documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all CampaignSettings documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
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

            Expression<Func<CampaignSettings, bool>> ForceDelete() =>
                CampaignSetting => CampaignSetting.Id != -1;
        }
    }
}
