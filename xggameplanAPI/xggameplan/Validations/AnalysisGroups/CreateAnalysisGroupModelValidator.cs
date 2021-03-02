using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations.AnalysisGroups
{
    public class CreateAnalysisGroupModelValidator : ModelDataValidatorBase<CreateAnalysisGroupModel>
    {
        public CreateAnalysisGroupModelValidator(IValidator<CreateAnalysisGroupModel> validator) : base(validator)
        {
        }
    }
}
