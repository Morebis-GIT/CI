using System;
using System.Collections.Generic;
using NodaTime;
using xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns;
using xgCore.xgGamePlan.ApiEndPoints.Models.Clashes;
using xgCore.xgGamePlan.ApiEndPoints.Models.Products;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Spots
{
    public class SpotModel
    {
        public Guid Uid { get; set; }
        public string ExternalCampaignNumber { get; set; }
        public string SalesArea { get; set; }
        public string GroupCode { get; set; }
        public string ExternalSpotRef { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Duration SpotLength { get; set; }
        public string BreakType { get; set; }
        public string ProductCode { get; set; }
        public string Demographic { get; set; }
        public bool ClientPicked { get; set; }
        public string MultipartSpot { get; set; }
        public string MultipartSpotPosition { get; set; }
        public string MultipartSpotRef { get; set; }
        public string RequestedPositioninBreak { get; set; }
        public string ActualPositioninBreak { get; set; }
        public string BreakRequest { get; set; }
        public string ExternalBreakNo { get; set; }
        public bool Sponsored { get; set; }
        public bool Preemptable { get; set; }
        public int Preemptlevel { get; set; }
        public Campaign Campaign { get; set; }
        public Product Product { get; set; }
        public IEnumerable<Clash> Clashes { get; set; }
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }
    }
}
