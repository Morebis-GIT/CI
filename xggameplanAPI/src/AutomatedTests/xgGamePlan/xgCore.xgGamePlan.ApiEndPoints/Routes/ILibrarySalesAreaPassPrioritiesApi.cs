using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.LibrarySalesAreaPassPriorities;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface ILibrarySalesAreaPassPrioritiesApi
    {
        [Get("/library/SalesAreaPassPriorities")]
        Task<IEnumerable<LibrarySalesAreaPassPriorityModel>> GetAll();

        [Get("/library/SalesAreaPassPriorities/{id}")]
        Task<LibrarySalesAreaPassPriorityModel> GetById(Guid id);

        [Get("/library/SalesAreaPassPriorities/default")]
        Task<LibrarySalesAreaPassPriorityModel> GetDefault();

        [Post("/library/SalesAreaPassPriorities")]
        Task<LibrarySalesAreaPassPriorityModel> Create(CreateLibrarySalesAreaPassPriorityModel model);

        [Put("/library/SalesAreaPassPriorities")]
        Task<LibrarySalesAreaPassPriorityModel> Update([Query] Guid id, UpdateLibrarySalesAreaPassPriorityModel model);

        [Delete("/library/SalesAreaPassPriorities")]
        Task Delete([Query] Guid id);

        [Put("/library/SalesAreaPassPriorities/default")]
        Task SetDefault([Query] Guid id);
    }
}
