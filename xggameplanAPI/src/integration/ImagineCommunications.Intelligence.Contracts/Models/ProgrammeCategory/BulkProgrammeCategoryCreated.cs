using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeCategory
{
    public class BulkProgrammeCategoryCreated : IBulkProgrammeCategoryCreated
    {
        public IEnumerable<IProgrammeCategoryCreated> Data { get; }

        public BulkProgrammeCategoryCreated(IEnumerable<ProgrammeCategoryCreated> data) => Data = data;
    }
}
