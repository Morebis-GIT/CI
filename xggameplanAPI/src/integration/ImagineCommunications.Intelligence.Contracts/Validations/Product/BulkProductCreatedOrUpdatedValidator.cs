using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Product
{
    public class BulkProductCreatedOrUpdatedValidator : AbstractValidator<IBulkProductCreatedOrUpdated>
    {
        public BulkProductCreatedOrUpdatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new ProductCreatedOrUpdatedValidator());
        }
    }
}
