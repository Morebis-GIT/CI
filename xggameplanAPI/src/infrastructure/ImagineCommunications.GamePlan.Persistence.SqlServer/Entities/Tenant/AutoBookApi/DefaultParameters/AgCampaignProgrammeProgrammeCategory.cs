using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgCampaignProgrammeProgrammeCategory : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignProgrammeId { get; set; }

        public int ProgrammeNumber { get; set; }
        public int CategoryNumber { get; set; }
    }
}
