using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Person : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ExternalIdentifier { get; set; }
    }
}
