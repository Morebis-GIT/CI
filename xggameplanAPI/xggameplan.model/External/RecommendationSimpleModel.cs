using System;
using ImagineCommunications.GamePlan.Infrastructure.Json.Core.Converters;
using Newtonsoft.Json;
using NodaTime;

namespace xggameplan.Model
{
    /// <summary>
    /// Recommendation, simple properties only
    /// </summary>
    public class RecommendationSimpleModel
    {
        /// <summary>
        /// ScenarioId
        /// </summary>
        public Guid ScenarioId { get; set; }

        /// <summary>
        /// Spot reference
        /// </summary>
        public string ExternalSpotRef { get; set; }

        /// <summary>
        /// Demographic
        /// </summary>
        public string Demographic { get; set; }

        /// <summary>
        /// Sales area
        /// </summary>
        public string SalesArea { get; set; }

        /// <summary>
        /// Processor
        /// </summary>
        public string Processor { get; set; }

        /// <summary>
        /// Spot efficiency
        /// </summary>
        public double SpotEfficiency { get; set; }

        /// <summary>Returned as campaign ref in request payload</summary>
        [JsonProperty("campaignRef")]
        public string ExternalCampaignNumber { get; set; }

        /// <summary>Returned as pass number in request payload</summary>
        [JsonProperty("passNumber")]
        public int OptimiserPassSequenceNumber { get; set; }

        /// <summary>Product property is not returned in the request payload</summary>
        [JsonIgnore]
        public string Product { get; set; }

        /// <summary>
        /// Agency name from Product.
        /// Returned as client name in request payload.
        /// </summary>
        [JsonProperty("clientName")]
        public string AgencyName { get; set; }

        /// <summary>Product name from Product model</summary>
        [JsonProperty("product")]
        public string ProductName { get; set; }

        [JsonProperty("startDate")]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime StartDateTime { get; set; }

        [JsonProperty("endDate")]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime EndDateTime { get; set; }

        /// <summary>Returned as duration in request payload</summary>
        [JsonProperty("duration")]
        public Duration SpotLength { get; set; }

        /// <summary>Returned as rating in request payload</summary>
        [JsonProperty("ratings")]
        public double SpotRating { get; set; }
    }
}
