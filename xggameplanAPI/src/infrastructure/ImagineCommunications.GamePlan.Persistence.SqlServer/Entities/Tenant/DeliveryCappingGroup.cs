using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class DeliveryCappingGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }
        public bool ApplyToPrice { get; set; }
    }
}
