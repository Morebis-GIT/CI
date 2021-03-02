using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Spot : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }        
        public string ExternalCampaignNumber { get; set; }
        public string SalesArea { get; set; }
        public string GroupCode { get; set; }
        public string ExternalSpotRef { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan SpotLength { get; set; }
        public string BreakType { get; set; }
        public string Product { get; set; }
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
        public int BookingPosition { get; set; }
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }
        public decimal NominalPrice { get; set; }
    }
}
