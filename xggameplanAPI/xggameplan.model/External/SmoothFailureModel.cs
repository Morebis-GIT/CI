using System;
using System.Collections.Generic;
using NodaTime;

namespace xggameplan.Model
{
    public class SmoothFailureModel
    {
        public Guid RunId { get; set; }
        public int TypeId { get; set; }
        public string SalesArea { get; set; }
        public string SalesAreaShortName { get; set; }

        public string ExternalSpotRef { get; set; }
        public string ExternalBreakRef { get; set; }
        public DateTime BreakDateTime { get; set; }
        public List<int> MessageIds = new List<int>();

        public Duration SpotLength { get; set; }    // Spot.SpotLength
        public string ExternalCampaignRef { get; set; }
        public string CampaignName { get; set; }    // Campaign.Name
        public string CampaignGroup { get; set; } // Campaign.CampaignGroup
        public string AdvertiserIdentifier { get; set; }
        public string AdvertiserName { get; set; }      // Product.AdvertiserName
        public string ProductName { get; set; }      // Product.Name
        public string ClashCode { get; set; }
        public string ClashDescription { get; set; }       // Clash.Description
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }
        public DateTime? RestrictionStartDate { get; set; }
        public DateTime? RestrictionEndDate { get; set; }
        public TimeSpan? RestrictionStartTime { get; set; }
        public TimeSpan? RestrictionEndTime { get; set; }
        public string RestrictionDays { get; set; }
    }
}
