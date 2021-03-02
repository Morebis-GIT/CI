using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreatePassModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLibraried { get; set; }

        public List<GeneralModel> General = new List<GeneralModel>();

        public List<WeightingModel> Weightings = new List<WeightingModel>();

        public List<ToleranceModel> Tolerances = new List<ToleranceModel>();

        public List<PassRuleModel> Rules = new List<PassRuleModel>();

        public List<RatingPointModel> RatingPoints = new List<RatingPointModel>();

        public List<ProgrammeRepetitionModel> ProgrammeRepetitions = new List<ProgrammeRepetitionModel>();

        public List<BreakExclusionModel> BreakExclusions = new List<BreakExclusionModel>();

        public List<SlottingLimitModel> SlottingLimits = new List<SlottingLimitModel>();

        public PassSalesAreaPriorityModel PassSalesAreaPriorities = new PassSalesAreaPriorityModel();
    }
}
