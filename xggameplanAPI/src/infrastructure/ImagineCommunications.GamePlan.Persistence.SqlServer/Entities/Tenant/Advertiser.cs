using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Advertiser : IIdentityPrimaryKey
    {
        public const string SearchFieldName = "TokenizedName";
        public static readonly IReadOnlyList<string> SearchFieldSources = new List<string>() { nameof(Name), nameof(ShortName), nameof(ExternalIdentifier) }.AsReadOnly();
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ExternalIdentifier { get; set; }
    }
}
