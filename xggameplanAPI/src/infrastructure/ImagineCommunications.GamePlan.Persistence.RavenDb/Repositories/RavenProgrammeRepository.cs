using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenProgrammeRepository : IProgrammeRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenProgrammeRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public void Add(IEnumerable<Programme> items)
        {
            var prgDict = _session.GetAll<ProgrammeDictionary>().ToDictionary(k => k.ExternalReference);
            var prgDictToProcess = new HashSet<ProgrammeDictionary>();

            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert())
            {
                foreach (var item in items)
                {
                    if (item.Id == Guid.Empty)
                    {
                        item.Id = Guid.NewGuid();
                    }

                    _ = bulkInsert.Store(item);

                    if (!prgDict.TryGetValue(item.ExternalReference, out var pd))
                    {
                        pd = new ProgrammeDictionary { ExternalReference = item.ExternalReference };
                        prgDict.Add(item.ExternalReference, pd);
                    }

                    pd.ProgrammeName = item.ProgrammeName;
                    pd.Description = item.Description;
                    pd.Classification = item.Classification;

                    _ = prgDictToProcess.Add(pd);
                }
            }

            using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(
                options: new BulkInsertOptions { OverwriteExisting = true }))
            {
                foreach (var item in prgDictToProcess)
                {
                    _ = bulkInsert.Store(item);
                }
            }
        }

        public void Add(Programme item)
        {
            lock (_session)
            {
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                _session.Store(item);

                var pd = _session.Query<ProgrammeDictionary>()
                    .FirstOrDefault(x => x.ExternalReference == item.ExternalReference);

                var isNew = false;
                if (pd is null)
                {
                    pd = new ProgrammeDictionary { ExternalReference = item.ExternalReference };
                    isNew = true;
                }

                pd.ProgrammeName = item.ProgrammeName;
                pd.Description = item.Description;
                pd.Classification = item.Classification;

                if (isNew)
                {
                    _session.Store(pd);
                }
            }
        }

        [Obsolete("Use Get()")]
        public Programme Find(Guid id) => Get(id);

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                var programmes = _session.GetAll<Programme>(s => s.Id.In(ids));

                foreach (var programme in programmes)
                {
                    _session.Delete(programme);
                }
            }
        }

        public Programme Get(Guid id)
        {
            lock (_session)
            {
                var item = _session.Load<Programme>(id);
                return item;
            }
        }

        public IEnumerable<Programme> Search(DateTime datefrom, DateTime dateto, string salesarea)
        {
            lock (_session)
            {
                var items = _session.GetAll<Programme>(currentItem => currentItem.SalesArea == salesarea && currentItem.StartDateTime >= datefrom && currentItem.StartDateTime <= dateto,
                                                    indexName: Programmes_ByIdAndSalesAreaStartDateTime.DefaultIndexName, isMapReduce: false);
                return items.ToList();
            }
        }

        public PagedQueryResult<ProgrammeNameModel> Search(ProgrammeSearchQueryModel searchQuery)
        {
            lock (_session)
            {
                var where = new List<Expression<Func<Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields, bool>>>();

                if (searchQuery.SalesArea != null && searchQuery.SalesArea.Any())
                {
                    where.Add(p => p.SalesArea.In(searchQuery.SalesArea));
                }

                if (searchQuery.FromDateInclusive != default(DateTime))
                {
                    where.Add(p => p.StartDateTime >= searchQuery.FromDateInclusive);
                }

                if (searchQuery.ToDateInclusive != default(DateTime))
                {
                    where.Add(p => p.StartDateTime <= searchQuery.ToDateInclusive);
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.NameOrRef))
                {
                    var termsList = searchQuery.NameOrRef.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    where.AddRange(termsList.Select(
                        term => (Expression<Func<Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields, bool>>)(p => p
                           .TokenizedProgramme
                           .StartsWith(term))));
                }

                string sortBy;
                switch (searchQuery.OrderBy)
                {
                    case ProgrammeOrder.LocalDate:
                        sortBy = "StartDateTime";
                        break;

                    case null:
                        sortBy = null;
                        break;

                    default:
                        sortBy = searchQuery.OrderBy.ToString();
                        break;
                }

                var items = DocumentSessionExtensions
                    .GetAll<Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields, Programmes_ByIdAndSalesAreaStartDateTime,
                        ProgrammeTransformer_BySearch, ProgrammeNameModel>(_session,
                        where.Any() ? where.AggregateAnd() : null,
                        out int totalResult, sortBy, searchQuery.OrderByDirection?.ToString() ?? "asc",
                        searchQuery.Skip,
                        searchQuery.Top);

                return new PagedQueryResult<ProgrammeNameModel>(totalResult, items);
            }
        }

        public IEnumerable<Programme> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<Programme>().ToList();
            }
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<Programme>();
                }
            }
        }

        public int Count(Expression<Func<Programme, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<Programme>().Where(query).Count();
            }
        }

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            lock (_session)
            {
                var item = Get(uid);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate() =>
            _session.TryDelete("Raven/DocumentsByEntityName", "Programmes");

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
                        .DeleteByIndexAsync<Programme, Programmes_ByIdAndSalesAreaStartDateTime>(ForceDelete())
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
                        $"Deleting all Programme documents stopped after {maximumNumberOfRetries} attempts. " +
                        "Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Programme documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. " +
                        "Wait for a few minutes and try again.",
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

            Expression<Func<Programme, bool>> ForceDelete() =>
                programme => programme.Id != Guid.Empty;
        }

        public IEnumerable<Programme> FindByExternal(string externalRef)
        {
            lock (_session)
            {
                return _session.GetAll<Programme>(p => p.ExternalReference == externalRef).ToList();
            }
        }

        public IEnumerable<Programme> FindByExternal(List<string> externalRefs)
        {
            lock (_session)
            {
                return _session.GetAll<Programme>(p => p.ExternalReference.In(externalRefs)).ToList();
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
        /// <summary> Identifies whether any record exists for the supplied
        /// condition </summary> <param name="condition">The condition of type
        /// <see cref="Expression<Func<Programme, bool>>"/></param>
        /// <returns>True if any record exists for the supplied condition or
        /// False otherwise</returns>
        public bool Exists(Expression<Func<Programme, bool>> condition)
        {
            bool exist;
            lock (_session)
            {
                exist = _session.Query<Programme>().Any(condition);
            }

            return exist;
        }
    }
}
