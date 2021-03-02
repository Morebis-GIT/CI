using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities
{
    public class SmoothPassIterationRecordPassSequenceItem : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothPassIterationRecordId { get; set; }
        public int Value { get; set; }
    }
}
