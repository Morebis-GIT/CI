using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ClashExceptions
{
    public class BulkClashExceptionCreated : IBulkClashExceptionCreated
    {
        public BulkClashExceptionCreated(IEnumerable<ClashExceptionCreated> data) => Data = data;

        public IEnumerable<IClashExceptionCreated> Data { get; }
    }
}
