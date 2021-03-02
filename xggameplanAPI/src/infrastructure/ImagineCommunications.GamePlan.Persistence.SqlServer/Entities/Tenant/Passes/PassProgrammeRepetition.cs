using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassProgrammeRepetition : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public int Minutes { get; set; }
        public double Factor { get; set; }
        public double? PeakFactor { get; set; }
    }
}
