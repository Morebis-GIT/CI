using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.LockType
{
    public class LockTypeCreatedValidator : AbstractValidator<ILockTypeCreated>
    {
        public LockTypeCreatedValidator()
        {
            RuleFor(r => r.LockType).NotEmpty();
            RuleFor(r => r.Name).NotEmpty();
        }
    }
}
