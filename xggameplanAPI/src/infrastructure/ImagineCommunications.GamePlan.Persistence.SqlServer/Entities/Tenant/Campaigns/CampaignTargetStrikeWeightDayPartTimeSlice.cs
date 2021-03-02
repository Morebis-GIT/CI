using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTargetStrikeWeightDayPartTimeSlice : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignTargetStrikeWeightDayPartId { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public SortedSet<DayOfWeek> DowPattern { get; set; } = new SortedSet<DayOfWeek>();
    }
}
