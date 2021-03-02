using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ProgrammeClassification
{
    public class ProgrammeClassificationCreatedValidator : AbstractValidator<IProgrammeClassificationCreated>
    {
        public ProgrammeClassificationCreatedValidator()
        {
            RuleFor(r => r.Uid).GreaterThan(0);
            RuleFor(r => r.Code).NotEmpty();
            RuleFor(r => r.Description).NotEmpty();
        }
    }
}
