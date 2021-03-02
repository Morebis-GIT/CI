using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Programme
{
    public class BulkProgrammeCreatedValidator : AbstractValidator<IBulkProgrammeCreated>
    {
        public BulkProgrammeCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new ProgrammeCreatedValidator());
        }
    }
}
