using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme
{
    public class BulkProgrammeCreated : IBulkProgrammeCreated
    {
        public BulkProgrammeCreated(IEnumerable<ProgrammeCreated> data) => Data = data;

        public IEnumerable<IProgrammeCreated> Data { get; }
    }
}
