using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class ProgrammeRestriction
    {
        public ProgrammeRestriction(List<string> salesAreas, List<string> categoryOrProgramme, string isCategoryOrProgramme, string isIncludeOrExclude)
        {
            SalesAreas = salesAreas;
            CategoryOrProgramme = categoryOrProgramme;
            IsCategoryOrProgramme = isCategoryOrProgramme?.ToUpperInvariant();
            IsIncludeOrExclude = isIncludeOrExclude?.ToUpperInvariant();
        }

        public string IsCategoryOrProgramme { get; }
        public string IsIncludeOrExclude { get; }

        public List<string> SalesAreas { get; }

        public List<string> CategoryOrProgramme { get; }
    }
}
