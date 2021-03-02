using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup
{
    public interface IDeliveryCappingGroupRepository
    {
        void Add(DeliveryCappingGroup deliveryCappingGroup);

        void Delete(int id);

        IEnumerable<DeliveryCappingGroup> GetAll();

        DeliveryCappingGroup Get(int id);

        DeliveryCappingGroup GetByDescription(string description);

        IEnumerable<DeliveryCappingGroup> Get(IEnumerable<int> ids);

        void SaveChanges();

        void Update(DeliveryCappingGroup deliveryCappingGroup);
    }
}
