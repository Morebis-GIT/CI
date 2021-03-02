using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings
{
    public class TenantSettings : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }

        public Guid DefaultScenarioId { get; set; }

        public Guid DefaultSalesAreaPassPriorityId { get; set; }

        public double AutoBookTargetedZeroRatedBreaks { get; set; }

        public TimeSpan PeakStartTime { get; set; }

        public TimeSpan PeakEndTime { get; set; }

        public TimeSpan MidnightStartTime { get; set; }

        public TimeSpan MidnightEndTime { get; set; }

        public int StartDayOfWeek { get; set; }

        public string SystemLogicalDate { get; set; }

        public bool Debug { get; set; }

        public int NoOfRatingsPerSalesDayDemo { get; set; }

        public double? OpenAirtimeFactor { get; set; }

        public RunRestrictions RunRestrictions { get; set; }

        public List<RunEventSettings> RunEventSettings { get; set; } = new List<RunEventSettings>();

        public List<WebhookSettings> WebhookSettings { get; set; } = new List<WebhookSettings>();

        public List<Feature> Features { get; set; } = new List<Feature>();
    }
}
