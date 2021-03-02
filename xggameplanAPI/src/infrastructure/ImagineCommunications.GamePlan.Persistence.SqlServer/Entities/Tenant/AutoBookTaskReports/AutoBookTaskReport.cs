using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookTaskReports
{
    internal class AutoBookTaskReport : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public DateTime TimeCreated { get; set; }

        public Guid RunId { get; set; }

        public Guid ScenarioId { get; set; }

        public string Url { get; set; }

        public string Region { get; set; }

        public string BinariesVersion { get; set; }

        public string Version { get; set; }

        public string FullName { get; set; }

        public string InstanceType { get; set; }

        public string StorageSizeGB { get; set; }
    }
}
