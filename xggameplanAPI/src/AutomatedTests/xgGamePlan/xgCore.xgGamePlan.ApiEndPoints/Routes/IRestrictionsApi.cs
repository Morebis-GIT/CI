using System;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiErrorResults;
using xgCore.xgGamePlan.ApiEndPoints.Models.Restrictions;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IRestrictionsApi
    {
        [Get("/Restrictions")]
        Task<GetRestrictionsResult> GetAll();

        [Post("/Restrictions")]
        Task<Restriction> Create(Restriction restriction);

        [Delete("/Restrictions")]
        Task<ApiErrorResult> DeleteAll();

        [Get("/Restrictions/{id}")]
        Task<Restriction> GetById(Guid id);

        [Get("/Restrictions/{id}")]
        Task<ApiResponse<Restriction>> GetByIdResponse(Guid id);

        [Put("/Restrictions/{externalIdentifier}")]
        Task<Restriction> Put(string externalIdentifier, Restriction restriction);

        [Put("/Restrictions/internal/{id}")]
        Task<Restriction> Put(Guid id, Restriction restriction);

        [Delete("/Restrictions/{id}")]
        Task<ApiErrorResult> DeleteById(Guid id);
    }
}
