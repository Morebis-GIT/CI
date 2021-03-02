using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules
{
    public class SpotBookingRule : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public List<SpotBookingRuleSalesArea> SalesAreas { get; set; }
        public TimeSpan SpotLength { get; set; }
        public TimeSpan MinBreakLength { get; set; }
        public TimeSpan MaxBreakLength { get; set; }
        public int MaxSpots { get; set; }
        public string BreakType { get; set; }
    }
}
