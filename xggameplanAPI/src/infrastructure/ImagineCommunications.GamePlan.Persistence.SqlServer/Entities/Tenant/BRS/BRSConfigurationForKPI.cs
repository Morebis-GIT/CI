using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BRS
{
    public class BRSConfigurationForKPI : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int BRSConfigurationTemplateId { get; set; }
        public string KPIName { get; set; }
        public int PriorityId { get; set; }
    }
}
