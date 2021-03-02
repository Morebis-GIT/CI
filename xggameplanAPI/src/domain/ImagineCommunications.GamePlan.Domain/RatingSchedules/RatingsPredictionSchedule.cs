using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace ImagineCommunications.GamePlan.Domain.RatingSchedules
{
    public class RatingsPredictionSchedule
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public DateTime ScheduleDay { get; set; }

        public List<Rating> Ratings { get; set; }

        public static int CountRatings(IEnumerable<RatingsPredictionSchedule> schedules)
        {
            return schedules?.Sum(x => x.Ratings?.Count ?? 0) ?? 0;
        }
    }
}
