using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.InventoryLock
{
    public class BulkInventoryLockCreatedValidator : AbstractValidator<IBulkInventoryLockCreated>
    {
        public BulkInventoryLockCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
        }
    }
}
