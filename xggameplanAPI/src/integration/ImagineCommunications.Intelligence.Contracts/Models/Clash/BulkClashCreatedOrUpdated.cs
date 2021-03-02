using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash
{
    public class BulkClashCreatedOrUpdated : IBulkClashCreatedOrUpdated
    {
        public BulkClashCreatedOrUpdated(IEnumerable<ClashCreatedOrUpdated> data) => Data = data;

        public IEnumerable<IClashCreatedOrUpdated> Data { get; }
    }
}
