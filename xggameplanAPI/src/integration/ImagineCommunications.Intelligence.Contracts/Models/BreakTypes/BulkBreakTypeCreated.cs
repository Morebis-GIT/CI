using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.BreakTypes
{
    public class BulkBreakTypeCreated : IBulkBreakTypeCreated
    {
        public IEnumerable<IBreakTypeCreated> Data { get; }

        public BulkBreakTypeCreated(IEnumerable<BreakTypeCreated> data)
        {
            Data = data;
        }
    }
}
