using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ProgrammeCategory
{
    public class ProgrammeCategoryCreatedValidator : AbstractValidator<IProgrammeCategoryCreated>
    {
        public ProgrammeCategoryCreatedValidator()
        {
            RuleFor(r => r.Name).NotEmpty();
        }
    }
}
