using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme.Models
{
    internal class BulkProgrammeDeletedBatch : IBulkProgrammeDeleted
    {
        public IEnumerable<IProgrammesDeleted> Data { get; }

        public BulkProgrammeDeletedBatch(IEnumerable<IProgrammesDeleted> data)
        {
            Data = data;
        }
    }
}
