using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class SpotPlacement : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string ExternalSpotRef { get; set; }
        public string ExternalBreakRef { get; set; }
        public string ResetExternalBreakRef { get; set; }
    }
}
