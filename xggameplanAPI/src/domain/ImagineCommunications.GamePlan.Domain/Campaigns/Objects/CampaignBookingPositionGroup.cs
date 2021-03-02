using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignBookingPositionGroup : ICloneable
    {
        public int GroupId { get; set; }
        public double DiscountSurchargePercentage { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public List<string> SalesAreas { get; set; }

        public object Clone()
        {
            var bookingPositionGroup = (CampaignBookingPositionGroup)MemberwiseClone();

            if (SalesAreas != null)
            {
                bookingPositionGroup.SalesAreas = new List<string>();
                SalesAreas.ForEach(sa => bookingPositionGroup.SalesAreas.Add((string)sa.Clone()));
            }

            return bookingPositionGroup;
        }
    }
}
