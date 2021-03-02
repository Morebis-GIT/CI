using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.TotalRatings
{
    public class TotalRatingDeleted : ITotalRatingDeleted
    {
        public DateTime DateTimeFrom { get; }
        public DateTime DateTimeTo { get; }
        public string SalesArea { get; }

        public TotalRatingDeleted(DateTime dateTimeFrom, DateTime dateTimeTo, string salesArea)
        {
            DateTimeFrom = dateTimeFrom;
            DateTimeTo = dateTimeTo;
            SalesArea = salesArea;
        }
    }
}
