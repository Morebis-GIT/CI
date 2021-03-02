using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Programme
{
    public class BulkProgrammeUpdatedValidator : AbstractValidator<IBulkProgrammeUpdated>
    {
        public BulkProgrammeUpdatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new ProgrammeUpdatedValidator());
        }
    }
}
