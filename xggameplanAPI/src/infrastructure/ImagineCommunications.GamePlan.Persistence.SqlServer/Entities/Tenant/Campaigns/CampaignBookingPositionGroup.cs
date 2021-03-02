using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignBookingPositionGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid CampaignId { get; set; }
        public int GroupId { get; set; }
        public double DiscountSurchargePercentage { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public ICollection<CampaignBookingPositionGroupSalesArea> SalesAreas { get; set; } =
            new HashSet<CampaignBookingPositionGroupSalesArea>();
    }
}
