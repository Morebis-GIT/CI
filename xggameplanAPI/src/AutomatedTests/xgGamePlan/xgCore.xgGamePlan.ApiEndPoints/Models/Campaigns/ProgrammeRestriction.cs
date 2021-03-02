using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class ProgrammeRestriction
    {
        public IEnumerable<string> SalesAreas { get; set; }
        public IEnumerable<string> CategoryOrProgramme { get; set; }
        public string IsIncludeOrExclude { get; set; }
        public string IsCategoryOrProgramme { get; set; }
    }
}
