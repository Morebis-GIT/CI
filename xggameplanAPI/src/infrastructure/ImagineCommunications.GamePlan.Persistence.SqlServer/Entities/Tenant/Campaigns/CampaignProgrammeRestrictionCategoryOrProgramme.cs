using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignProgrammeRestrictionCategoryOrProgramme : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignProgrammeRestrictionId { get; set; }
        public string Name { get; set; }
    }
}
