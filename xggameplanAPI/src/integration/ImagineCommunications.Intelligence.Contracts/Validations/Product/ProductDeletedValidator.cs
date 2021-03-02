using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Product
{
    public class ProductDeletedValidator : AbstractValidator<IProductDeleted>
    {
        public ProductDeletedValidator()
        {
            RuleFor(d => d.Externalidentifier).NotEmpty();
        }
    }
}
