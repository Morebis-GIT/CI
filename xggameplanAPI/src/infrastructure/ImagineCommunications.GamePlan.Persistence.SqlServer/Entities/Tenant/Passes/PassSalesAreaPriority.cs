using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassSalesAreaPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassSalesAreaPriorityCollectionId { get; set; }
        public Guid SalesAreaId { get; set; }
        public SalesAreaPriorityType Priority { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
