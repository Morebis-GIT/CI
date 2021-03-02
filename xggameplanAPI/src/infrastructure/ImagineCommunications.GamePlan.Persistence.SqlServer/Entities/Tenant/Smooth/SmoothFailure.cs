using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth
{
    public class SmoothFailure: IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public Guid RunId { get; set; }

        public int TypeId { get; set; }

        public string SalesArea { get; set; }

        public string ExternalSpotRef { get; set; }

        public string ExternalBreakRef { get; set; }

        public DateTime BreakDateTime { get; set; }

        public TimeSpan SpotLength { get; set; }

        public string ExternalCampaignRef { get; set; }

        public string CampaignName { get; set; }

        public string CampaignGroup { get; set; }

        public string AdvertiserIdentifier { get; set; }

        public string AdvertiserName { get; set; }

        public string ProductName { get; set; }

        public string ClashCode { get; set; }

        public string ClashDescription { get; set; }

        public string IndustryCode { get; set; }

        public string ClearanceCode { get; set; }

        public DateTime? RestrictionStartDate { get; set; }

        public DateTime? RestrictionEndDate { get; set; }

        public TimeSpan? RestrictionStartTime { get; set; }

        public TimeSpan? RestrictionEndTime { get; set; }

        public SortedSet<DayOfWeek> RestrictionDays { get; set; } = new SortedSet<DayOfWeek>();

        public ICollection<SmoothFailureSmoothFailureMessage> FailureMessagesMap { get; set; } =
            new List<SmoothFailureSmoothFailureMessage>(0);
    }
}
