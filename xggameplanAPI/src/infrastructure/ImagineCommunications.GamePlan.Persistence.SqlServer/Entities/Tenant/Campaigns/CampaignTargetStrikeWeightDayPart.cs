using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTargetStrikeWeightDayPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string DayPartName { get; set; } = "NotSupplied";
        public int CampaignTargetStrikeWeightId { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public int SpotMaxRatings { get; set; }
        public decimal CampaignPrice { get; set; }
        public int TotalSpotCount { get; set; }
        public int ZeroRatedSpotCount { get; set; }
        public double Ratings { get; set; }
        public double BaseDemographRatings { get; set; }
        public double NominalValue { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }

        public ICollection<CampaignTargetStrikeWeightDayPartLength> Lengths { get; set; } =
            new HashSet<CampaignTargetStrikeWeightDayPartLength>();
        public ICollection<CampaignTargetStrikeWeightDayPartTimeSlice> Timeslices { get; set; } =
            new HashSet<CampaignTargetStrikeWeightDayPartTimeSlice>();
    }
}
