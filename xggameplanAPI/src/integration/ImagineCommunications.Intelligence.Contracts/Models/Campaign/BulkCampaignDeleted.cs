using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class BulkCampaignDeleted : IBulkCampaignDeleted
    {
        public IEnumerable<ICampaignDeleted> Data { get; }

        public BulkCampaignDeleted(IEnumerable<CampaignDeleted> data) => Data = data;
    }
}
