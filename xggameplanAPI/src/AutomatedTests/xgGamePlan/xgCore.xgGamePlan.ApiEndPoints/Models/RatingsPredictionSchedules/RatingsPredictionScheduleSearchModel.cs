using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.RatingsPredictionSchedules
{
    public class RatingsPredictionScheduleSearchModel
    {
        public DateTime FromScheduleDate { get; set; }
        public DateTime ToScheduleDate { get; set; }
        public string SalesArea { get; set; }
    }
}
