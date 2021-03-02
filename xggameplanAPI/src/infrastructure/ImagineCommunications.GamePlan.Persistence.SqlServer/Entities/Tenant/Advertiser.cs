using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Advertiser : IIdentityPrimaryKey
    {
        public const string SearchFieldName = "TokenizedName";

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ExternalIdentifier { get; set; }
    }
}
