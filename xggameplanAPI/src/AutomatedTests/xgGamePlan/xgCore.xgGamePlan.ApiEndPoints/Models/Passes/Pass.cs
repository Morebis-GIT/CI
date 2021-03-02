using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class Pass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsLibraried { get; set; }
        public IEnumerable<GeneralModel> General { get; set; }
        public IEnumerable<WeightingModel> Weightings { get; set; }
        public IEnumerable<ToleranceModel> Tolerances { get; set; }
        public IEnumerable<RuleModel> Rules { get; set; }
        public IEnumerable<ProgrammeRepetitionModel> ProgrammeRepetitions { get; set; }
        public IEnumerable<BreakExclusionModel> BreakExclusions { get; set; }
        public IEnumerable<SlottingLimitModel> SlottingLimits { get; set; }
        public PassSalesAreaPriority PassSalesAreaPriorities { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
