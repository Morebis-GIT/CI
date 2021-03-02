using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Universe
{
    public class BulkUniverseDeleted : IBulkUniverseDeleted
    {
        public IEnumerable<IUniverseDeleted> Data { get; }

        public BulkUniverseDeleted(IEnumerable<UniverseDeleted> data) => Data = data;
    }
}
