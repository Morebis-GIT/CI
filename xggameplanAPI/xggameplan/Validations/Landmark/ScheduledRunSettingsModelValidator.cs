using FluentValidation;
using xggameplan.model.External;

namespace xggameplan.Validations.Landmark
{
    public class ScheduledRunSettingsModelValidator : ModelDataValidatorBase<ScheduledRunSettingsModel>
    {
        public ScheduledRunSettingsModelValidator(IValidator<ScheduledRunSettingsModel> validator) : base(validator)
        {
        }
    }
}
