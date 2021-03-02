using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateLibrarySalesAreaPassPriorityModelValidator : ModelDataValidatorBase<CreateLibrarySalesAreaPassPriorityModel>
                                                                    ,IModelDataValidator<CreateLibrarySalesAreaPassPriorityModel>
    {
        public CreateLibrarySalesAreaPassPriorityModelValidator(IValidator<CreateLibrarySalesAreaPassPriorityModel> modelValidator)
                                                                : base(modelValidator)
        {
        }
    }
}
