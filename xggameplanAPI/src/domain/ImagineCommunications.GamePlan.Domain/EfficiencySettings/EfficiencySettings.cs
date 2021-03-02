using System;
using ImagineCommunications.GamePlan.Domain.Runs;

namespace ImagineCommunications.GamePlan.Domain.EfficiencySettings
{
    public class EfficiencySettings
    {
        public Guid Id { get; set; }
        public EfficiencyCalculationPeriod EfficiencyCalculationPeriod { get; set; }
        public int? DefaultNumberOfWeeks { get; set; }
        public PersistEfficiency PersistEfficiency { get; set; }

        public EfficiencySettings FulfillFrom(EfficiencySettings settings)
        {
            EfficiencyCalculationPeriod = settings.EfficiencyCalculationPeriod;
            DefaultNumberOfWeeks = settings.DefaultNumberOfWeeks;
            PersistEfficiency = settings.PersistEfficiency;

            return this;
        }

        public (bool success, string validationErrorMessage) ValidateForSave()
        {
            var validationErrorMessage = string.Empty;

            const int minimumNumberOfWeeks = 1;
            const int maximumNumberOfWeeks = 25;

            if (EfficiencyCalculationPeriod == EfficiencyCalculationPeriod.NumberOfWeeks)
            {
                if (!DefaultNumberOfWeeks.HasValue)
                {
                    validationErrorMessage = "Number of weeks can't be empty if Efficiency Period is 'Number of weeks'";
                }
                else if (DefaultNumberOfWeeks.Value < minimumNumberOfWeeks || DefaultNumberOfWeeks.Value > maximumNumberOfWeeks)
                {
                    validationErrorMessage = $"{nameof(EfficiencyCalculationPeriod.NumberOfWeeks)} " +
                                             $"value must be between {minimumNumberOfWeeks} and {maximumNumberOfWeeks}";
                }
            }

            return (string.IsNullOrWhiteSpace(validationErrorMessage), validationErrorMessage);
        }
    }
}
