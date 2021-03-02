using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.Users;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IUsersApi
    {
        [Get("/users")]
        Task<List<UserModel>> GetAll();

        [Get("/users/{id}")]
        Task<UserModel> GetById([Query] int id);

        [Post("/users")]
        Task<UserModel> Create(CreateUserModel command);

        [Put("/users/{id}")]
        Task<UserModel> Update([Query] int id, CreateUserModel command);

        [Put("/users/{id}/password")]
        Task UpdatePassword(int id, UpdatePasswordModel command);

        [Get("/users/{id}/preview")]
        Task<UserModel> GetPreview(int id);

        [Post("/users/{id}/preview")]
        Task CreatePreview(int id);

        [Delete("/users/{id}/preview")]
        Task DeletePreview(int id);
    }
}
