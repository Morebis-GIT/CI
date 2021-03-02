using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Runs;
using xggameplan.Model;

namespace xggameplan.Validations.Landmark
{
    public class LandmarkRunTriggerModelValidation : AbstractValidator<LandmarkRunTriggerModel>
    {
        public LandmarkRunTriggerModelValidation(IRunRepository runRepository)
        {
            RuleFor(c => c.ScenarioId).NotEmpty().WithMessage("Scenario id missing");

            RuleFor(c => c).Custom((command, context) =>
            {
                var run = runRepository.FindByScenarioId(command.ScenarioId);
                var scenario = run?.Scenarios.Find(s => s.Id == command.ScenarioId);

                if (run is null || scenario is null)
                {
                    context.AddFailure($"Run for scenario {command.ScenarioId} was not found");
                    return;
                }

                if (!run.IsCompleted)
                {
                    context.AddFailure("Run is not completed yet");
                }

                if (!run.IsOptimiserNeeded)
                {
                    context.AddFailure("Run has no optimizations enabled");
                }
            });
        }
    }
}
