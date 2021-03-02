using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class UpdateLibrarySalesAreaPassPriorityModelValidator : ModelDataValidatorBase<UpdateLibrarySalesAreaPassPriorityModel>
                                                                   ,IModelDataValidator<UpdateLibrarySalesAreaPassPriorityModel>
    {
        public UpdateLibrarySalesAreaPassPriorityModelValidator(IValidator<UpdateLibrarySalesAreaPassPriorityModel> modelValidator)
                                                               : base(modelValidator)
        {
        }
    }
}
