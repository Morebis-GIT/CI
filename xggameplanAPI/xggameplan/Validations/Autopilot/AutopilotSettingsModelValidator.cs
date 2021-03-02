using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations.AutopilotSettings
{
    public class AutopilotSettingsModelValidator : ModelDataValidatorBase<UpdateAutopilotSettingsModel>, IModelDataValidator<UpdateAutopilotSettingsModel>
    {
        public AutopilotSettingsModelValidator(IValidator<UpdateAutopilotSettingsModel> validator) : base(validator)
        {
        }
    }
}
