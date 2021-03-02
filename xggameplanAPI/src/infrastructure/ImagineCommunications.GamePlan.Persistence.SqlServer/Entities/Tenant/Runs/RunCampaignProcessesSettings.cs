using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunCampaignProcessesSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid RunId { get; set; }
        public string ExternalId { get; set; }
        public bool? InefficientSpotRemoval { get; set; }
        public bool? IncludeRightSizer { get; set; }
        public RightSizerLevel? RightSizerLevel { get; set; }
        public int DeliveryCappingGroupId { get; set; }
    }
}
