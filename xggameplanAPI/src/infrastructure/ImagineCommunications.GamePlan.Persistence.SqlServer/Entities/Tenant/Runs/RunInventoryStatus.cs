using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunInventoryStatus : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid RunId { get; set; }
        public string InventoryCode { get; set; }
        public Run Run { get; set; }
    }
}
