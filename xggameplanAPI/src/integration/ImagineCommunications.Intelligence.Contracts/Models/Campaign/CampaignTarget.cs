using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignTarget
    {
        public CampaignTarget(List<StrikeWeight> strikeWeights) => StrikeWeights = strikeWeights;

        public List<StrikeWeight> StrikeWeights { get; }
    }
}
