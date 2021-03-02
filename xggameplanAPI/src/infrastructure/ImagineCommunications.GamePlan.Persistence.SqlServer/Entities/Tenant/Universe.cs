using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Universe : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public string Demographic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UniverseValue { get; set; }

        public SalesArea SalesArea { get; set; }
    }
}
