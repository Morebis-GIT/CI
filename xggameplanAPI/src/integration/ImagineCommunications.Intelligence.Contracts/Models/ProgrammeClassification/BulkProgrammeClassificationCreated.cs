using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeClassification
{
    public class BulkProgrammeClassificationCreated : IBulkProgrammeClassificationCreated
    {
        public BulkProgrammeClassificationCreated(IEnumerable<ProgrammeClassificationCreated> data) => Data = data;

        public IEnumerable<IProgrammeClassificationCreated> Data { get; }
    }
}
