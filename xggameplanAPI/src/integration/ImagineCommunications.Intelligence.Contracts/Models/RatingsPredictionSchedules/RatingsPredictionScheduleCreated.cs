using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.RatingsPredictionSchedules
{
    public class RatingsPredictionScheduleCreated : IRatingsPredictionScheduleCreated
    {
        public RatingsPredictionScheduleCreated(string salesArea, DateTime scheduleDay, IEnumerable<RatingModel> ratings)
        {
            SalesArea = salesArea;
            ScheduleDay = scheduleDay;
            Ratings = ratings;
        }

        public string SalesArea { get; }

        public DateTime ScheduleDay { get; }

        public IEnumerable<RatingModel> Ratings { get; }
    }
}
