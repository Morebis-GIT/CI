using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Models
{
    public class SalesAreaPriority : ICloneable
    {
        public string SalesArea { get; set; }

        public SalesAreaPriorityType Priority { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
