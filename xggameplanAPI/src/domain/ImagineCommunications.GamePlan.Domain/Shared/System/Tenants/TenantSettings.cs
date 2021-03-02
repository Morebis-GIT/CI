using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Tenants
{
    /// <summary>
    /// Tenant settings
    /// </summary>
    public class TenantSettings
    {
        /// <summary>
        /// Default ABTZRB: Autobook Targeted Zero Rated Breaks Booking (Nine)
        /// </summary>
        public const double DefaultAutoBookTargetedZeroRatedBreaks = 50;

        public Guid Id { get; set; }

        public Guid DefaultScenarioId { get; set; }
        public Guid DefaultSalesAreaPassPriorityId { get; set; }
        public double AutoBookTargetedZeroRatedBreaks { get; set; } = DefaultAutoBookTargetedZeroRatedBreaks;

        public string PeakStartTime { get; set; }
        public string PeakEndTime { get; set; }

        public string MidnightStartTime { get; set; }
        public string MidnightEndTime { get; set; }

        public DayOfWeek StartDayOfWeek { get; set; } = DayOfWeek.Monday;

        ///<summary>
        /// System logical date format is "MMddyyyy".
        /// </summary>
        public string SystemLogicalDate { get; set; }

        ///<summary>
        /// Defaults to four 15 min slots per hour per day.
        /// </summary>
        public int NoOfRatingsPerSalesDayDemo { get; set; } = 4 * 24;

        public double? OpenAirtimeFactor { get; set; }

        public bool Debug { get; set; }
        public RunRestrictions RunRestrictions { get; set; }

        /// <summary>
        /// List of settings for run events
        /// </summary>
        public List<RunEventSettings> RunEventSettings = new List<RunEventSettings>();

        public List<WebhookSettings> WebhookSettings = new List<WebhookSettings>();

        /// <summary>
        /// List of Features which can be enabled or disabled for this tenant
        /// </summary>
        public List<Feature> Features = new List<Feature>();
    }
}
