using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.InventoryStatus.InventoryLock
{
    public class BulkInventoryLockDeletedValidator : AbstractValidator<IBulkInventoryLockDeleted>
    {
        public BulkInventoryLockDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
        }
    }
}
