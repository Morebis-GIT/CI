using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships
{
    public class SponsoredDayPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SponsorshipItemId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public SortedSet<DayOfWeek> DaysOfWeek { get; set; }
    }
}
