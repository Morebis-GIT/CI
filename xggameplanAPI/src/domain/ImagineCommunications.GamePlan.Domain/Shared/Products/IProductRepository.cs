using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;

namespace ImagineCommunications.GamePlan.Domain.Shared.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        void Delete(Guid uid);
        Product Get(Guid uid);
        Product Get(Guid uid, DateTime onDate);
        IEnumerable<Product> GetAll(DateTime onDate);
        PagedQueryResult<ProductNameModel> Search(ProductSearchQueryModel searchQuery, DateTime onDate);
        IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds, DateTime onDate);
        IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds);
        PagedQueryResult<ProductAdvertiserModel> Search(AdvertiserSearchQueryModel searchQuery, DateTime onDate);
        void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs);
        IEnumerable<ProductAdvertiserModel> GetAdvertisers(ICollection<string> advertiserIds);
        IEnumerable<ProductAgencyModel> GetAgencies(ICollection<string> agencyIds);
        IEnumerable<AgencyGroup> GetAgencyGroups(ICollection<string> agencyGroupIds);
        IEnumerable<ProductNameModel> GetByExternalIds(ICollection<string> externalIds);
        IEnumerable<SalesExecutiveModel> GetSalesExecutives(ICollection<int> salesExecIds);
        IEnumerable<string> GetReportingCategories();

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="product"></param>
        void Update(Product product);
        Task TruncateAsync();
        void SaveChanges();
        bool Exists(Expression<Func<Product, bool>> condition);
        IEnumerable<Product> FindByExternal(string externalRef, DateTime onDate);
        IEnumerable<Product> FindByExternal(List<string> externalRefs, DateTime onDate);
    }
}
