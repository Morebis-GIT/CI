using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignDeleted : ICampaignDeleted
    {
        public CampaignDeleted(string externalId) => ExternalId = externalId;
        public string ExternalId { get; }
    }
}
