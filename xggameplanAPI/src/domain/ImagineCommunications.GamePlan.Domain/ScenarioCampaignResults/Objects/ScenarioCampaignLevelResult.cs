using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects
{
    public class ScenarioCampaignLevelResult
    {
        /// <summary>
        /// ScenarioId
        /// </summary>
        public Guid Id { get; set; }

        public List<ScenarioCampaignLevelResultItem> Items = new List<ScenarioCampaignLevelResultItem>();
    }
}
