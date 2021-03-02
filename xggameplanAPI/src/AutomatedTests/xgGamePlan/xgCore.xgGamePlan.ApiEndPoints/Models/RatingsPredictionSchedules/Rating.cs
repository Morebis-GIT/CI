using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.RatingsPredictionSchedules
{
    public class Rating
    {
        public DateTime Time { get; set; }
        public string Demographic { get; set; }
        public double NoOfRatings { get; set; }
    }
}
