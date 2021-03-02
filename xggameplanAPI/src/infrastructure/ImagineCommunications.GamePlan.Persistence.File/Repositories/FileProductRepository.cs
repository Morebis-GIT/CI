using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileProductRepository : FileRepositoryBase, IProductRepository
    {
        public FileProductRepository(string folder) : base(folder, "product")
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Product> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Uid.ToString()).ToList());
        }

        public void Add(Product item)
        {
            Add(new List<Product>() { item });
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
            UpdateOrInsertItem(_folder, _type, item, item.Uid.ToString());
        }

        public IEnumerable<Product> FindByExternal(string productref)
        {
            return GetAllByType<Product>(_folder, _type, currentItem => currentItem.Externalidentifier == productref);
        }

        public IEnumerable<Product> FindByExternal(List<string> productRefs) =>
            FindByExternal(productRefs, NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

        [Obsolete("Use Get()")]
        public Product Find(Guid id) => Get(id);

        public Product Get(Guid id) => Get(id, NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

        public Product Get(Guid uid, DateTime onDate) => GetItemByID<Product>(_folder, _type, uid.ToString());

        public IEnumerable<Product> GetAll(DateTime onDate) => GetAllByType<Product>(_folder, _type);

        public IEnumerable<Product> GetAll() => GetAll(NodaTime.SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

        public int CountAll => CountAll(_folder, _type);

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            DeleteItem(_folder, _type, uid.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds) =>
            throw new NotImplementedException();

        public PagedQueryResult<ProductAdvertiserModel> Search(AdvertiserSearchQueryModel searchQuery, DateTime onDate) =>
            throw new NotImplementedException();

        public PagedQueryResult<ProductNameModel> Search(ProductSearchQueryModel searchQuery, DateTime onDate) =>
            throw new NotImplementedException();

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds, DateTime onDate)
        {
            return GetAllByType<Product>(_folder, _type, p => advertiserIds.Contains(p.AdvertiserIdentifier));
        }

        public int Count(Expression<Func<Product, bool>> query)
        {
            return GetAllByType(_folder, _type, query).Count;
        }

        public void SaveChanges()
        {
        }

        public bool Exists(Expression<Func<Product, bool>> condition) => throw new NotImplementedException();

        public IEnumerable<Product> FindByExternal(string externalRef, DateTime onDate) =>
            GetAllByType<Product>(_folder, _type, currentItem => externalRef.Contains(currentItem.Externalidentifier));

        public IEnumerable<Product> FindByExternal(List<string> externalRefs, DateTime onDate) =>
            throw new NotImplementedException();
    }
}
