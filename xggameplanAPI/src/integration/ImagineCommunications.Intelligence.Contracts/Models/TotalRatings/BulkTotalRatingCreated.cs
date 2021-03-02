using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.TotalRatings
{
    public class BulkTotalRatingCreated : IBulkTotalRatingCreated
    {
        public IEnumerable<ITotalRatingCreated> Data { get; }

        public BulkTotalRatingCreated(IEnumerable<TotalRatingCreated> data) => Data = data;
    }
}
