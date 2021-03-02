using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.RatingsPredictionSchedules
{
    public class BulkRatingsPredictionScheduleCreated : IBulkRatingsPredictionSchedulesCreated
    {
        public IEnumerable<IRatingsPredictionScheduleCreated> Data { get; }

        public BulkRatingsPredictionScheduleCreated(IEnumerable<RatingsPredictionScheduleCreated> data) => Data = data;
    }
}
