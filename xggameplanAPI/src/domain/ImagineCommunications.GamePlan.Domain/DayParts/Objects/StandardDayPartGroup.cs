using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.DayParts.Objects
{
    public class StandardDayPartGroup
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
