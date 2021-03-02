using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;

namespace ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories
{
    public interface IInventoryLockRepository
    {
        IEnumerable<InventoryLock> GetAll();
        InventoryLock Get(int id);
        void AddRange(IEnumerable<InventoryLock> lockTypes);
        void SaveChanges();
        void Truncate();
        void DeleteRange(IEnumerable<string> salesAreas);
    }
}
