using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunSalesAreaPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid RunId { get; set; }
        public string SalesArea { get; set; }
        public SalesAreaPriorityType Priority { get; set; }
    }
}
