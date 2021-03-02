namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignBreakRequirement
    {
        public CampaignBreakRequirement(string salesArea, CampaignBreakRequirementItem centreBreakRequirement, CampaignBreakRequirementItem endBreakRequirement)
        {
            SalesArea = salesArea;
            CentreBreakRequirement = centreBreakRequirement;
            EndBreakRequirement = endBreakRequirement;
        }

        public string SalesArea { get; }
        public CampaignBreakRequirementItem CentreBreakRequirement { get; }
        public CampaignBreakRequirementItem EndBreakRequirement { get; }
    }
}
