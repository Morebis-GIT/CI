using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTargetStrikeWeightLength : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignTargetStrikeWeightId { get; set; }
        public int MultipartNumber { get; set; }
        public TimeSpan Length { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
    }
}
