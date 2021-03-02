using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.InventoryStatuses.Objects
{
    public class InventoryType
    {
        public int Id { get; set; }
        public string InventoryCode { get; set; }
        public string Description { get; set; }
        public string System { get; set; }
        public List<int> LockTypes { get; set; } = new List<int>();
    }
}
