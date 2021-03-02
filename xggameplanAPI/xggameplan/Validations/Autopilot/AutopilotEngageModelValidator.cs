using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations.AutopilotSettings
{
    public class AutopilotEngageModelValidator : ModelDataValidatorBase<AutopilotEngageModel>, IModelDataValidator<AutopilotEngageModel>
    {
        public AutopilotEngageModelValidator(IValidator<AutopilotEngageModel> validator) : base(validator)
        {
        }
    }
}
