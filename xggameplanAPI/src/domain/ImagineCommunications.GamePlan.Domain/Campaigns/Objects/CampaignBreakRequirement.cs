namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignBreakRequirement
    {
        public string SalesArea { get; set; }
        public CampaignBreakRequirementItem CentreBreakRequirement { get; set; }
        public CampaignBreakRequirementItem EndBreakRequirement { get; set; }
    }
}
