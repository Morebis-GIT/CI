using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations.DeliveryCappingGroup
{
    public class DeliveryCappingGroupValidator : ModelDataValidatorBase<DeliveryCappingGroupModel>
    {
        public DeliveryCappingGroupValidator(IValidator<DeliveryCappingGroupModel> modelValidator)
            : base(modelValidator)
        {
        }
    }
}
