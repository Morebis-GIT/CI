using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassSlottingLimit : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public string Demographs { get; set; }
        public int MinimumEfficiency { get; set; }
        public int MaximumEfficiency { get; set; }
        public int BandingTolerance { get; set; }
    }
}
