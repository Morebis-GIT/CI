using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class RatingsPredictionScheduleModel
    {

        public string SalesArea { get; set; }
        public DateTime ScheduleDay { get; set; }

        public IEnumerable<RatingModel> Ratings { get; set; }
    }

    public class CreateRatingsPredictionSchedule
    {

        public string SalesArea { get; set; }
        public DateTime ScheduleDay { get; set; }

        public IEnumerable<RatingModel> Ratings { get; set; }
    }


    public class RatingModel
    {
        public DateTime Time { get; set; }
        public string Demographic { get; set; }
        public double NoOfRatings { get; set; }
    }


}
