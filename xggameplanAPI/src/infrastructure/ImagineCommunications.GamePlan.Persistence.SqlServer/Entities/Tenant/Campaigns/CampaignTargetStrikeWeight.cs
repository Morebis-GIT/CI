using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTargetStrikeWeight : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignTargetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public int SpotMaxRatings { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }

        public ICollection<CampaignTargetStrikeWeightLength> Lengths { get; set; } =
            new HashSet<CampaignTargetStrikeWeightLength>();
        public ICollection<CampaignTargetStrikeWeightDayPart> DayParts { get; set; } =
            new HashSet<CampaignTargetStrikeWeightDayPart>();
    }
}
