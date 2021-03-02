using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class AutopilotScenarioEngageModelValidation : AbstractValidator<AutopilotScenarioEngageModel>
    {
        public AutopilotScenarioEngageModelValidation()
        {
            RuleFor(s => s.Name).NotEmpty().WithMessage("Scenario name must be set");
            RuleFor(s => s.Passes).Must(passes => passes != null && passes.Count > 0).WithMessage("Scenario has no passes");

            RuleFor(s => s.LoosenPassIndex).NotNull().WithMessage(s => "Please specify loosen pass index");
            RuleFor(s => s.TightenPassIndex).NotNull().WithMessage(s => "Please specify tighten pass index");
            RuleFor(s => s.LoosenPassIndex).GreaterThanOrEqualTo(0).WithMessage(s => "Please specify valid loosen pass index");
            RuleFor(s => s.TightenPassIndex).GreaterThanOrEqualTo(0).WithMessage(s => "Please specify valid tighten pass index");

            RuleFor(s => s.LoosenPassIndex).Must(IsPresentInPasses).WithMessage(s => "Specified loosen pass index is not present in the passes collection");
            RuleFor(s => s.TightenPassIndex).Must(IsPresentInPasses).WithMessage(s => "Specified tighten pass index is not present in the passes collection");

            RuleFor(s => s.TightenPassIndex).LessThanOrEqualTo(s => s.LoosenPassIndex).WithMessage(s => "Tighten pass must go first");
        }

        private static bool IsPresentInPasses(AutopilotScenarioModel model, int? passIndex) => passIndex < model.Passes.Count;
    }
}
