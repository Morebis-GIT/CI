using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts
{
    public class StandardDayPartGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Guid SalesAreaId { get; set; }
        public string Demographic { get; set; }
        public bool Optimizer { get; set; }
        public bool Policy { get; set; }
        public bool RatingReplacement { get; set; }
        public List<StandardDayPartSplit> Splits { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
