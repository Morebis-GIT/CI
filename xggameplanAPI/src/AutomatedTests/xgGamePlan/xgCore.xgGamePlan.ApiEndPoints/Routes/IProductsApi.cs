using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Products;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IProductsApi
    {
        [Get("/Product")]
        Task<IEnumerable<Product>> GetAll();

        [Get("/Product/externalRef/{externalId}")]
        Task<Product> GetByExternalRef(string externalId);

        [Get("/Product/Search")]
        Task<SearchResultModel<Product>> Search([Query] ProductSearchQueryModel model);

        [Get("/Product/Advertiser/Search")]
        Task<SearchResultModel<Product>> SearchByAdvertiser([Query] AdvertiserSearchQueryModel model);

        [Post("/Product")]
        Task<ApiErrorResult> Create(IEnumerable<Product> products);

        [Put("/Product/externalRef/{externalId}")]
        Task<Product> Put(string externalId, Product product);

        [Delete("/Product/DeleteAll")]
        Task<ApiErrorResult> DeleteAll();
    }
}
