using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.DayParts.Objects
{
    public class StandardDayPart
    {
        public int Id { get; set; }
        public int DayPartId { get; set; }
        public string SalesArea { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<StandardDayPartTimeslice> Timeslices { get; set; }
    }
}
