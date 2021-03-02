using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash
{
    public class BulkClashDeleted : IBulkClashDeleted
    {
        public IEnumerable<IClashDeleted> Data { get; }

        public BulkClashDeleted(IEnumerable<ClashDeleted> data) => Data = data;
    }
}
