using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Breaks
{
    public class BulkBreaksDeleted : IBulkBreaksDeleted
    {
        public IEnumerable<IBreakDeleted> Data { get; }

        public BulkBreaksDeleted(IEnumerable<BreakDeleted> data)
        {
            Data = data;
        }
    }
}
