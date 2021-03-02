using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme
{
    public class BulkProgrammeDeleted : IBulkProgrammeDeleted
    {
        public IEnumerable<IProgrammesDeleted> Data { get; }

        public BulkProgrammeDeleted(IEnumerable<ProgrammesDeleted> data)
        {
            Data = data;
        }
    }
}
