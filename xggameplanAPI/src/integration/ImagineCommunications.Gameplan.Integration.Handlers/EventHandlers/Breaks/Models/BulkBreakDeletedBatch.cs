using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Breaks.Models
{
    internal class BulkBreakDeletedBatch : IBulkBreaksDeleted
    {
        public IEnumerable<IBreakDeleted> Data { get; }

        public BulkBreakDeletedBatch(IEnumerable<IBreakDeleted> data)
        {
            Data = data;
        }
    }
}
