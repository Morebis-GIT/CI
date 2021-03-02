using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas
{
    public class MetadataCategory : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<MetadataValue> MetadataValues { get; set; } = new List<MetadataValue>();
    }
}
