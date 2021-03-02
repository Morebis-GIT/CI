using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts
{
    public class StandardDayPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int DayPartId { get; set; }
        public string SalesArea { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<StandardDayPartTimeslice> Timeslices { get; set; }
    }
}
