using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts
{
    public class StandardDayPartGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string SalesArea { get; set; }
        public string Demographic { get; set; }
        public bool Optimizer { get; set; }
        public bool Policy { get; set; }
        public bool RatingReplacement { get; set; }
        public List<StandardDayPartSplit> Splits { get; set; }
    }
}
