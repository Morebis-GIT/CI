using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class AWSInstanceConfiguration : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public string InstanceType { get; set; }

        public int StorageSizeGb { get; set; }

        public double Cost { get; set; }
    }
}
