using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS
{
    public class BRSConfigurationTemplate : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDefault { get; set; }
        public List<BRSConfigurationForKPI> KPIConfigurations { get; set; } = new List<BRSConfigurationForKPI>();
    }
}
