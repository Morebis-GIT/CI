using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities
{
    public class BestBreakFilterFactor: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int BestBreakFactorGroupItemId { get; set; }

        public int Priority { get; set; }

        public BestBreakFactors Factor { get; set; }
    }
}
