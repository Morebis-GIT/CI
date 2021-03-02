using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class AutopilotSettingsModelValidation : AbstractValidator<UpdateAutopilotSettingsModel>
    {
        public AutopilotSettingsModelValidation(IFlexibilityLevelRepository flexibilityLevelRepository)
        {
            RuleFor(s => s.DefaultFlexibilityLevelId)
                .Must(id => flexibilityLevelRepository.Get(id) != null)
                .WithMessage(s => $"FlexibilityLevel with identifier {s.DefaultFlexibilityLevelId} does not exist");

            RuleFor(s => s.ScenariosToGenerate).InclusiveBetween(1, 8).WithMessage("Please specify valid scenario types count to be generated");
        }
    }
}
