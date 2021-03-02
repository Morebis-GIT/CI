using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateSponsorshipModelValidator : ModelDataValidatorBase<CreateSponsorshipModel>,
                                                   IModelDataValidator<CreateSponsorshipModel>
    {
        public CreateSponsorshipModelValidator(IValidator<CreateSponsorshipModel> modelValidator) : base(modelValidator)
        {
        }
    }
}
