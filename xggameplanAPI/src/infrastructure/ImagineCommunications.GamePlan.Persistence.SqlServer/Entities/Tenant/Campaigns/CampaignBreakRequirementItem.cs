using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public abstract class CampaignBreakRequirementItem : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignBreakRequirementId { get; set; }
        public double CurrentPercentageSplit { get; set; }
        public double DesiredPercentageSplit { get; set; }
    }

    public class CampaignCentreBreakRequirementItem : CampaignBreakRequirementItem
    {
    }

    public class CampaignEndBreakRequirementItem : CampaignBreakRequirementItem
    {
    }
}
