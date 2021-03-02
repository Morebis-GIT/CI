using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.Tenants;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ITenantsApi
    {
        [Get("/tenants")]
        Task<List<Tenant>> GetAll();

        [Get("/tenants/{id}")]
        Task<Tenant> GetById(int id);

        [Get("/tenants/{id}")]
        Task<ApiResponse<Tenant>> GetByIdResponse(int id);

        [Post("/tenants")]
        Task<Tenant> Create(TenantCreate tenantCreate);

        [Put("/tenants/{id}")]
        Task<Tenant> Update(TenantCreate tenantCreate, [Query]int id);

    }
}
