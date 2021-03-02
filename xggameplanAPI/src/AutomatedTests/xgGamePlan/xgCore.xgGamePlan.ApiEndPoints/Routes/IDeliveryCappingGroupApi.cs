using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using xgCore.xgGamePlan.ApiEndPoints.Models.DeliveryCappingGroup;

namespace xgCore.xgGamePlan.ApiEndPoints.Routes
{
    [Headers("Authorization: Bearer")]
    public interface IDeliveryCappingGroupApi
    {
        [Get("/DeliveryCappingGroups")]
        Task<IEnumerable<DeliveryCappingGroupModel>> GetAll();

        [Get("/DeliveryCappingGroups/{id}")]
        Task<DeliveryCappingGroupModel> GetById(int id);

        [Post("/DeliveryCappingGroups")]
        Task<DeliveryCappingGroupModel> Create(DeliveryCappingGroupModel model);

        [Put("/DeliveryCappingGroups/{id}")]
        Task<DeliveryCappingGroupModel> Update(int id, DeliveryCappingGroupModel model);

        [Delete("/DeliveryCappingGroups/{id}")]
        Task Delete(int id);
    }
}
