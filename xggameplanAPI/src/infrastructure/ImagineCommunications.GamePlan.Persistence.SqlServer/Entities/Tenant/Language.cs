using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Language : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Alpha3b { get; set; }
        public string Alpha2 { get; set; }
    }
}
