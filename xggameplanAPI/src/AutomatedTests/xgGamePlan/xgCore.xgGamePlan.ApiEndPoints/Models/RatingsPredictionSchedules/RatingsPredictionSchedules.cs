using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.RatingsPredictionSchedules
{
    public class RatingsPredictionSchedule
    {
        public string SalesArea { get; set; }
        public DateTime ScheduleDay { get; set; }
        public IEnumerable<Rating> Ratings { get; set; }
    }
}
