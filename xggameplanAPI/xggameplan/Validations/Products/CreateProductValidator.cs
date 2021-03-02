using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateProductValidator : ModelDataValidatorBase<CreateProduct>, IModelDataValidator<CreateProduct>
    {
        public CreateProductValidator(IValidator<CreateProduct> modelValidator)
            : base(modelValidator)
        {
        }
    }
}
