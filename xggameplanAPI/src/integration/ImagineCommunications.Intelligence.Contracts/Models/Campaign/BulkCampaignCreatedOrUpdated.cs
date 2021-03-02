using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class BulkCampaignCreatedOrUpdated : IBulkCampaignCreatedOrUpdated
    {
        public IEnumerable<ICampaignCreatedOrUpdated> Data { get; }

        public BulkCampaignCreatedOrUpdated(IEnumerable<CampaignCreatedOrUpdated> data) => Data = data;
    }
}
