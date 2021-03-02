using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS
{
    public class KPIPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double WeightingFactor { get; set; }
    }
}
