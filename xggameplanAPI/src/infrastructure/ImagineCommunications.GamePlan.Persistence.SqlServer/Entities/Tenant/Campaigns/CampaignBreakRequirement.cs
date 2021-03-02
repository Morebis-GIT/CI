using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignBreakRequirement : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public string SalesArea { get; set; }
        public CampaignCentreBreakRequirementItem CentreBreakRequirement { get; set; }
        public CampaignEndBreakRequirementItem EndBreakRequirement { get; set; }
    }
}
