using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Spot
{
    public class BulkSpotCreatedOrUpdated : IBulkSpotCreatedOrUpdated
    {
        public IEnumerable<ISpotCreatedOrUpdated> Data { get; }

        public BulkSpotCreatedOrUpdated(IEnumerable<SpotCreatedOrUpdated> data) => Data = data;
    }
}
