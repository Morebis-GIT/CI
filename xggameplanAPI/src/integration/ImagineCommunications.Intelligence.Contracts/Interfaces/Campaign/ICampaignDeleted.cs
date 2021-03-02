using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign
{
    public interface ICampaignDeleted : IEvent
    {
        string ExternalId { get; }
    }
}
