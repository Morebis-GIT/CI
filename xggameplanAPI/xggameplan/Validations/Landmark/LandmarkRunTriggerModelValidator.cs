using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations.Landmark
{
    public class LandmarkRunTriggerModelValidator : ModelDataValidatorBase<LandmarkRunTriggerModel>
    {
        public LandmarkRunTriggerModelValidator(IValidator<LandmarkRunTriggerModel> validator) : base(validator)
        {
        }
    }
}
