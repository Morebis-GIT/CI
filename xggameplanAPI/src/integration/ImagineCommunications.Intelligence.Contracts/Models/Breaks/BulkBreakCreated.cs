using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Breaks
{
    public class BulkBreakCreated : IBulkBreakCreated
    {
        public IEnumerable<IBreakCreated> Data { get; }

        public BulkBreakCreated(IEnumerable<BreakCreated> data)
        {
            Data = data;
        }
    }
}
