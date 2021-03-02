using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassSalesAreaPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassSalesAreaPriorityCollectionId { get; set; }
        public string SalesArea { get; set; }
        public SalesAreaPriorityType Priority { get; set; }
    }
}
