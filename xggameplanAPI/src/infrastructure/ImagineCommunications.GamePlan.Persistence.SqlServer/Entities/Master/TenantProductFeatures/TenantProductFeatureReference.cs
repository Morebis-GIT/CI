using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures
{
    public class TenantProductFeatureReference : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int TenantProductFeatureChildId { get; set; }
        public int TenantProductFeatureParentId { get; set; }
    }
}
