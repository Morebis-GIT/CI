using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects
{
    public class ScenarioCampaignResult
    {
        /// <summary>
        /// ScenarioId
        /// </summary>
        public Guid Id { get; set; }

        public List<ScenarioCampaignResultItem> Items = new List<ScenarioCampaignResultItem>();
    }
}
