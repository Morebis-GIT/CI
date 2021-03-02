using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Scenarios.Objects
{
    public class CampaignPriorityRounds
    {
        public bool ContainsInclusionRound { get; set; }
        public List<PriorityRound> Rounds { get; set; }
    }
}
