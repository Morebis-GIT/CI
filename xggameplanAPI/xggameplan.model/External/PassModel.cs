using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class PassModel : ICloneable
    {
        public int Id { get; set; }
        public int PassId => Id;
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
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public object Clone()
        {
            var passModel = (PassModel)MemberwiseClone();

            if (General != null)
            {
                passModel.General = new List<GeneralModel>();
                General.ForEach(gm => passModel.General.Add((GeneralModel)gm.Clone()));
            }

            if (Weightings != null)
            {
                passModel.Weightings = new List<WeightingModel>();
                Weightings.ForEach(wm => passModel.Weightings.Add((WeightingModel)wm.Clone()));
            }

            if (Tolerances != null)
            {
                passModel.Tolerances = new List<ToleranceModel>();
                Tolerances.ForEach(tm => passModel.Tolerances.Add((ToleranceModel)tm.Clone()));
            }

            if (Rules != null)
            {
                passModel.Rules = new List<PassRuleModel>();
                Rules.ForEach(rm => passModel.Rules.Add((PassRuleModel)rm.Clone()));
            }

            if (RatingPoints != null)
            {
                passModel.RatingPoints = new List<RatingPointModel>();
                RatingPoints.ForEach(trp => passModel.RatingPoints.Add((RatingPointModel)trp.Clone()));
            }

            if (ProgrammeRepetitions != null)
            {
                passModel.ProgrammeRepetitions = new List<ProgrammeRepetitionModel>();
                ProgrammeRepetitions.ForEach(pm => passModel.ProgrammeRepetitions.Add((ProgrammeRepetitionModel)pm.Clone()));
            }

            if (BreakExclusions != null)
            {
                passModel.BreakExclusions = new List<BreakExclusionModel>();
                BreakExclusions.ForEach(bm => passModel.BreakExclusions.Add((BreakExclusionModel)bm.Clone()));
            }

            if (SlottingLimits != null)
            {
                passModel.SlottingLimits = new List<SlottingLimitModel>();
                SlottingLimits.ForEach(bm => passModel.SlottingLimits.Add((SlottingLimitModel)bm.Clone()));
            }

            if (PassSalesAreaPriorities != null)
            {
                passModel.PassSalesAreaPriorities = (PassSalesAreaPriorityModel)PassSalesAreaPriorities.Clone();
            }

            return passModel;
        }
    }
}
