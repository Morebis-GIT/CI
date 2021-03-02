using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi
{
    public class AutoBookInstanceConfigurationCriteria : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int AutoBookInstanceConfigurationId { get; set; }

        public int? MaxDays { get; set; }

        public int? MaxSalesAreas { get; set; }

        public int? MaxDemographics { get; set; }

        public int? MaxCampaigns { get; set; }

        public int? MaxBreaks { get; set; }
    }
}
