using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities
{
    public abstract class SmoothPass : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothConfigurationId { get; set; }
        public int Sequence { get; set; }
    }
}
