using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ProgrammeClassification : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
