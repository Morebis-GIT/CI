using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Demographic : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string ExternalRef { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Gameplan { get; set; }
    }
}
