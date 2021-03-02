using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class AutopilotSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int DefaultFlexibilityLevelId { get; set; }
        public int ScenariosToGenerate { get; set; }
    }
}
