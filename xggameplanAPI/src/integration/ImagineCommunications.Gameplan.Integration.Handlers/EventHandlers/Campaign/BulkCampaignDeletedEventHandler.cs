using System;
using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using xggameplan.core.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign
{
    public class BulkCampaignDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkCampaignDeleted>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ILoggerService _logger;
        private readonly ICampaignCleaner _campaignCleaner;

        public BulkCampaignDeletedEventHandler(
            ICampaignRepository campaignRepository,
            ILoggerService logger,
            ICampaignCleaner campaignCleaner)
        {
            _campaignRepository = campaignRepository;
            _logger = logger;
            _campaignCleaner = campaignCleaner;
        }

        public override void Handle(IBulkCampaignDeleted command)
        {
            var allActiveExternalIds = _campaignRepository.GetAllActiveExternalIds();
            var externalIdsToDelete = command.Data.Select(x => x.ExternalId).ToList();
            var inactiveExternalIds = externalIdsToDelete
                .Except(allActiveExternalIds, StringComparer.OrdinalIgnoreCase).ToList();

            if (inactiveExternalIds.Count > 0)
            {
                _logger.Warn($"Active campaigns not found: {String.Join(", ", inactiveExternalIds)}");
            }
            else
            {
                Task.Run(async () => await _campaignCleaner.ExecuteAsync(externalIdsToDelete).ConfigureAwait(false))
                    .GetAwaiter().GetResult();
            }
        }
    }
}
