using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme
{
    public class BulkProgrammeUpdated : IBulkProgrammeUpdated
    {
        public BulkProgrammeUpdated(IEnumerable<ProgrammeUpdated> data)
        {
            Data = data;
        }

        public IEnumerable<IProgrammeUpdated> Data { get; }
    }
}
