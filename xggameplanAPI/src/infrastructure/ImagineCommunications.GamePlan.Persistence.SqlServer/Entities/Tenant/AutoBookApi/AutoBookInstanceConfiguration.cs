using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi
{
    public class AutoBookInstanceConfiguration : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public CloudProviders CloudProvider { get; set; }

        public string InstanceType { get; set; }

        public int StorageSizeGb { get; set; }

        public double Cost { get; set; }

        public List<AutoBookInstanceConfigurationCriteria> CriteriaList { get; set; } = new List<AutoBookInstanceConfigurationCriteria>();
    }

    public enum CloudProviders : byte
    {
        AWS = 0,
        Azure = 1
    }
}
