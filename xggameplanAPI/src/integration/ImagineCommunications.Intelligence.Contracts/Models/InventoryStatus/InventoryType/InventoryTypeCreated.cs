using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryType
{
    public class InventoryTypeCreated : IInventoryTypeCreated
    {
        public string InventoryCode { get; }
        public string Description { get; }
        public string System { get; }
        public IEnumerable<int> LockTypes { get; }

        public InventoryTypeCreated(string inventoryCode, string description, string system, IEnumerable<int> lockTypes)
        {
            InventoryCode = inventoryCode;
            Description = description;
            System = system;
            LockTypes = lockTypes;
        }
    }
}
