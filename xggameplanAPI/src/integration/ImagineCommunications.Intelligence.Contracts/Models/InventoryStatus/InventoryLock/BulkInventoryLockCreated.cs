using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryLock
{
    public class BulkInventoryLockCreated : IBulkInventoryLockCreated
    {
        public IEnumerable<IInventoryLockCreated> Data { get; }

        public BulkInventoryLockCreated(IEnumerable<InventoryLockCreated> data) => Data = data;
    }
}
