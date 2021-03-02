using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes
{
    public class ProgrammeEpisode : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        public int ProgrammeDictionaryId { get; set; }
        public ProgrammeDictionary ProgrammeDictionary { get; set; }
    }
}
