using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ClashDifference : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public Guid ClashId { get; set; }

        public Guid? SalesAreaId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? PeakExposureCount { get; set; }

        public int? OffPeakExposureCount { get; set; }

        public ClashDifferenceTimeAndDow TimeAndDow { get; set; }

        public SalesArea SalesArea { get; set; }
    }
}
