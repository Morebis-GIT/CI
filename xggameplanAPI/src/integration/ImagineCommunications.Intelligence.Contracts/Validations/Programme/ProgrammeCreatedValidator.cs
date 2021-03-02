using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Programme
{
    public class ProgrammeCreatedValidator : AbstractValidator<IProgrammeCreated>
    {
        public ProgrammeCreatedValidator()
        {
            RuleFor(r => r.SalesArea).NotEmpty();
            RuleFor(r => r.ExternalReference).NotEmpty();
            RuleFor(r => r.ProgrammeName).NotEmpty();
            RuleFor(r => r.StartDateTime).NotEmpty();
            RuleFor(r => r.Duration).NotEmpty();
        }
    }
}
