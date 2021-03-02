using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.BreakTypes
{
    public class BulkBreakTypeCreatedValidator : AbstractValidator<IBulkBreakTypeCreated>
    {
        public BulkBreakTypeCreatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(x => new BreakTypeCreatedValidator());

            RuleFor(x => x.Data)
                .Must(x => x.IsUnique(u => u.Name))
                .WithErrorCode("Name must be unique");
        }
    }
}
