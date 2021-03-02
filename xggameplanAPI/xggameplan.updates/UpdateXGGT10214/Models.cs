using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;

namespace xggameplan.Updates.UpdateXGGT10214_Models
{
    internal class PassV1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? IsLibraried { get; set; } = false;
        public List<General> General = new List<General>();
        public List<Weighting> Weightings = new List<Weighting>();
        public List<Tolerance> Tolerances = new List<Tolerance>();
        public List<PassRule> Rules = new List<PassRule>();
        public List<TarpV1> Tarps = new List<TarpV1>();
        public List<ProgrammeRepetition> ProgrammeRepetitions = new List<ProgrammeRepetition>();
        public List<BreakExclusion> BreakExclusions = new List<BreakExclusion>();
        public List<SlottingLimit> SlottingLimits = new List<SlottingLimit>();
        public PassSalesAreaPriority PassSalesAreaPriorities = new PassSalesAreaPriority();
    }

    internal class TarpV1
    {
        public int Id { get; set; }
        public IEnumerable<string> SalesAreas { get; set; }
        public double? OffPeakValue { get; set; }
        public double? PeakValue { get; set; }
        public double? MidnightToDawnValue { get; set; }
    }
}
