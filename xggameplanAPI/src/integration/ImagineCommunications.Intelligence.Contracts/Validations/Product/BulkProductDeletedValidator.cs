using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Product
{
    public class BulkProductDeletedValidator : AbstractValidator<IBulkProductDeleted>
    {
        public BulkProductDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new ProductDeletedValidator());
        }
    }
}
