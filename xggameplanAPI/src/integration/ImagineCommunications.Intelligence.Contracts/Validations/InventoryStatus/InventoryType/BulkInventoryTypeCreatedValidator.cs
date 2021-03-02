using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.InventoryType
{
    public class BulkInventoryTypeCreatedValidator : AbstractValidator<IBulkInventoryTypeCreated>
    {
        public BulkInventoryTypeCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new InventoryTypeCreatedValidator());
        }
    }
}
