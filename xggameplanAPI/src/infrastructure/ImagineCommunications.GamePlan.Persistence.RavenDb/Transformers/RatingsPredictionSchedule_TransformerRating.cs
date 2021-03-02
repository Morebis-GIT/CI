using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class RatingsPredictionSchedule_TransformerRating
        : AbstractTransformerCreationTask<RatingsPredictionSchedule>
    {
        public class Result
        {
            public string SalesArea { get; set; }
            public DateTime ScheduleDay { get; set; }
            public string Demographic { get; set; }

            public int Count { get; set; }
        }

        public RatingsPredictionSchedule_TransformerRating()
        {
            TransformResults = ratingsPredictionSchedules =>
                from schedule in ratingsPredictionSchedules
                from rating in schedule.Ratings
                group rating by
                new
                {
                    schedule.SalesArea,
                    schedule.ScheduleDay,
                    rating.Demographic
                }
                into agg
                select new
                {
                    agg.Key.SalesArea,
                    agg.Key.ScheduleDay,
                    agg.Key.Demographic,
                    Count = agg.Sum(x => 1)
                };
        }
    }
}
