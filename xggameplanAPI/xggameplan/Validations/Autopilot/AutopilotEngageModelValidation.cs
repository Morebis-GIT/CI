using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class AutopilotEngageModelValidation : AbstractValidator<AutopilotEngageModel>
    {
        public AutopilotEngageModelValidation(IFlexibilityLevelRepository flexibilityLevelRepository)
        {
            RuleFor(s => s.FlexibilityLevelId)
                .Must(id => id == 0 || flexibilityLevelRepository.Get(id) != null)
                .WithMessage(s => $"FlexibilityLevel with identifier {s.FlexibilityLevelId} does not exist");

            RuleFor(s => s.Scenarios).Must(s => s != null && s.Any()).WithMessage("Please provide at least one scenario");

            RuleForEach(s => s.Scenarios).SetValidator(new AutopilotScenarioEngageModelValidation());
        }
    }
}
