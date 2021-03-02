using System;
using Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Domain.Recommendations.Objects
{
    public class RecommendationReducedModel
    {
        [JsonProperty(Order = 1)]
        public Guid ScenarioId { get; set; }
        [JsonProperty(Order = 2)]
        public string ExternalCampaignNumber { get; set; }
        [JsonProperty(Order = 3)]
        public string SalesArea { get; set; }
        [JsonProperty(Order = 4)]
        public string ExternalSpotRef { get; set; }
        [JsonProperty(Order = 5)]
        public string ExternalBreakNo { get; set; }
        [JsonProperty(Order = 6)]
        public DateTime StartDateTime { get; set; }
        [JsonProperty(Order = 7)]
        public string Action { get; set; }
    }
}
