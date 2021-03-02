using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Spot
{
    public class BulkSpotDeleted : IBulkSpotDeleted
    {
        public BulkSpotDeleted(IEnumerable<SpotDeleted> data) => Data = data;

        public IEnumerable<ISpotDeleted> Data { get; }
    }
}
