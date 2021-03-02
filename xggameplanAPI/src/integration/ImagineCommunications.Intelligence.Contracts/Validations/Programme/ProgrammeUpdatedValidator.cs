using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Programme
{
    public class ProgrammeUpdatedValidator : AbstractValidator<IProgrammeUpdated>
    {
        public ProgrammeUpdatedValidator()
        {
            RuleFor(r => r.ExternalReference).NotEmpty();
            RuleFor(r => r.ProgrammeName).NotEmpty();
        }
    }
}
