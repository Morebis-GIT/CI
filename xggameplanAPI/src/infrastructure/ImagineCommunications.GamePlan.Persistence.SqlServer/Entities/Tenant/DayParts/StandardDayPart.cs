using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts
{
    public class StandardDayPart : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int DayPartId { get; set; }
        public Guid SalesAreaId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<StandardDayPartTimeslice> Timeslices { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
