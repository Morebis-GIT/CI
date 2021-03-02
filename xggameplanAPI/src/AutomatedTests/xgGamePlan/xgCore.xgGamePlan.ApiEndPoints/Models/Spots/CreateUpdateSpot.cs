using System;
using NodaTime;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Spots
{
    public class CreateUpdateSpot
    {
        public string ExternalCampaignNumber { get; set; }
        public string SalesArea { get; set; }
        public string GroupCode { get; set; }
        public string ExternalSpotRef { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Duration SpotLength { get; set; }
        public string BreakType { get; set; }
        public string Product { get; set; }
        public string Demographic { get; set; }
        public bool ClientPicked { get; set; }
        public string MultipartSpot { get; set; }
        public string MultipartSpotPosition { get; set; }
        public string MultipartSpotRef { get; set; }
        public string RequestedPositionInBreak { get; set; }
        public string ActualPositionInBreak { get; set; }
        public string BreakRequest { get; set; }
        public string ExternalBreakNo { get; set; }
        public bool Sponsored { get; set; }
        public bool Preemptable { get; set; }
        public int PreemptLevel { get; set; }
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }
    }
}
