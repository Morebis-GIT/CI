using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.BRS
{
    public class BRSConfigurationTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDefault { get; set; }
        public List<BRSConfigurationForKPI> KPIConfigurations { get; set; } = new List<BRSConfigurationForKPI>();
    }
}
