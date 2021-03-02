using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ClashException
{
    public class BulkClashExceptionDeletedValidator : AbstractValidator<IBulkClashExceptionDeleted>
    {
        public BulkClashExceptionDeletedValidator() => RuleFor(x => x.Data).NotEmpty();
    }
}
