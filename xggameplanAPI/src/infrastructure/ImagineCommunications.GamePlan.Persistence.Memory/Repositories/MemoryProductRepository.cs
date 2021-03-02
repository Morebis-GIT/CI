using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryProductRepository :
        MemoryRepositoryBase<Product>,
        IProductRepository
    {
        public MemoryProductRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Product> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Add(Product item)
        {
            InsertOrReplaceItem(item, item.Uid.ToString());
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductAdvertiserModel> GetAdvertisers(ICollection<string> advertiserIds) => throw new NotImplementedException();

        public IEnumerable<ProductAgencyModel> GetAgencies(ICollection<string> agencyIds) => throw new NotImplementedException();

        public IEnumerable<AgencyGroup> GetAgencyGroups(ICollection<string> agencyGroupIds) => throw new NotImplementedException();

        public IEnumerable<ProductNameModel> GetByExternalIds(ICollection<string> externalIds) => throw new NotImplementedException();

        public IEnumerable<SalesExecutiveModel> GetSalesExecutives(ICollection<int> salesExecIds) => throw new NotImplementedException();

        public IEnumerable<string> GetReportingCategories() => throw new NotImplementedException();

        public void Update(Product item)
        {
            InsertOrReplaceItem(item, item.Uid.ToString());
        }

        public IEnumerable<Product> FindByExternal(string productref)
        {
            return GetAllItems(currentItem => currentItem.Externalidentifier == productref);
        }

        public IEnumerable<Product> FindByExternal(List<string> productRefs)
        {
            return GetAllItems(currentItem => productRefs.Contains(currentItem.Externalidentifier));
        }

        [Obsolete("Use Get()")]
        public Product Find(Guid id) => Get(id);

        public Product Get(Guid id) => Get(id, NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

        public Product Get(Guid uid, DateTime onDate) => GetItemById(uid.ToString());

        public IEnumerable<Product> GetAll(DateTime onDate) => GetAllItems();

        public IEnumerable<Product> GetAll() =>
            GetAll(NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

        public int CountAll => GetCount();

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            DeleteItem(uid.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems();
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds) =>
            FindByAdvertiserId(advertiserIds, NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

        public PagedQueryResult<ProductAdvertiserModel> Search(AdvertiserSearchQueryModel searchQuery, DateTime onDate)
        {
            throw new NotImplementedException();
        }

        public PagedQueryResult<ProductNameModel> Search(ProductSearchQueryModel searchQuery, DateTime onDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds, DateTime onDate) =>
            GetAllItems(p => advertiserIds.Contains(p.AdvertiserIdentifier));

        public int Count(Expression<Func<Product, bool>> query) => GetCount(query);

        public void SaveChanges()
        {
        }

        public bool Exists(Expression<Func<Product, bool>> condition) =>
            throw new NotImplementedException();

        public IEnumerable<Product> FindByExternal(string externalRef, DateTime onDate) => throw new NotImplementedException();

        public IEnumerable<Product> FindByExternal(List<string> externalRefs, DateTime onDate) => throw new NotImplementedException();
    }
}
