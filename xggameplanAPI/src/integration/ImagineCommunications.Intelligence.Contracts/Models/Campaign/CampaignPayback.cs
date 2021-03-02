namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignPayback
    {
        public string Name { get; }
        public double Amount { get; }

        public CampaignPayback(string name, double amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
