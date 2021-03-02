using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Settings
{
    public class InventoryStatus : ICloneable
    {
        public string InventoryCode { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
