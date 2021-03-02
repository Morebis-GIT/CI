using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassRatingPoint : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }

        public ICollection<string> SalesAreas { get; set; }
        public double? OffPeakValue { get; set; }
        public double? PeakValue { get; set; }
        public double? MidnightToDawnValue { get; set; }
    }
}
