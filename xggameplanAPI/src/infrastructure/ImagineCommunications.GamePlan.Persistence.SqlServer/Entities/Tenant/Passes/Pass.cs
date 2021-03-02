using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class Pass : IAuditEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsLibraried { get; set; }
        public List<PassRuleGeneral> General { get; set; }
        public List<PassRuleWeighting> Weightings { get; set; }
        public List<PassRuleTolerance> Tolerances { get; set; }
        public List<PassRule> Rules { get; set; }
        public List<PassRatingPoint> RatingPoints { get; set; }
        public List<PassProgrammeRepetition> ProgrammeRepetitions { get; set; }
        public List<PassBreakExclusion> BreakExclusions { get; set; }
        public List<PassSlottingLimit> SlottingLimits { get; set; }

        public PassSalesAreaPriorityCollection PassSalesAreaPriorities { get; set; }

        public const string SearchField = "TokenizedName";
    }
}
