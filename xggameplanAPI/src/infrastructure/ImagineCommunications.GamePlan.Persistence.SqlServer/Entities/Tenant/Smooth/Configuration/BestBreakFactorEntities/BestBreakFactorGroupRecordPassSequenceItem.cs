using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities
{
    public class BestBreakFactorGroupRecordPassSequenceItem : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int BestBreakFactorGroupRecordId { get; set; }
        public int Value { get; set; }
    }
}
