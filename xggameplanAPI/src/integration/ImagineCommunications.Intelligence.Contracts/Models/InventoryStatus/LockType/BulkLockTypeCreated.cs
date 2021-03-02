using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.LockType
{
    public class BulkLockTypeCreated : IBulkLockTypeCreated
    {
        public IEnumerable<ILockTypeCreated> Data { get; }

        public BulkLockTypeCreated(IEnumerable<LockTypeCreated> data) => Data = data;
    }
}
