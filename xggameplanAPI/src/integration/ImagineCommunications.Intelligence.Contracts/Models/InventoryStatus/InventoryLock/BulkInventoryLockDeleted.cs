using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryLock
{
    public class BulkInventoryLockDeleted : IBulkInventoryLockDeleted
    {
        public IEnumerable<IInventoryLockDeleted> Data { get; }

        public BulkInventoryLockDeleted(IEnumerable<InventoryLockDeleted> data) => Data = data;
    }
}
