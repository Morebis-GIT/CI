using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class KPIComparisonConfig : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string KPIName { get; set; }
        public float DiscernibleDifference { get; set; }
        public bool HigherIsBest { get; set; }
        public bool Ranked { get; set; }
    }
}
