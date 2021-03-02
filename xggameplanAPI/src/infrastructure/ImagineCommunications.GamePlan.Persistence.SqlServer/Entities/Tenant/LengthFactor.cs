using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class LengthFactor : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public TimeSpan Duration { get; set; }
        public double Factor { get; set; }

        public SalesArea SalesArea { get; set; }
    }
}
