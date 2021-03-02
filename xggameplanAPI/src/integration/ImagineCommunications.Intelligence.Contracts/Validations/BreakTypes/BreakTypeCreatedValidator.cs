using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.BreakTypes
{
    public class BreakTypeCreatedValidator : AbstractValidator<IBreakTypeCreated>
    {
        public BreakTypeCreatedValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
