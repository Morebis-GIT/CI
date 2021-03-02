using System.Collections.Generic;
using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateSponsorshipsModelValidator : ModelDataValidatorBase<IEnumerable<CreateSponsorshipModel>>,
                                                    IModelDataValidator<IEnumerable<CreateSponsorshipModel>>
    {
        public CreateSponsorshipsModelValidator(IValidator<IEnumerable<CreateSponsorshipModel>> modelValidator) : base(modelValidator)
        {
        }
    }
}
