using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryType
{
    public class BulkInventoryTypeCreated : IBulkInventoryTypeCreated
    {
        public IEnumerable<IInventoryTypeCreated> Data { get; }

        public BulkInventoryTypeCreated(IEnumerable<InventoryTypeCreated> data) => Data = data;
    }
}
