using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ProgrammeDictionary : IIdentityPrimaryKey
    {
        public const string SearchField = "TokenizedName";

        public int Id { get; set; }
        public string ExternalReference { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Classification { get; set; }

        public ICollection<ProgrammeEpisode> ProgrammeEpisodes { get; set; } = new HashSet<ProgrammeEpisode>();
    }
}
