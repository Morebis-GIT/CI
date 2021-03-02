using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.RatingsPredictionSchedules
{
    public class BulkRatingsPredictionSchedulesDeleted : IBulkRatingsPredictionSchedulesDeleted
    {
        public IEnumerable<IRatingsPredictionSchedulesDeleted> Data { get; }

        public BulkRatingsPredictionSchedulesDeleted(IEnumerable<RatingsPredictionSchedulesDeleted> data) => Data = data;
    }
}
