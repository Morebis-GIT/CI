using System;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Runs;

namespace xggameplan.model.External
{
    public class EfficiencySettingsModel
    {
        public Guid Id { get; set; }

        public EfficiencyCalculationPeriod EfficiencyCalculationPeriod { get; set; }
        public int? DefaultNumberOfWeeks { get; set; }
        public PersistEfficiency PersistEfficiency { get; set; }
    }
}
