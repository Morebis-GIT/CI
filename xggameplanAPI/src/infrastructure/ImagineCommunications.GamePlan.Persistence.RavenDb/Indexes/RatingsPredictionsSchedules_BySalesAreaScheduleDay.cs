using System.Linq;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class RatingsPredictionsSchedules_BySalesAreaScheduleDay
        : AbstractIndexCreationTask<RatingsPredictionSchedule>
    {
        public static string DefaultIndexName => "RatingsPredictionsSchedules/BySalesAreaScheduleDay";

        public RatingsPredictionsSchedules_BySalesAreaScheduleDay()
        {
            Map = ratingsPredictionsSchedules =>
                from ratingsPredictionsSchedule in ratingsPredictionsSchedules
                select new
                {
                    ratingsPredictionsSchedule.SalesArea,
                    ratingsPredictionsSchedule.ScheduleDay
                };
        }
    }
}
