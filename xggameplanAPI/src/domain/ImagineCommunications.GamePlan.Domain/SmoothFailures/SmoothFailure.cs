using System;
using System.Collections.Generic;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.SmoothFailures
{
    /// <summary>
    /// Smooth failure attempting to place spot
    /// </summary>
    public class SmoothFailure : ICloneable
    {
        /// <summary>
        /// Run id
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Failure type:
        /// <list type="table">
        /// <listheader>
        ///     <term>Value</term>
        ///     <description>Description</description>
        /// </listheader>
        /// <item>
        ///     <term>1</term>
        ///     <description>Unplaced spot failure. Spot is unplaced.</description>
        /// </item>
        /// <item>
        ///     <term>2</term>
        ///     <description>Top/Tail break split. One instance for each linked spot.</description>
        /// </item>
        /// </list>
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// The name of the sales area.
        /// </summary>
        public string SalesArea { get; set; }

        /// <summary>
        /// Spot
        /// </summary>
        public string ExternalSpotRef { get; set; }

        /// <summary>
        /// Break that was considered for adding the spot
        /// </summary>
        public string ExternalBreakRef { get; set; }

        /// <summary>
        /// Breaks date and time
        /// </summary>
        public DateTime BreakDateTime { get; set; }

        /// <summary>
        /// Messages for failure
        /// </summary>
        public List<int> MessageIds = new List<int>();

        /// <summary>
        /// Spot length
        /// </summary>
        public Duration SpotLength { get; set; }

        /// <summary>
        /// External campaign ref
        /// </summary>
        public string ExternalCampaignRef { get; set; }

        /// <summary>
        /// Campaign name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Campaign group
        /// </summary>
        public string CampaignGroup { get; set; }

        /// <summary>
        /// Advertiser (Client) identifier
        /// </summary>
        public string AdvertiserIdentifier { get; set; }

        /// <summary>
        /// Advertiser (Client) name
        /// </summary>
        public string AdvertiserName { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Clash code
        /// </summary>
        public string ClashCode { get; set; }

        /// <summary>
        /// Clash name
        /// </summary>
        public string ClashDescription { get; set; }

        /// <summary>
        /// Industry code if restriction
        /// </summary>
        public string IndustryCode { get; set; }

        /// <summary>
        /// Clearance code if restricted
        /// </summary>
        public string ClearanceCode { get; set; }

        /// <summary>
        /// Restriction start date if restricted
        /// </summary>
        public DateTime? RestrictionStartDate { get; set; }

        /// <summary>
        /// Restriction end date if restricted
        /// </summary>
        ///
        public DateTime? RestrictionEndDate { get; set; }

        /// <summary>
        /// Restriction start time if restricted
        /// </summary>
        public TimeSpan? RestrictionStartTime { get; set; }

        /// <summary>
        /// Restriction end time if restricted
        /// </summary>
        public TimeSpan? RestrictionEndTime { get; set; }

        /// <summary>
        /// ”1111111” Days of the week that the restriction applies to Monday to Sunday
        /// where 1 means applies and 0 means does not - this will always have 7 digits
        /// </summary>
        public string RestrictionDays { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
