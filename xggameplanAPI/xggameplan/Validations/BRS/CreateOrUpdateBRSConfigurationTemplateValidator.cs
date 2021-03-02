using FluentValidation;
using xggameplan.model.External;

namespace xggameplan.Validations.BRS
{
    public class CreateOrUpdateBRSConfigurationTemplateValidator : ModelDataValidatorBase<CreateOrUpdateBRSConfigurationTemplateModel>
    {
        public CreateOrUpdateBRSConfigurationTemplateValidator(IValidator<CreateOrUpdateBRSConfigurationTemplateModel> modelValidator)
            : base(modelValidator)
        {
        }
    }
}
