using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenClashRepository : IClashRepository
    {
        private const int MaxClauseCount = 1000;
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenClashRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public void Add(IEnumerable<Clash> items)
        {
            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                items.ToList().ForEach(item => _session.Store(item));
            }
        }

        public void Add(Clash item)
        {
            _session.Store(item);
        }

        [Obsolete("Use Get()")]
        public Clash Find(Guid id) => Get(id);

        public Clash Get(Guid id)
        {
            var item = _session
                .Query<Clash>()
                .FirstOrDefault(currentItem => currentItem.Uid == id);

            return item;
        }

        public IEnumerable<Clash> FindByExternal(string externalref)
        {
            var items = _session.GetAll<Clash>(currentItem => currentItem.Externalref == externalref);

            return items;
        }

        public IEnumerable<Clash> FindByExternal(List<string> externalRefs)
        {
            externalRefs = externalRefs.Distinct().ToList();
            var result = new List<Clash>();
            for (int i = 0, page = 0; i < externalRefs.Count; i += MaxClauseCount, page++)
            {
                var ids = externalRefs.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_session.GetAll<Clash>(currentItem => currentItem.Externalref.In(ids)));
            }

            return result;
        }

        public IEnumerable<Clash> GetAll()
        {
            return _session.GetAll<Clash>();
        }

        public IEnumerable<ClashNameModel> GetDescriptionByExternalRefs(ICollection<string> externalRefs)
        {
            var distinctExternalRefs = externalRefs.Distinct().ToList();
            var result = new List<ClashNameModel>();
            for (int i = 0, page = 0; i < distinctExternalRefs.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctExternalRefs.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_session.GetAllUsingProjection<Clash, Clash_BySearch, ClashNameModel>(x => x.Externalref.In(ids)));
            }

            return result;
        }

        public int Count()
        {
            lock (_session)
            {
                return _session.CountAll<Clash>();
            }
        }

        [Obsolete("Use Count()")]
        public int CountAll => Count();

        public void Delete(Guid uid)
        {
            var item = Get(uid);
            if (item is null)
            {
                return;
            }

            _session.Delete(item);
        }

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid, out bool isDeleted)
        {
            lock (_session)
            {
                isDeleted = false;

                var item = Get(uid);
                if (item != null)
                {
                    _session.Delete(item);
                    isDeleted = true;
                }
            }
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
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
                        .DeleteByIndexAsync<Clash, Clashes_ByUid>(ForceDelete())
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
                        $"Deleting all Clash documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Clash documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
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


            Expression<Func<Clash, bool>> ForceDelete() =>
                clash => clash.Uid != Guid.Empty;
        }

        public PagedQueryResult<ClashNameModel> Search(ClashSearchQueryModel queryModel)
        {
            lock (_session)
            {
                var where = new List<Expression<Func<Clash_BySearch.IndexedFields, bool>>>();


                if (!string.IsNullOrWhiteSpace(queryModel.NameOrRef))
                {
                    var termsList = queryModel.NameOrRef.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    where.AddRange(termsList.Select(
                        term => (Expression<Func<Clash_BySearch.IndexedFields, bool>>)(p => p.TokenizedName
                            .StartsWith(term))));
                }

                var items = DocumentSessionExtensions
                    .GetAll<Clash_BySearch.IndexedFields, Clash_BySearch, ClashTransformer_BySearch, ClashNameModel>(_session,
                        where.Any() ? where.AggregateAnd() : null,
                        out int totalResult, null, null,
                        queryModel.Skip,
                        queryModel.Top);

                return new PagedQueryResult<ClashNameModel>(totalResult, items);
            }
        }

        public int Count(Expression<Func<Clash, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<Clash>().Where(query).Count();
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
        /// <param name="condition">The condition of type <see cref="Expression<Func<Clash, bool>>"/></param>
        /// <returns>True if any record exists for the supplied condition or False otherwise</returns>
        public bool Exists(Expression<Func<Clash, bool>> condition)
        {
            bool exist;
            lock (_session)
            {
                exist = _session.Query<Clash>().Any(condition);
            }

            return exist;
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                var clashes = _session.GetAll<Clash>(s => s.Uid.In(ids));

                foreach (var clash in clashes)
                {
                    _session.Delete(clash);
                }
            }
        }

        public void UpdateRange(IEnumerable<Clash> clashes)
        {
            if (clashes != null && clashes.Any())
            {
                lock (_session)
                {
                    foreach (var clash in clashes)
                    {
                        _session.Store(clash);
                    }
                }
            }
        }
    }
}
