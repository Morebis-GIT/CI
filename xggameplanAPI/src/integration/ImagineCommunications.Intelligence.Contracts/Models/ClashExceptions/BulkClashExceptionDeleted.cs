using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ClashExceptions
{
    public class BulkClashExceptionDeleted : IBulkClashExceptionDeleted
    {
        public BulkClashExceptionDeleted(IEnumerable<ClashExceptionDeleted> data) => Data = data;

        public IEnumerable<IClashExceptionDeleted> Data { get; }
    }
}
