using System;

namespace xggameplan.model.External
{
    public class InventoryStatusModel : ICloneable
    {
        public string InventoryCode { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
