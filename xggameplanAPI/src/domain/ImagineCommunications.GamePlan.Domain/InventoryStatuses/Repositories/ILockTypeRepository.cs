using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;

namespace ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories
{
    public interface ILockTypeRepository
    {
        IEnumerable<InventoryLockType> GetAll();
        InventoryLockType Get(int id);
        void AddRange(IEnumerable<InventoryLockType> lockTypes);
        void SaveChanges();
        void Truncate();
    }
}
