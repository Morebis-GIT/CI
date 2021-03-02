using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.RatingsPredictionSchedules
{
    public class RatingsPredictionSchedulesDeleted : IRatingsPredictionSchedulesDeleted
    {
        public RatingsPredictionSchedulesDeleted(DateTime dateTimeFrom, DateTime dateTimeTo, string salesArea)
        {
            DateTimeFrom = dateTimeFrom;
            DateTimeTo = dateTimeTo;
            SalesArea = salesArea;
        }

        public DateTime DateTimeFrom { get; }

        public DateTime DateTimeTo { get; }

        public string SalesArea { get; }
    }
}
