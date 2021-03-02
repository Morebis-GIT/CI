using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.Common;
using xggameplan.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenClashExceptionRepository : IClashExceptionRepository
    {
        private readonly IDocumentSession _session;
        private readonly IMapper _mapper;

        public RavenClashExceptionRepository(IDocumentSession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        [Obsolete("Use the Get() method")]
        public ClashException Find(int id) => Get(id);

        public ClashExceptionModel GetWithDescriptions(int id)
        {
            var res = Get(id);
            return res is null ? null : GetClashExceptionModels(new[] { res }).FirstOrDefault();
        }

        public List<ClashExceptionModel> GetWithDescriptions(IEnumerable<int> ids)
        {
            var res = _session.GetAll<ClashException>(ce => ce.Id.In(ids));
            return GetClashExceptionModels(res);
        }

        [Obsolete("Use the Delete() method")]
        public void Remove(int id) => Delete(id);

        public void Add(ClashException item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public void Add(IEnumerable<ClashException> items)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    items.ToList().ForEach(item =>

                        bulkInsert.Store(item));
                }
            }
        }

        public IEnumerable<ClashException> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<ClashException>().ToList();
            }
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<ClashException>();
                }
            }
        }

        public ClashException Find(Guid uid)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid uid)
        {
            throw new NotImplementedException();
        }

        public ClashException Get(int id)
        {
            lock (_session)
            {
                return _session.Load<ClashException>(id);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                _session.Delete<ClashException>(id);
            }
        }

        public IEnumerable<ClashException> FindByExternal(string externalRef)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClashException> FindByExternal(List<string> externalRefs)
        {
            throw new NotImplementedException();
        }

        public void Truncate()
        {
            lock (_session)
            {
                _session.TryDelete("Raven/DocumentsByEntityName", "ClashExceptions");
            }
        }

        public IEnumerable<ClashException> Search(DateTime? dateRangeStart, DateTime? dateRangeEnd)
        {
            lock (_session)
            {
                var items = _session.GetAll<ClashException>();

                if (dateRangeStart != null)
                {
                    items = items.Where(p => p.StartDate.Date >= dateRangeStart.Value.Date).ToList();
                }

                if (dateRangeEnd != null)
                {
                    items = items.Where(p => p.EndDate == null ||
                                             p.EndDate.Value.Date < dateRangeEnd.Value.Date.AddDays(1)).ToList();
                }

                return items;
            }
        }

        public PagedQueryResult<ClashException> Search(ClashExceptionSearchQueryModel searchQuery)
        {
            lock (_session)
            {
                var items = _session.GetAll<ClashException>();

                if (searchQuery.StartDate != default(DateTime) && searchQuery.EndDate != default(DateTime))
                {
                    items = items.Where(c => (c.StartDate.Date >= searchQuery.StartDate.Date &&
                                              c.StartDate.Date < searchQuery.EndDate.Date.AddDays(1)) ||
                                             (c.EndDate != null && c.EndDate.Value.Date >= searchQuery.StartDate.Date &&
                                              c.EndDate.Value.Date < searchQuery.EndDate.Date.AddDays(1)) ||
                                             (c.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) &&
                                              (c.EndDate == null || c.EndDate.Value.Date >= searchQuery.EndDate.Date))).ToList();
                }
                else
                {
                    if (searchQuery.StartDate != default(DateTime) && searchQuery.EndDate == default(DateTime))
                    {
                        items = items.Where(c => (c.StartDate.Date >= searchQuery.StartDate.Date) ||
                                                 (c.StartDate.Date < searchQuery.StartDate.Date.AddDays(1) &&
                                                  (c.EndDate == null ||
                                                   c.EndDate.Value.Date >= searchQuery.StartDate.Date))).ToList();
                    }
                    else
                    {
                        if (searchQuery.StartDate == default(DateTime) && searchQuery.EndDate != default(DateTime))
                        {
                            items = items
                                .Where(c => (c.EndDate != null &&
                                             c.EndDate.Value.Date < searchQuery.EndDate.Date.AddDays(1)) ||
                                            (c.StartDate.Date < searchQuery.EndDate.Date.AddDays(1) &&
                                             (c.EndDate == null || c.EndDate.Value.Date >= searchQuery.EndDate.Date)))
                                .ToList();
                        }
                    }
                }
                if (items != null && items.Any())
                {
                    IComparer<object> propertyComparer = null;
                    if (searchQuery.OrderBy == ClashExceptionOrder.FromType || searchQuery.OrderBy == ClashExceptionOrder.ToType)
                    {
                        propertyComparer = new EnumNameComparer();
                    }

                    var sortedItems = items.Sort(searchQuery.OrderBy.ToString(),
                        searchQuery.OrderByDirection.ToString(), propertyComparer);
                    if (searchQuery.Skip != null)
                    {
                        sortedItems = sortedItems.Skip(searchQuery.Skip.Value).ToList();
                    }
                    if (searchQuery.Top != null)
                    {
                        sortedItems = sortedItems.Take(searchQuery.Top.Value).ToList();
                    }
                    return new PagedQueryResult<ClashException>(items.Count, sortedItems?.ToList());
                }

                return null;
            }
        }

        public PagedQueryResult<ClashExceptionModel> SearchWithDescriptions(ClashExceptionSearchQueryModel searchQuery)
        {
            lock (_session)
            {
                var res = Search(searchQuery);

                return res is null
                    ? null
                    : new PagedQueryResult<ClashExceptionModel>(res.TotalCount, GetClashExceptionModels(res.Items));
            }
        }

        public int Count(Expression<Func<ClashException, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<ClashException>().Where(query).Count();
            }
        }

        public void Delete(ClashExceptionType fromType, ClashExceptionType toType, string fromValue, string toValue)
        {
            lock (_session)
            {
                var items = _session.GetAll<ClashException>(x => x.FromType == fromType && x.ToType == toType && x.FromValue == fromValue && x.ToValue == toValue);

                foreach (var item in items)
                {
                    _session.Delete(item);
                }
            }
        }

        public IEnumerable<ClashException> GetActive() => _session.GetAll<ClashException>(e => e.EndDate == null || e.EndDate >= DateTime.Today.Date);

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            lock (_session)
            {
                var items = _session.GetAll<ClashException>(currentItem => currentItem.ExternalRef.In(externalRefs));

                foreach (var item in items)
                {
                    _session.Delete(item);
                }
            }
        }

        /// <summary>
        /// Maps ClashException list to ClashExceptionModel list
        /// </summary>
        /// <param name="clashExceptionsIn"></param>
        private List<ClashExceptionModel> GetClashExceptionModels(ICollection<ClashException> clashExceptionsIn)
        {
            if (clashExceptionsIn is null || clashExceptionsIn.Count == 0)
            {
                return new List<ClashExceptionModel>();
            }

            var clashExceptions = _mapper.Map<List<ClashExceptionModel>>(clashExceptionsIn);

            var clashesByExternalRef = GetClashesIndexedByExternalRef(clashExceptions);
            var productsByExternalRef = GetProductsIndexedByExternalRef(clashExceptions);
            var advertisersByExternalRef = GetAdvertisersIndexedByExternalRef(clashExceptions);

            foreach (var clashException in clashExceptions)
            {
                // Set FromValueDescription
                switch (clashException.FromType)
                {
                    case ClashExceptionType.Advertiser:
                        clashException.FromValueDescription =
                            advertisersByExternalRef.TryGetValue(clashException.FromValue, out var advertiser)
                                ? advertiser
                                : clashException.FromValue;
                        break;

                    case ClashExceptionType.Clash:
                        clashException.FromValueDescription =
                            clashesByExternalRef.TryGetValue(clashException.FromValue, out var clash)
                                ? clash.Description
                                : clashException.FromValue;
                        break;

                    case ClashExceptionType.Product:
                        clashException.FromValueDescription =
                            productsByExternalRef.TryGetValue(clashException.FromValue, out var product)
                                ? product.Name
                                : clashException.FromValue;
                        break;
                }

                // Set ToValueDescription
                switch (clashException.ToType)
                {
                    case ClashExceptionType.Advertiser:
                        clashException.ToValueDescription =
                            advertisersByExternalRef.TryGetValue(clashException.ToValue, out var advertiser)
                                ? advertiser
                                : clashException.ToValue;
                        break;

                    case ClashExceptionType.Clash:
                        clashException.ToValueDescription =
                            clashesByExternalRef.TryGetValue(clashException.ToValue, out var clash)
                                ? clash.Description
                                : clashException.ToValue;
                        break;

                    case ClashExceptionType.Product:
                        clashException.ToValueDescription =
                            productsByExternalRef.TryGetValue(clashException.ToValue, out var product)
                                ? product.Name
                                : clashException.ToValue;
                        break;
                }
            }

            return clashExceptions;
        }

        /// <summary>
        /// Get clashes from clash exception model indexed by clash externalRef
        /// </summary>
        private IImmutableDictionary<string, Clash> GetClashesIndexedByExternalRef(
            IReadOnlyCollection<ClashExceptionModel> clashExceptionModels)
        {
            var clashExternalRefs = new List<string>();

            clashExternalRefs.AddRange(clashExceptionModels
                .Where(ce => ce.FromType == ClashExceptionType.Clash && !string.IsNullOrEmpty(ce.FromValue))
                .Select(ce => ce.FromValue).Distinct());
            clashExternalRefs.AddRange(clashExceptionModels
                .Where(ce => ce.ToType == ClashExceptionType.Clash && !string.IsNullOrEmpty(ce.ToValue))
                .Select(ce => ce.ToValue).Distinct());

            clashExternalRefs = clashExternalRefs.Distinct().ToList();
            var clashes = _session.GetAll<Clash>(currentItem => currentItem.Externalref.In(clashExternalRefs));

            return clashExternalRefs.Any()
                ? Clash.IndexListByExternalRef(clashes)
                : ImmutableDictionary<string, Clash>.Empty;
        }

        /// <summary>
        /// Get products from clash exception models indexed by product ExternalRef
        /// </summary>
        private IImmutableDictionary<string, Product> GetProductsIndexedByExternalRef(
            IReadOnlyCollection<ClashExceptionModel> clashExceptionModels)
        {
            var productExternalRefs = new List<string>();

            productExternalRefs.AddRange(clashExceptionModels
                .Where(ce => ce.FromType == ClashExceptionType.Product && !string.IsNullOrEmpty(ce.FromValue))
                .Select(ce => ce.FromValue).Distinct());
            productExternalRefs.AddRange(clashExceptionModels
                .Where(ce => ce.ToType == ClashExceptionType.Product && !string.IsNullOrEmpty(ce.ToValue))
                .Select(ce => ce.ToValue).Distinct());

            productExternalRefs = productExternalRefs.Distinct().ToList();
            var products = _session.GetAll<Product>(
                currentItem => currentItem.Externalidentifier.In(productExternalRefs),
                indexName: Product_BySearch.DefaultIndexName, isMapReduce: false);

            return productExternalRefs.Any()
                ? products.IndexListByExternalID()
                : ImmutableDictionary<string, Product>.Empty;
        }

        /// <summary>
        /// Get advertisers from clash exception models, indexed by advertiser ExternalRef
        /// </summary>
        private IImmutableDictionary<string, string> GetAdvertisersIndexedByExternalRef(
            IReadOnlyCollection<ClashExceptionModel> clashExceptionModels)
        {
            var advertiserExternalRefs = new List<string>();
            advertiserExternalRefs.AddRange(clashExceptionModels
                .Where(ce =>
                    ce.FromType == ClashExceptionType.Advertiser && !string.IsNullOrEmpty(ce.FromValue))
                .Select(ce => ce.FromValue).Distinct());
            advertiserExternalRefs.AddRange(clashExceptionModels
                .Where(ce => ce.ToType == ClashExceptionType.Advertiser && !string.IsNullOrEmpty(ce.ToValue))
                .Select(ce => ce.ToValue).Distinct());

            if (advertiserExternalRefs.Count == 0)
            {
                return ImmutableDictionary<string, string>.Empty;
            }

            var manyAdvertisersByExternalRef = new Dictionary<string, string>();
            var products =
                _session.GetAll<Product>(currentItem => currentItem.AdvertiserIdentifier.In(advertiserExternalRefs));

            foreach (var product in products)
            {
                if (!manyAdvertisersByExternalRef.ContainsKey(product.AdvertiserIdentifier))
                {
                    manyAdvertisersByExternalRef.Add(product.AdvertiserIdentifier,
                        string.IsNullOrEmpty(product.AdvertiserName)
                            ? product.AdvertiserIdentifier
                            : product.AdvertiserName);
                }
            }

            return manyAdvertisersByExternalRef.ToImmutableDictionary();
        }
    }
}
