using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects;

namespace ImagineCommunications.GamePlan.Domain.InventoryStatuses.Repositories
{
    public interface IInventoryTypeRepository
    {
        IEnumerable<InventoryType> GetAll();
        IEnumerable<InventoryType> GetSystemInventories();
        InventoryType Get(int id);
        void AddRange(IEnumerable<InventoryType> lockTypes);
        void SaveChanges();
        void Truncate();
    }
}
