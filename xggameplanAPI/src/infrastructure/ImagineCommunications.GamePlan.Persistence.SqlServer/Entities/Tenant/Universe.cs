using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Universe : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }
        public string SalesArea { get; set; }
        public string Demographic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UniverseValue { get; set; }
    }
}
