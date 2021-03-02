using FluentValidation;
using xggameplan.model.External;

namespace xggameplan.Validations.Landmark
{
    public class ScheduledRunSettingsModelValidation : AbstractValidator<ScheduledRunSettingsModel>
    {
        public ScheduledRunSettingsModelValidation()
        {
            RuleFor(c => c.ScenarioId).NotEmpty();
            RuleFor(c => c.QueueName).NotEmpty();
            RuleFor(c => c.Priority).InclusiveBetween(500, 599);
        }
    }
}
