using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.InventoryType
{
    public class InventoryTypeCreatedValidator : AbstractValidator<IInventoryTypeCreated>
    {
        public InventoryTypeCreatedValidator()
        {
            RuleFor(r => r.Description).NotEmpty();
            RuleFor(r => r.InventoryCode).NotEmpty();
            RuleFor(r => r.System).NotEmpty();
        }
    }
}
