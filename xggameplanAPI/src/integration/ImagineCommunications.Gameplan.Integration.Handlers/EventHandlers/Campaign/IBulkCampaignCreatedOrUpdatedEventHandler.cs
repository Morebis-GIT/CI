using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign
{
    public interface IBulkCampaignCreatedOrUpdatedEventHandler : IEventHandler<IBulkCampaignCreatedOrUpdated>
    {
        void HandleWithoutLibraryScenario(IBulkCampaignCreatedOrUpdated command);

        void HandleLibraryScenario();
    }
}
