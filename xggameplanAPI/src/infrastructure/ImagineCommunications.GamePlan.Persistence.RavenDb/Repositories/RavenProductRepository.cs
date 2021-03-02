using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers;
using NodaTime;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenProductRepository : IProductRepository
    {
        private const int MaxClauseCount = 1000;
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;
        private readonly IClock _clock;

        public RavenProductRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync, IClock clock)
        {
            _session = session;
            _sessionAsync = sessionAsync;
            _clock = clock;
        }

        public void Add(IEnumerable<Product> items)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    items.ToList().ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public void Add(Product item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            var products = _session.GetAll<Product>(s => s.Externalidentifier.In(externalRefs.ToList()));
            foreach (var product in products)
            {
                _session.Delete(product);
            }
        }

        public void Update(Product item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public IEnumerable<Product> FindByExternal(string productref) =>
            FindByExternal(productref, _clock.GetCurrentInstant().ToDateTimeUtc());

        public IEnumerable<Product> FindByExternal(List<string> productRefs) =>
            FindByExternal(productRefs, _clock.GetCurrentInstant().ToDateTimeUtc());

        public IEnumerable<ProductAdvertiserModel> GetAdvertisers(ICollection<string> advertiserIds)
        {
            var distinctAdvertiserIds = advertiserIds.Distinct().ToList();
            var result = new Dictionary<string, ProductAdvertiserModel>();
            for (int i = 0, page = 0; i < distinctAdvertiserIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctAdvertiserIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                var advertisersBatch = _session.GetAllUsingProjection<Product, Product_BySearch, ProductAdvertiserModel>(x =>
                    x.AdvertiserIdentifier.In(ids));

                foreach (var item in advertisersBatch)
                {
                    if (!result.ContainsKey(item.AdvertiserIdentifier))
                    {
                        result.Add(item.AdvertiserIdentifier, item);
                    }
                }
            }

            return result.Values;
        }

        public IEnumerable<ProductAgencyModel> GetAgencies(ICollection<string> agencyIds)
        {
            var distinctAgencyIds = agencyIds.Distinct().ToList();
            var result = new Dictionary<string, ProductAgencyModel>();
            for (int i = 0, page = 0; i < distinctAgencyIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctAgencyIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                var agenciesBatch = _session.GetAllUsingProjection<Product, Product_BySearch, ProductAgencyModel>(x =>
                    x.AgencyIdentifier.In(ids));

                foreach (var item in agenciesBatch)
                {
                    if (!result.ContainsKey(item.AgencyIdentifier))
                    {
                        result.Add(item.AgencyIdentifier, item);
                    }
                }
            }

            return result.Values;
        }

        public IEnumerable<AgencyGroup> GetAgencyGroups(ICollection<string> agencyGroupIds)
        {
            var distinctAgencyGroupIds = agencyGroupIds.Distinct().ToList();
            var result = new Dictionary<string, AgencyGroup>();
            for (int i = 0, page = 0; i < distinctAgencyGroupIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctAgencyGroupIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                var agencyGroupsBatch = _session.GetAll<Product>(x =>
                    x.AgencyGroup != null && x.AgencyGroup.Code.In(ids));

                foreach (var item in agencyGroupsBatch)
                {
                    if (!result.ContainsKey(item.AgencyGroup.Code))
                    {
                        result.Add(item.AgencyGroup.Code, item.AgencyGroup);
                    }
                }
            }

            return result.Values;
        }

        public IEnumerable<ProductNameModel> GetByExternalIds(ICollection<string> externalIds)
        {
            var distinctExternalIds = externalIds.Distinct().ToList();
            var result = new List<ProductNameModel>();
            for (int i = 0, page = 0; i < distinctExternalIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctExternalIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_session.GetAllUsingProjection<Product, Product_BySearch, ProductNameModel>(x =>
                    x.Externalidentifier.In(ids)));
            }

            return result;
        }

        public IEnumerable<string> GetReportingCategories() =>
            _session.GetAllWithSelect<Product, string>(x => x.ReportingCategory != null && x.ReportingCategory != string.Empty,
                x => x.ReportingCategory, Product_BySearch.DefaultIndexName, false).Distinct().ToList();

        public IEnumerable<SalesExecutiveModel> GetSalesExecutives(ICollection<int> salesExecIds)
        {
            var result = new Dictionary<int, SalesExecutiveModel>();
            for (int i = 0, page = 0; i < salesExecIds.Count; i += MaxClauseCount, page++)
            {
                var ids = salesExecIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                var salesExecutivesBatch = _session.GetAll<Product>(x =>
                    x.SalesExecutive != null && x.SalesExecutive.Identifier.In(ids));

                foreach (var item in salesExecutivesBatch)
                {
                    if (!result.ContainsKey(item.SalesExecutive.Identifier))
                    {
                        result.Add(item.SalesExecutive.Identifier, new SalesExecutiveModel
                        {
                            Identifier = item.SalesExecutive.Identifier,
                            Name = item.SalesExecutive.Name
                        });
                    }
                }
            }

            return result.Values;
        }

        [Obsolete("Use Get()")]
        public Product Find(Guid id) => Get(id);

        public Product Get(Guid id) => Get(id, _clock.GetCurrentInstant().ToDateTimeUtc());

        public Product Get(Guid uid, DateTime onDate)
        {
            var item = _session.Query<Product>().FirstOrDefault(currentItem => currentItem.Uid == uid);
            return item;
        }

        public IEnumerable<Product> GetAll(DateTime onDate) => _session.GetAll<Product>().ToList();

        public IEnumerable<Product> GetAll() => GetAll(_clock.GetCurrentInstant().ToDateTimeUtc());

        [Obsolete("Should be renamed to Count()")]
        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<Product>();
                }
            }
        }

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            var item = Get(uid);

            if (item != null)
            {
                _session.Delete<Product>(item);
            }
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "Products");
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
                        .DeleteByIndexAsync<Product, Products_ByUid>(ForceDelete())
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
                        $"Deleting all Product documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Product documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
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

            Expression<Func<Product, bool>> ForceDelete() =>
                product => product.Uid != Guid.Empty;
        }

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds) =>
            FindByAdvertiserId(advertiserIds, _clock.GetCurrentInstant().ToDateTimeUtc());

        public PagedQueryResult<ProductAdvertiserModel> Search(AdvertiserSearchQueryModel searchQuery, DateTime onDate)
        {
            lock (_session)
            {
                var where = new List<Expression<Func<Product_BySearch.IndexedFields, bool>>>();
                if (!string.IsNullOrWhiteSpace(searchQuery.AdvertiserNameorRef))
                {
                    var termsList =
                        searchQuery.AdvertiserNameorRef.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    where.AddRange(termsList.Select(
                        term => (Expression<Func<Product_BySearch.IndexedFields, bool>>)(p => p.TokenizedAdvertiser
                           .StartsWith(term))));
                }
                var items = DocumentSessionExtensions
                    .GetAll<Product_BySearch.IndexedFields, Product_BySearch, ProductAdvertiserTransformer_BySearch, ProductAdvertiserModel>(_session,
                        where.Any() ? where.AggregateAnd() : null,
                        out int totalResult, null, null,
                        searchQuery.Skip,
                        searchQuery.Top);

                var totalCount = searchQuery.IncludeTotalCount ? totalResult : 0;

                return new PagedQueryResult<ProductAdvertiserModel>(totalCount, items);
            }
        }

        public PagedQueryResult<ProductNameModel> Search(ProductSearchQueryModel searchQuery, DateTime onDate)
        {
            lock (_session)
            {
                var where = new List<Expression<Func<Product_BySearch.IndexedFields, bool>>>();
                if (!string.IsNullOrWhiteSpace(searchQuery.Externalidentifier))
                {
                    where.Add(p => p.Externalidentifier.Equals(searchQuery.Externalidentifier, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.Name))
                {
                    where.Add(p => p.Name.Equals(searchQuery.Name, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.ClashCode))
                {
                    where.Add(p => p.ClashCode.Equals(searchQuery.ClashCode, StringComparison.OrdinalIgnoreCase));
                }

                if (searchQuery.FromDateInclusive != default(DateTime))
                {
                    where.Add(p => p.EffectiveStartDate >= searchQuery.FromDateInclusive);
                }

                if (searchQuery.ToDateInclusive != default(DateTime))
                {
                    where.Add(p => p.EffectiveEndDate < searchQuery.ToDateInclusive.AddTicks(1));
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.NameOrRef))
                {
                    var termsList = searchQuery.NameOrRef.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    where.AddRange(termsList.Select(
                        term => (Expression<Func<Product_BySearch.IndexedFields, bool>>)(p => p.TokenizedName
                           .StartsWith(term))));
                }
                string sortBy;
                switch (searchQuery.OrderBy)
                {
                    case ProductOrder.StartDate:
                        sortBy = "EffectiveStartDate";
                        break;

                    case ProductOrder.EndDate:
                        sortBy = "EffectiveEndDate";
                        break;

                    case null:
                        sortBy = null;
                        break;

                    default:
                        sortBy = searchQuery.OrderBy.ToString();
                        break;
                }

                var items = DocumentSessionExtensions
                    .GetAll<Product_BySearch.IndexedFields, Product_BySearch, ProductTransformer_BySearch, ProductNameModel>(_session,
                        where.Any() ? where.AggregateAnd() : null,
                        out int totalResult, sortBy, searchQuery.OrderByDirection?.ToString() ?? "asc",
                        searchQuery.Skip,
                        searchQuery.Top);

                var totalCount = searchQuery.IncludeTotalCount ? totalResult : 0;

                return new PagedQueryResult<ProductNameModel>(totalCount, items);
            }
        }

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds, DateTime onDate)
        {
            lock (_session)
            {
                return _session.GetAll<Product>(currentItem => currentItem.AdvertiserIdentifier.In(advertiserIds));
            }
        }

        public int Count(Expression<Func<Product, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<Product>().Where(query).Count();
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
        /// <see cref="Expression<Func<Product, bool>>"/></param> <returns>True
        /// if any record exists for the supplied condition or False otherwise</returns>
        public bool Exists(Expression<Func<Product, bool>> condition)
        {
            bool exist;
            lock (_session)
            {
                exist = _session.Query<Product>().Any(condition);
            }

            return exist;
        }

        public IEnumerable<Product> FindByExternal(string externalRef, DateTime onDate)
        {
            var items = _session.GetAll<Product>(currentItem => currentItem.Externalidentifier == externalRef);
            return items.ToList();
        }

        /// <summary>
        /// Find all product matching the given external product reference.
        /// </summary>
        /// <param name="externalProductReferences">
        /// <para>A list of external procduct references to search for.</para>
        /// <para>
        /// Note: To improve performance ensure the values passed in this list
        /// are distinct. The method does not check for you.
        /// </para>
        /// </param>
        /// <param name="onDate"></param>
        /// <returns></returns>
        public IEnumerable<Product> FindByExternal(
            List<string> externalProductReferences,
            DateTime onDate)
        {
            if (externalProductReferences is null || externalProductReferences.Count == 0)
            {
                return Enumerable.Empty<Product>();
            }

            return GetProducts_Impl(externalProductReferences.ToImmutableList());

            // Partition lists that RavenDB cannot itself partition.
            List<Product> GetProducts_Impl(IReadOnlyCollection<string> ids)
            {
                try
                {
                    return _session
                        .GetAll<Product>(currentItem =>
                            currentItem.Externalidentifier.In(ids),
                            indexName: Product_BySearch.DefaultIndexName,
                            isMapReduce: false);
                }
                catch (Exception ex) when (ex.Message.Contains("Exceeded maximum clause count in the query"))
                {
                    var midPoint = ids.Count / 2;
                    var left = ImmutableList.CreateRange(ids.Take(midPoint));
                    var right = ImmutableList.CreateRange(ids.Skip(midPoint).Take(midPoint + 1));

                    List<Product> leftResult = GetProducts_Impl(left);
                    List<Product> rightResult = GetProducts_Impl(right);

                    leftResult.AddRange(rightResult);

                    return leftResult;
                }
            }
        }

        public void DeleteByExternalIdentifier(IEnumerable<string> externalRefs)
        {
            var items = _session.GetAll<Product>(currentItem => currentItem.Externalidentifier.In(externalRefs));

            foreach (var item in items)
            {
                _session.Delete(item);
            }
        }
    }
}
