using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.TotalRatings
{
    public class BulkTotalRatingDeleted : IBulkTotalRatingDeleted
    {
        public IEnumerable<ITotalRatingDeleted> Data { get; }

        public BulkTotalRatingDeleted(IEnumerable<TotalRatingDeleted> data) => Data = data;
    }
}
