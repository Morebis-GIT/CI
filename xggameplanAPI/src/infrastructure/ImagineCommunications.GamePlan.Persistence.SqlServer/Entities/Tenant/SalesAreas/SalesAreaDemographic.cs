using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas
{
    public class SalesAreaDemographic : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public string ExternalRef { get; set; }
        public bool Exclude { get; set; }
        public string SupplierCode { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
