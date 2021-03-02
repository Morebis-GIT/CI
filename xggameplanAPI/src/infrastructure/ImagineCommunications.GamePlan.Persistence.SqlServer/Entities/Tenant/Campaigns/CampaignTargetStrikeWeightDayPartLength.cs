using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTargetStrikeWeightDayPartLength : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignTargetStrikeWeightDayPartId { get; set; }
        public TimeSpan Length { get; set; }
        public int MultipartNumber { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

    }
}
