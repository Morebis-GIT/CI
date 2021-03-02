using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions
{
    public class RestrictionSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int RestrictionId { get; set; }
        public Guid SalesAreaId { get; set; }

        public SalesArea SalesArea { get; set; }
    }
}
