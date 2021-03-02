namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignBreakRequirementItem
    {
        public CampaignBreakRequirementItem(double currentPercentageSplit, double desiredPercentageSplit)
        {
            CurrentPercentageSplit = currentPercentageSplit;
            DesiredPercentageSplit = desiredPercentageSplit;
        }

        public double CurrentPercentageSplit { get; }
        public double DesiredPercentageSplit { get; }
    }
}
