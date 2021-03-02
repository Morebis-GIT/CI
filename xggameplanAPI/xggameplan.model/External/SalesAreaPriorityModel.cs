using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;

namespace xggameplan.Model
{
    public class SalesAreaPriorityModel : ICloneable
    {
        public string SalesArea { get; set; }
        public SalesAreaPriorityType Priority { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
