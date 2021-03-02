using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Universe
{
    public class BulkUniverseCreated : IBulkUniverseCreated
    {
        public BulkUniverseCreated(IEnumerable<UniverseCreated> data) => Data = data;

        public IEnumerable<IUniverseCreated> Data { get; }
    }
}
