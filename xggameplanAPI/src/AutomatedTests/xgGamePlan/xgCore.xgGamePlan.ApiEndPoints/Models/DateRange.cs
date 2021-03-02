using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models
{
    public class DateRange
    {
        public DateRange() { }
        public DateRange(DateTime start, DateTime end) => (Start, End) = (start, end);
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Contains(DateTime value) => value.Date >= Start.Date && value.Date <= End.Date;
    }
}
