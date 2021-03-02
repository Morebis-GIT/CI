using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class RuleType : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool AllowedForAutopilot { get; set; }
        public bool IsCustom { get; set; }
    }
}
