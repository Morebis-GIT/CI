using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas
{
    public class MetadataValue : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ValueId { get; set; }
        public string Value { get; set; }
    }
}
