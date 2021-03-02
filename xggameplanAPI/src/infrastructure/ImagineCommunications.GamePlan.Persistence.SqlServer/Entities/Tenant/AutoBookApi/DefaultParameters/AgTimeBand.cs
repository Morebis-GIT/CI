using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    public class AgTimeBand : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int AgCampaignProgrammeId { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Days { get; set; }
    }
}
