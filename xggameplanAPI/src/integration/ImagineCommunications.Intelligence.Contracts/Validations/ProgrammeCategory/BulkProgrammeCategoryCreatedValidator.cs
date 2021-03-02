using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ProgrammeCategory
{
    public class BulkProgrammeCategoryCreatedValidator : AbstractValidator<IBulkProgrammeCategoryCreated>
    {
        public BulkProgrammeCategoryCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new ProgrammeCategoryCreatedValidator());
            RuleFor(r => r.Data)
                .Must(x => x.IsUnique(u => u.Name))
                .WithErrorCode("Name must be unique");
        }
    }
}
