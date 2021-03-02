using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunSalesAreaPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid RunId { get; set; }
        public Guid SalesAreaId { get; set; }
        public SalesAreaPriorityType Priority { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
