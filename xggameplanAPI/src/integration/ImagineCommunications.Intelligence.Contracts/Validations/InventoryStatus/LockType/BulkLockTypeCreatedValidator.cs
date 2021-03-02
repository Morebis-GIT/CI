using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.LockType
{
    public class BulkLockTypeCreatedValidator : AbstractValidator<IBulkLockTypeCreated>
    {
        public BulkLockTypeCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new LockTypeCreatedValidator());

            RuleFor(r => r.Data)
                .Must(x => x.IsUnique(u => u.LockType))
                .WithErrorCode("Lock Type must be unique");

            RuleFor(r => r.Data)
                .Must(x => x.IsUnique(u => u.Name))
                .WithErrorCode("Name must be unique");
        }
    }
}
