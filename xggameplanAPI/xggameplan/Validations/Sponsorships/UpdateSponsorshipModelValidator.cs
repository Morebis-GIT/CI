using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class UpdateSponsorshipModelValidator : ModelDataValidatorBase<UpdateSponsorshipModel>,
                                                 IModelDataValidator<UpdateSponsorshipModel>
    {
        public UpdateSponsorshipModelValidator(IValidator<UpdateSponsorshipModel> modelValidator) : base(modelValidator)
        {
        }
    }
}
