using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign
{
    public class CampaignBookingPositionGroup
    {
        public int GroupId { get; set; }
        public double DiscountSurchargePercentage { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public List<string> SalesAreas { get; set; }
    }
}
