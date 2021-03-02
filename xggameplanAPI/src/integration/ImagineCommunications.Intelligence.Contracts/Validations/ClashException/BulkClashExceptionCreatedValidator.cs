using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ClashException
{
    public class BulkClashExceptionCreatedValidator: AbstractValidator<IBulkClashExceptionCreated>
    {
        public BulkClashExceptionCreatedValidator()
        {
            RuleFor(d => d.Data).NotEmpty();
            RuleForEach(d => d.Data).SetValidator(t => new ClashExceptionCreatedValidator());
        }
    }
}
