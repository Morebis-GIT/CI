using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ProgrammeCategoryHierarchy : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalRef { get; set; }
        public string ParentExternalRef { get; set; }
    }
}
